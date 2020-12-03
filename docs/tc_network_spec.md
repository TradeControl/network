# Network - Technical Spec

Trade Control is a recursive application. Recursion is used to model workflows within the node and allows them to be connected together. The inputs of each node are therefore designed to be a mirror image of its outputs, enabling them to be plugged together in supply-chains of any depth or complexity.

The core algorithms of the node are expressed in T-Sql, whilst they are connected through Ethereum via a c# interface. While [the tutorial](https://tradecontrol.github.io/tutorials/network) shows how these elements function together, the following document provides links to the code that implements this network.

## Solidity Contracts

The contracts have been written in VS Code using [Nethereum](https://github.com/Nethereum/Nethereum). To connect the contracts to the network interface, the Code Workspace is placed in a *solidity* sub-folder of the tcNetwork project. When the contracts are compiled, the solidity compiler outputs the byte code into *.bin files and Nethereum generates corresponding c# contract definitions. From Visual Studio, the service and definition classes are referenced by tcNetwork and the bytecode is obtained directly from Resources:   

``` csharp
string OrgByteCode
{
    get
    {
        byte[] source = (byte[])Properties.Resources.ResourceManager.GetObject("OrgByteCode");
        return Encoding.UTF8.GetString(source);
    }
}
```

To work on a private network, the contracts are in conformity with ``` pragma solidity >=0.4.21 <=0.5.12 ```; and the contracts are [mortal](../src/tcNetwork/solidity/contracts/mortal.sol), meaning they can be wiped off the test net by the owner. In a live environment, Invoices in particular would not be mortal.

### Design Constraints

1. Contracts cannot call other contracts 
2. An Externally Owned Account (EOA) can only modify their own contracts

The only exception to rule 2 is the [invoicing procedure](#invoice-mirrors), where an EOA signs off the other party's contract through mirroring.

### Org

The [Org Contract](../src/tcNetwork/solidity/contracts/Org.sol) is an extension of a node's Org schema and it is implemented using a library called [OrgLib](../src/tcNetwork/solidity/contracts/OrgLib.sol). The functions of the library are **internal** for compilation to a private network but could be compiled to a public address. Events are triggered outside the library simply because Nethereum does not parse them into the c# definition.

Each EOA has its own copy of the contract. The collection of connected Org contracts is called a Consortium. The contract contains references to the contract addresses of its member's Tasks and Invoices.  Members notify each other of new or updated contracts by raising events on their respective contracts.

The contract also contains mirrors of Activity and Cash Codes. Ideally, activities would be modelled in contracts: for communicating attributes, catalogue numbers, price lists and so on (whereby object, subject and project == Activity, Org and Task); but for the moment it is mirrored by a string rather than an address.

### Task

The [Task Contract](../src/tcNetwork/solidity/contracts/Task.sol) is an extension of the node's Task schema. By now, you will know that there are no inherent sales or purchase orders, nor customers and suppliers in the schema design. However, it is not possible to mirror tasks directly because trade operates in terms of supply and demand. One supply often fulfils many demands. Instead, we mirror [allocations](#allocations). Whether or not a Task contract is on the supply or demand side is determined by its polarity. If the Cash Polarity is negative, the Quantity must be positive, making the Task a demand-side allocation. Switch that around and it becomes a supply-side order. You can see that working in the [Network Transactions](https://tradecontrol.github.io/tutorials/network#network-transactions) section of the demo.

Ethereum does not support decimals. Therefore, task quantities and dates are converted by the following interface functions:

``` csharp
#region ethereum storage conversions
Func<long, DateTime> FromUnixEpoch = (ms) => new DateTime(1970, 1, 1).AddMilliseconds(ms);
Func<DateTime, long> ToUnixEpoch = (dt) => (long)(dt - new DateTime(1970, 1, 1)).TotalMilliseconds;

readonly Func<decimal, short, BigInteger> ToEthDecimalStorage = (v, dp) => new BigInteger(Math.Round((double)v * Math.Pow(10, dp), 0));
readonly Func<BigInteger, short, decimal> FromEthDecimalStorage = (v, dp) => decimal.Parse(v.ToString()) * (decimal)Math.Pow(10, dp * -1);

const short EVM_CHARGE_DP = 5;
const short EVM_QUANTITY_DP = 3;
const short EVM_TAX_RATE_DP = 3;
#endregion
```

When task contracts are deployed, the owner raises a notify event on the Org contract of the target account, communicating the task's contract address. The target can either watch for future events on that contract (active mode) or wait for further notifications (passive mode).  

### Invoice

The [Invoice Contract](../src/tcNetwork/solidity/contracts/Invoice.sol) is an extension of the node's Invoice schema. The contract has details that correspond to the schema Invoice.tbItem and Invoice.tbTask definitions. Each invoice contract is mirrored according to the polarity of the demand to pay. There are two polarities required to model each invoice type:

``` javascript
    enum CashPolarity { Expense, Income }

    CashPolarity invoicePolarity;
    CashPolarity paymentPolarity;
```

Upon contract deployment, the target account is notified of its address by a call to their Org contract. They will mirror the invoice according to the following table:

| Demand | Demand Invoice | Demand Payment | Mirror | Mirror Invoice | Mirror Payment |
| -- | -- | -- | -- | -- | -- |
| Sales Invoice | POS | POS | Purchase Invoice | NEG | NEG |
| Debit Note | NEG | POS | Credit Note | POS | NEG |

Mirrored invoices are not identical reflections. They add up to the same value, one negative and the other positive, but their details are allocations and, in consequence, they can be very different. In addition, a mirror emits its own events - payment scheduling, status changes and fulfilments. This is covered in [the tutorial](https://tradecontrol.github.io/tutorials/network#payment).

## Network Interface

The network interface is written in c# by asynchronous calls to the Nethereum generated service and contract definitions (Org, Task and Invoice). These routines can be found in the [tcWeb3 class](../src/tcNetwork/tcWeb3.cs) of the VS tcNetwork project.

Because the service accepts its own outputs as inputs, you can only debug one side of the transaction (unless you have multiple instances of the same code, which is ill advised). It is involved, so download the repository and open the network.sln file in Visual Studio to investigate the code. Each node on the network is polling using the same *PassiveWatch()* routine; therefore, that is the best place to start. The Watch function opens a new background thread and polls both the consortium for events and the node for new deployments and updates:

``` csharp
async void PassiveWatch()
{
    try
    {
        var consortium = new OrgService(web3, ConsortiumAddress);
        var taskNotifyEventHandler = web3.Eth.GetEvent<OnTaskNotifyEventDTO>(ConsortiumAddress);
        var activityMirrorEventHandler = web3.Eth.GetEvent<OnActivityMirrorEventDTO>(ConsortiumAddress);
        var invoiceNotifyEventHandler = web3.Eth.GetEvent<OnInvoiceNotifyEventDTO>(ConsortiumAddress);
        var cashCodeMirrorEventHandler = web3.Eth.GetEvent<OnCashCodeMirrorEventDTO>(ConsortiumAddress);

        List<string> accountEOAs = new List<string>();

        foreach (string accountCode in _passiveModeAccounts)
            accountEOAs.Add(await consortium.GetEOAQueryAsync(accountCode));

        var taskNotifyFilter = await taskNotifyEventHandler.CreateFilterAsync(accountEOAs.ToArray());
        var activityMirrorFilter = await activityMirrorEventHandler.CreateFilterAsync(accountEOAs.ToArray());
        var invoiceNotifyFilter = await invoiceNotifyEventHandler.CreateFilterAsync(accountEOAs.ToArray());
        var cashCodeMirrorFilter = await cashCodeMirrorEventHandler.CreateFilterAsync(accountEOAs.ToArray());

        while (!threadInterrupt)
        {
            bool activitiesMirrored = await ActivityMirrors(consortium);
            if (!activitiesMirrored)
                WatchTransaction("NULL", $"Update of activity mirrors failed.");

            var taskDeployments = from tb in tcNode.vwTaskDeployments orderby tb.CashModeCode select tb;
            foreach (var task in taskDeployments)
            {
                bool deployed = await TaskDeployment(consortium, task);
                if (!deployed)
                    WatchTransaction("NULL", $"Deployment of task {task.TaskCode} for {task.AccountCode} failed.");
            }

            var taskUpdates = from tb in tcNode.vwTaskUpdates select tb;
            foreach (var task in taskUpdates)
            {
                bool updated = await TaskUpdate(consortium, task);
                if (!updated)
                    WatchTransaction("NULL", $"Update of task {task.TaskCode} for {task.AccountCode} failed.");
            }

            var activityMirrorEvents = await activityMirrorEventHandler.GetFilterChanges(activityMirrorFilter);
            foreach (var activityMirror in activityMirrorEvents)
            {
                string accountCode = await consortium.EoaAccountCodeQueryAsync(activityMirror.Event.Eoa);
                string validationCode = await consortium.AllocationCodeQueryAsync(activityMirror.Event.Eoa, activityMirror.Event.ActivityCode);

                if (validationCode == activityMirror.Event.AllocationCode)
                {
                    bool mirrored = await TCNode.MirrorAllocation(activityMirror.Event.ActivityCode, accountCode, activityMirror.Event.AllocationCode);
                    if (mirrored)
                        WatchEvent(accountCode, $"Activity {activityMirror.Event.ActivityCode} mirrored to {activityMirror.Event.AllocationCode}");
                    else
                        WatchEvent(accountCode, $"Activity {activityMirror.Event.ActivityCode} mirror {activityMirror.Event.AllocationCode} failed db write!");
                }
                else
                    WatchEvent(accountCode, $"Activity {activityMirror.Event.ActivityCode} mirror {activityMirror.Event.AllocationCode} failed validation!");
            }

            var taskNotifyEvents = await taskNotifyEventHandler.GetFilterChanges(taskNotifyFilter);
            foreach (var taskNotification in taskNotifyEvents)
            {
                string accountCode = await consortium.EoaAccountCodeQueryAsync(taskNotification.Event.Eoa);

                bool processed = await TaskAllocation(accountCode, taskNotification.Event.MirrorContract);
                if (!processed)
                    WatchEvent(accountCode, $"Task notification failed {taskNotification.Event.MirrorContract}!");
            }

            bool cashCodesMirrored = await CashCodeMirrors(consortium);
            if (!cashCodesMirrored)
                WatchTransaction("NULL", $"Update of cash code mirrors failed.");

            var invoiceDeployments = from tb in tcNode.vwInvoiceDeployments orderby tb.InvoiceNumber select tb;
            foreach (var invoice in invoiceDeployments)
            {
                bool deployed = await InvoiceDeployment(consortium, invoice);
                if (!deployed)
                    WatchTransaction("NULL", $"Deployment of invoice {invoice.InvoiceNumber} for {invoice.AccountCode} failed.");
            }

            var invoiceUpdates = from tb in tcNode.vwInvoiceUpdates orderby tb.InvoiceNumber select tb;
            foreach (var invoice in invoiceUpdates)
            {
                bool updated = await InvoiceUpdate(consortium, invoice);
                if (!updated)
                    WatchTransaction("NULL", $"Update of invoice {invoice.InvoiceNumber} for {invoice.AccountCode} failed.");
            }

            var cashCodeMirrorEvents = await cashCodeMirrorEventHandler.GetFilterChanges(cashCodeMirrorFilter);
            foreach (var cashCodeMirror in cashCodeMirrorEvents)
            {
                string accountCode = await consortium.EoaAccountCodeQueryAsync(cashCodeMirror.Event.Eoa);
                string validationCode = await consortium.ChargeCodeQueryAsync(cashCodeMirror.Event.Eoa, cashCodeMirror.Event.CashCode);

                if (validationCode == cashCodeMirror.Event.ChargeCode)
                {
                    bool mirrored = await TCNode.MirrorCashCode(cashCodeMirror.Event.CashCode, accountCode, cashCodeMirror.Event.ChargeCode);
                    if (mirrored)
                        WatchEvent(accountCode, $"Cash Code {cashCodeMirror.Event.CashCode} mirrored to {cashCodeMirror.Event.ChargeCode}");
                    else
                        WatchEvent(accountCode, $"Cas Code {cashCodeMirror.Event.CashCode} mirror {cashCodeMirror.Event.ChargeCode} failed db write!");
                }
                else
                {
                    WatchEvent(accountCode, $"Cash Code {cashCodeMirror.Event.CashCode} mirror {cashCodeMirror.Event.ChargeCode} failed validation!");
                }
            }

            var invoiceNotifyEvents = await invoiceNotifyEventHandler.GetFilterChanges(invoiceNotifyFilter);
            foreach (var invoiceNotification in invoiceNotifyEvents)
            {
                string accountCode = await consortium.EoaAccountCodeQueryAsync(invoiceNotification.Event.Eoa);

                bool processed = await InvoiceMirror(consortium, accountCode, invoiceNotification.Event.MirrorContract);
                if (!processed)
                    WatchEvent(accountCode, $"Invoice notification failed {invoiceNotification.Event.MirrorContract}!");
            }

            Thread.Sleep(pollRate);
        }
    }
    catch (Exception err)
    {
        OnWatchCloseError?.Invoke(new EthEventArgs("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}"));
    }
}

```

## Allocations

[The tutorial](https://tradecontrol.github.io/tutorials/network) reveals how allocations are deployed to synchronise the network. The T-Sql algorithm for calculating SvD projections can be found in the [Task.vwAllocationSvD](https://github.com/TradeControl/sqlnode/blob/master/src/tcNodeDb/Task/Views/vwAllocationSvD.sql) view.

## Invoice Mirrors

[Invoice](#Invoice) mirrors are not reflections because they must mirror allocations, not orders. The [Invoice.proc_Mirror](https://github.com/TradeControl/sqlnode/blob/master/src/tcNodeDb/Invoice/Stored%20Procedures/proc_Mirror.sql) procedure applies the above allocations algorithm, but changes the polarity depending on the invoice demand. When the mirror is deployed, the owner writes the address to the target consortium. This corresponds to an act of signing off the invoice. Only the EOA of the invoice can sign, since the sender address is used by the Org.sol contract to obtain access to the invoice:

``` javascript
function InvoiceMirror(Orgs storage _self, string memory _invoiceNumber, address _invoiceContract) internal
{
    require (_self.Accounts[_self.Consortiums[msg.sender].AccountCode].IsAuthorised, "Un-authorised account");
    string memory accountCode;
    accountCode = _self.Consortiums[msg.sender].AccountCode;
    require (!_self.Invoices[accountCode][_invoiceNumber].IsMirrored, "Invoice is already mirrored");

    _self.Invoices[accountCode][_invoiceNumber].MirrorContract = _invoiceContract;
    _self.Invoices[accountCode][_invoiceNumber].IsMirrored = true;
}
```

## Licence

Trade Control Documentation by Trade Control Ltd is licenced under a [Creative Commons Attribution-ShareAlike 4.0 International License](http://creativecommons.org/licenses/by-sa/4.0/) 

![Creative Commons](https://i.creativecommons.org/l/by-sa/4.0/88x31.png)
