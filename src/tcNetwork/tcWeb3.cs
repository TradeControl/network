//#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.RPC;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Hex.HexTypes;

using Solidity.Contracts.Org;
using Solidity.Contracts.Org.ContractDefinition;
using Solidity.Contracts.Task;
using Solidity.Contracts.Task.ContractDefinition;
using Solidity.Contracts.Invoice;
using Solidity.Contracts.Invoice.ContractDefinition;

using TradeControl.Node;

namespace TradeControl.Network
{
    public class EthEventArgs : EventArgs
    {
        public string Tag { get; }
        public string Description { get; }
        public DateTime TransactedOn { get; }

        public EthEventArgs(string tag, string description)
        {
            Tag = tag;
            Description = description;
            TransactedOn = DateTime.Now;
        }
    }

    public delegate void EthEventHandler(EthEventArgs e);

    public sealed class TCWeb3 : IDisposable
    {
        #region declarations
        private static string NetworkProvider { get ; set; }
        private static string PublicKey { get; set; }
        private static string PrivateKey { get; set; }
        private static string ConsortiumAddress { get; set; }

        private const string nodeVersion = "3.27.1";

        static Web3 web3 = null;
        static Account account;
        static TCNodeNetwork tcNode;

        public event EthEventHandler OnEthEvent;
        public event EthEventHandler OnEthTransaction;
        public event EthEventHandler OnWatchEvent;
        public event EthEventHandler OnWatchTransaction;
        public event EthEventHandler OnWatchCloseError;

        static readonly List<string> _passiveModeAccounts = new List<string>();
        static readonly List<string> _activeModeAccounts = new List<string>();

        Thread passiveThread;
        List<Thread> activeThreads;
        bool threadInterrupt = true;
        const int pollRate = 10000;

        #region ethereum storage conversions
        readonly Func<long, DateTime> FromUnixEpoch = (ms) => new DateTime(1970, 1, 1).AddMilliseconds(ms);
        readonly Func<DateTime, long> ToUnixEpoch = (dt) => (long)(dt - new DateTime(1970, 1, 1)).TotalMilliseconds;

        readonly Func<decimal, short, BigInteger> ToEthDecimalStorage = (v, dp) => new BigInteger(Math.Round((double)v * Math.Pow(10, dp), 0));
        readonly Func<BigInteger, short, decimal> FromEthDecimalStorage = (v, dp) => decimal.Parse(v.ToString()) * (decimal)Math.Pow(10, dp * -1);

        const short EVM_CHARGE_DP = 5;
        const short EVM_QUANTITY_DP = 3;
        const short EVM_TAX_RATE_DP = 3;
        #endregion

        #endregion

        #region class
        public TCWeb3(string networkProvider)
        {
            NetworkProvider = networkProvider;
            web3 = new Web3(networkProvider);
        }

        public TCWeb3(string networkProvider, string publicKey)
        {
            NetworkProvider = networkProvider;
            PublicKey = publicKey;

            web3 = new Web3(networkProvider);
        }

        public TCWeb3(string networkProvider, string publicKey, string privateKey)
        {
            NetworkProvider = networkProvider;
            PublicKey = publicKey;
            PrivateKey = privateKey;

            account = new Account(PrivateKey);
            web3 = new Web3(account, networkProvider);            
        }

        public TCWeb3(string networkProvider, string publicKey, string privateKey, string consortiumAddress)
        {
            NetworkProvider = networkProvider;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            ConsortiumAddress = consortiumAddress;

            account = new Account(PrivateKey);
            web3 = new Web3(account, networkProvider);
        }

        public void Dispose()
        {
            try
            {
                StopWatching();

                TCNode?.AddNetworkProvider(NetworkProvider, PublicKey, PrivateKey, ConsortiumAddress);
            }
            catch (Exception err)
            {
                OnEthEvent?.Invoke(new EthEventArgs("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}"));
            }
        }
        #endregion

        #region tcnode
        public static SemVer NetworkNodeVersion
        {
            get
            {
                SemVer semVer = new SemVer();
                semVer.FromString(nodeVersion);
                return semVer;
            }
        }

        public TCNodeNetwork TCNode
        {
            get { return tcNode; }
            set { tcNode = value; }
        }

        #endregion

        #region accounts
        public async Task<List<string>> Accounts()
        {
            var accounts = await web3.Eth.Accounts.SendRequestAsync();
            return accounts.ToList<string>();
        }

        public async Task<string> NewAccount(string passPhrase)
        {
            var account = await web3.Personal.NewAccount.SendRequestAsync(passPhrase);
            return account;
        }
        #endregion

        #region Ether
        public async Task<string> Coinbase() 
        { 
            var coinbase = await web3.Eth.CoinBase.SendRequestAsync();
            return coinbase;
        }

        public async Task<decimal> GetAccountBalance(string eoa)
        {
            var balance = await web3.Eth.GetBalance.SendRequestAsync(eoa);
            
            return Web3.Convert.FromWei(balance.Value);
        }

        public async Task<string> TransferEther(string toAddress, decimal etherAmount)
        {
            var transaction = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(toAddress, etherAmount);
            return transaction.TransactionHash;
        }

        public async Task<string> TransferEther(string toAddress, decimal etherAmount, decimal gasPriceGwei = 21000)
        {
            var transaction = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(toAddress, etherAmount, gasPriceGwei);
            return transaction.TransactionHash;
        }

        public async Task<string> TransferEther(string toAddress, decimal etherAmount, decimal gasPriceGwei = 21000, long gas = 25000 )
        {
            var transaction = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(toAddress, etherAmount, gasPriceGwei, new BigInteger(gas));
            return transaction.TransactionHash;
        }

        #endregion

        #region consortium
        string OrgByteCode
        {
            get
            {
                byte[] source = (byte[])Properties.Resources.ResourceManager.GetObject("OrgByteCode");
                return Encoding.UTF8.GetString(source);
            }
        }

        string TaskByteCode
        {
            get
            {
                byte[] source = (byte[])Properties.Resources.ResourceManager.GetObject("TaskByteCode");
                return Encoding.UTF8.GetString(source);
            }
        }

        string InvoiceByteCode
        {
            get
            {
                byte[] source = (byte[])Properties.Resources.ResourceManager.GetObject("InvoiceByteCode");
                return Encoding.UTF8.GetString(source);
            }
        }

        public async Task<string> DeployConsortium()
        {
            
            var transactionReceipt = await OrgService.DeployContractAndWaitForReceiptAsync(web3, new OrgDeployment(OrgByteCode));
            ConsortiumAddress = transactionReceipt.ContractAddress;
            EthTransaction($"Tx {transactionReceipt.TransactionHash}", $"Consortium deployed. Gas {transactionReceipt.GasUsed} Wei");
            return transactionReceipt.ContractAddress;
        }

        public async Task<string> GetConsortium(string accountCode)
        {
            var consortium = new OrgService(web3, ConsortiumAddress);
            return await consortium.GetConsortiumQueryAsync(accountCode);
        }

        public async Task<string> GetEOA(string accountCode)
        {
            var consortium = new OrgService(web3, ConsortiumAddress);
            return await consortium.GetEOAQueryAsync(accountCode);
        }

        public async Task<bool> GetAuthorisation(string accountCode)
        {
            var consortium = new OrgService(web3, ConsortiumAddress);
            return await consortium.IsAuthorisedAccountQueryAsync(accountCode);
        }

        public async Task<string> SetAuthorisation(string accountCode, bool isAuthorised)
        {
            var consortium = new OrgService(web3, ConsortiumAddress);
            var transactionReceipt = await consortium.AuthoriseRequestAndWaitForReceiptAsync(accountCode, isAuthorised);

            EthTransaction($"Tx {transactionReceipt.TransactionHash}", $"{accountCode} authorisation is {isAuthorised}. Gas {transactionReceipt.GasUsed}  Wei");

            return transactionReceipt.TransactionHash;
        }

        public async Task<bool> AddMember(string accountCode, string eoa, string consortiumAddress)
        {
            if (tcNode == null) return false;

            var consortium = new OrgService(web3, ConsortiumAddress);

            var transactionReceipt = await consortium.NewMemberRequestAndWaitForReceiptAsync(eoa, consortiumAddress, accountCode, true);

            EthTransaction($"Tx {transactionReceipt.TransactionHash}", $"{accountCode} is new member. Gas {transactionReceipt.GasUsed} Wei");

            return await tcNode.AddMember(accountCode);
        }
        #endregion

        #region watch for events
        public void AddActiveAccount(string accountCode) { _activeModeAccounts.Add(accountCode); }

        public void AddPassiveAccount(string accountCode) { _passiveModeAccounts.Add(accountCode);  }

        private async Task<bool> IsInitialised()
        {
            try
            {
                var balance = await GetAccountBalance(PublicKey);
                if (balance == 0)
                {
                    OnEthEvent?.Invoke(new EthEventArgs("NULL", $"Start failed due to zero account balance for {PublicKey}"));
                    return false;
                }
                else if (tcNode == null)
                {
                    OnEthEvent?.Invoke(new EthEventArgs("NULL", "The database is not assigned"));
                    return false;
                }
                else if ((_activeModeAccounts.Count + _passiveModeAccounts.Count) == 0)
                {
                    OnEthEvent?.Invoke(new EthEventArgs("NULL", "The consortium is empty"));
                    return false;
                }
                else
                    return true;
            }
            catch (Exception err)
            {
                OnEthEvent?.Invoke(new EthEventArgs("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}"));
                return false;
            }
        }

        public async Task<bool> StartWatching()
        {
            try
            {
                var result = await IsInitialised();

                if (result)
                {
                    threadInterrupt = false;
                    passiveThread = new Thread(new ThreadStart(PassiveWatch)) { IsBackground = true, Priority = ThreadPriority.Normal };                    
                    passiveThread.Start();

                    activeThreads?.Clear();
                    activeThreads = new List<Thread>();

                    foreach(string accountCode in _activeModeAccounts)
                    {
                        Thread activeThread = new Thread(new ThreadStart(ActiveWatch)) { IsBackground = true, Priority = ThreadPriority.Normal, Name = accountCode };
                        activeThreads.Add(activeThread);
                        activeThread.Start();
                    }
                }
                return result;

            }
            catch (Exception err)
            {
                OnEthEvent?.Invoke(new EthEventArgs("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}"));
                return false;
            }
        }

        public void StopWatching()
        {
            threadInterrupt = true;
        }

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
                    #region tasks
                    bool activitiesMirrored = await ActivityMirrors(consortium);

                    if (!activitiesMirrored)
                    {
                        WatchTransaction("NULL", $"Update of activity mirrors failed.");
#if TEST
                        throw new Exception("Activity mirroring update error");
#endif
                    }

                    var taskDeployments = from tb in tcNode.vwTaskDeployments orderby tb.CashModeCode select tb;
                    foreach (var task in taskDeployments)
                    {
                        bool deployed = await TaskDeployment(consortium, task);
                        if (!deployed)
                        {
                            WatchTransaction("NULL", $"Deployment of task {task.TaskCode} for {task.AccountCode} failed.");
#if TEST
                            throw new Exception("Task deploy error");
#endif
                        }
                    }

                    var taskUpdates = from tb in tcNode.vwTaskUpdates select tb;
                    foreach (var task in taskUpdates)
                    {
                        bool updated = await TaskUpdate(consortium, task);
                        if (!updated)
                        { 
                            WatchTransaction("NULL", $"Update of task {task.TaskCode} for {task.AccountCode} failed.");
#if TEST
                            throw new Exception("Task update error");
#endif
                        }
                    }

                    var activityMirrorEvents = await activityMirrorEventHandler.GetFilterChangesAsync(activityMirrorFilter);
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
                        {
                            WatchEvent(accountCode, $"Activity {activityMirror.Event.ActivityCode} mirror {activityMirror.Event.AllocationCode} failed validation!");
                        }
                    }

                    var taskNotifyEvents = await taskNotifyEventHandler.GetFilterChangesAsync(taskNotifyFilter);
                    foreach (var taskNotification in taskNotifyEvents)
                    {
                        string accountCode = await consortium.EoaAccountCodeQueryAsync(taskNotification.Event.Eoa);

                        bool processed = await TaskAllocation(accountCode, taskNotification.Event.MirrorContract);
                        if (!processed)
                            WatchEvent(accountCode, $"Task notification failed {taskNotification.Event.MirrorContract}!");
                    }
                    #endregion

                    #region invoices
                    bool cashCodesMirrored = await CashCodeMirrors(consortium);

                    if (!cashCodesMirrored)
                    {
                        WatchTransaction("NULL", $"Update of cash code mirrors failed.");
#if TEST
                        throw new Exception("Cash Code mirroring update error");
#endif
                    }

                    var invoiceDeployments = from tb in tcNode.vwInvoiceDeployments orderby tb.InvoiceNumber select tb;
                    foreach (var invoice in invoiceDeployments)
                    {
                        bool deployed = await InvoiceDeployment(consortium, invoice);
                        if (!deployed)
                        {
                            WatchTransaction("NULL", $"Deployment of invoice {invoice.InvoiceNumber} for {invoice.AccountCode} failed.");
#if TEST
                            throw new Exception("Invoice deploy error");
#endif
                        }
                    }

                    var invoiceUpdates = from tb in tcNode.vwInvoiceUpdates orderby tb.InvoiceNumber select tb;
                    foreach (var invoice in invoiceUpdates)
                    {
                        bool updated = await InvoiceUpdate(consortium, invoice);
                        if (!updated)
                        {
                            WatchTransaction("NULL", $"Update of invoice {invoice.InvoiceNumber} for {invoice.AccountCode} failed.");
#if TEST
                            throw new Exception("Invoice update error");
#endif
                        }
                    }

                    var cashCodeMirrorEvents = await cashCodeMirrorEventHandler.GetFilterChangesAsync(cashCodeMirrorFilter);
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

                    var invoiceNotifyEvents = await invoiceNotifyEventHandler.GetFilterChangesAsync(invoiceNotifyFilter);
                    foreach (var invoiceNotification in invoiceNotifyEvents)
                    {
                        string accountCode = await consortium.EoaAccountCodeQueryAsync(invoiceNotification.Event.Eoa);                        

                        bool processed = await InvoiceMirror(consortium, accountCode, invoiceNotification.Event.MirrorContract);
                        if (!processed)
                            WatchEvent(accountCode, $"Invoice notification failed {invoiceNotification.Event.MirrorContract}!");
                    }
                    #endregion


                    Thread.Sleep(pollRate);
                }
            }
            catch (Exception err)
            {
                OnWatchCloseError?.Invoke(new EthEventArgs("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}"));
            }
        }

        /// <summary>
        /// Not yet implemented
        /// Purpose: to actively monitor events on the Org contracts of consortium members
        /// </summary>
        private void ActiveWatch()
        {
            /*
                active mode:

                task>>
                event OnTaskAdd (address indexed eoa, string taskCode);                
                event OnPriceChange(string taskCode);
                event OnRescheduled(string taskCode, uint quantity, uint actionOn);
                event OnDelivery(string taskCode, uint quantity, uint actionedOn);
                event OnStatusChange(string taskCode, TaskStatus status, uint actionedOn);

                invoice>>
                event OnInvoiceAdd (address indexed eoa, string invoiceNumber);
                event OnPayment (uint paymentValue, uint outstandingAmount);
                event OnStatusChange (InvoiceStatus status);
                event OnDueChange (uint nowDue);
            */
            try
            {
                string accountCode = Thread.CurrentThread.Name;

                while (!threadInterrupt)
                {
                    WatchEvent(accountCode, "Active poll event");
                    WatchEvent(accountCode, "Active poll transaction");


                    Thread.Sleep(pollRate);
                }
            }
            catch (Exception err)
            {
                OnWatchCloseError?.Invoke(new EthEventArgs("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}"));
            }
        }
        #endregion

        #region Activities
        async Task<bool> ActivityMirrors(OrgService consortium)
        {
            try
            {
                bool success = true;
                string accountCode = string.Empty;
                OrgService targetConsortium = null;

                var activities = from tb in tcNode.vwActivityMirrors orderby tb.AccountCode, tb.AllocationCode select tb;
                
                foreach (var activity in activities)
                {
                    if (accountCode != activity.AccountCode)
                    {
                        string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(activity.AccountCode);
                        targetConsortium = new OrgService(web3, targetConsortiumAddress);
                        accountCode = activity.AccountCode;
                    }

                    var mirrorReceipt = await targetConsortium.ActivityMirrorRequestAndWaitForReceiptAsync(activity.AllocationCode, activity.ActivityCode);
                    WatchTransaction($"Tx {mirrorReceipt.TransactionHash}", $"{activity.ActivityCode} mirrored to {activity.AllocationCode}. Gas {mirrorReceipt.GasUsed} Wei");

                    if (!await TCNode.AllocationTransmitted(activity.AccountCode, activity.AllocationCode))
                        success = false;
                }

                return success;
            }                       
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }
        #endregion

        #region Tasks        
        async Task<bool> TaskDeployment(OrgService consortium, vwTaskDeployment task)
        {
            try
            {
                string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(task.AccountCode);

                var deploymentReceipt = await TaskService.DeployContractAndWaitForReceiptAsync(web3, new TaskDeployment(TaskByteCode));
                WatchTransaction($"Tx {deploymentReceipt.TransactionHash}", $"Task {task.TaskCode} deployed. Gas {deploymentReceipt.GasUsed} Wei");

                var taskContract = new TaskService(web3, deploymentReceipt.ContractAddress);

                var headerReceipt = await taskContract.InitialiseRequestAndWaitForReceiptAsync(task.TaskCode, targetConsortiumAddress, (byte)task.CashModeCode, 
                        task.ActivityCode, task.ActivityDescription ?? string.Empty, task.TaskTitle ?? string.Empty,
                        (byte)task.TaskStatusCode, new BigInteger(ToUnixEpoch(task.ActionOn)), ToEthDecimalStorage(task.UnitCharge, EVM_CHARGE_DP), ToEthDecimalStorage(task.Quantity, EVM_QUANTITY_DP), 
                        ToEthDecimalStorage(task.TaxRate, EVM_TAX_RATE_DP), task.UnitOfMeasure, task.UnitOfCharge ?? string.Empty);
                WatchTransaction($"Tx {headerReceipt.TransactionHash}", $"Task {task.TaskCode} header set. Gas {headerReceipt.GasUsed} Wei");

                var registerReceipt = await consortium.TaskNewRequestAndWaitForReceiptAsync(task.TaskCode, task.AccountCode, deploymentReceipt.ContractAddress);
                WatchTransaction($"Tx {registerReceipt.TransactionHash}", $"Task {task.TaskCode} registered with consortium. Gas {registerReceipt.GasUsed} Wei");

                var targetConsortium = new OrgService(web3, targetConsortiumAddress);
                var notificationReceipt = await targetConsortium.TaskNotificationRequestAndWaitForReceiptAsync(deploymentReceipt.ContractAddress);
                WatchTransaction($"Tx {notificationReceipt.TransactionHash}", $"Account {task.AccountCode} notified of task {task.TaskCode} at {targetConsortiumAddress}. Gas {notificationReceipt.GasUsed} Wei");

                ulong totalGasUsed = deploymentReceipt.GasUsed.ToUlong() + headerReceipt.GasUsed.ToUlong() + registerReceipt.GasUsed.ToUlong() + notificationReceipt.GasUsed.ToUlong();
                WatchTransaction(deploymentReceipt.ContractAddress, $"Task {task.TaskCode} successfully deployed for {totalGasUsed} Wei");

                return await TCNode.TaskTransmitted(task.TaskCode);
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }

        }

        async Task<bool> TaskUpdate(OrgService consortium, vwTaskUpdate task)
        {
            try
            {
                ulong totalGasUsed = 0;

                string taskContractAddress = await consortium.TaskContractQueryAsync(task.TaskCode);
                var taskContract = new TaskService(web3, taskContractAddress);

                var taskSchedule = await taskContract.GetScheduleQueryAsync();

                if ((byte)task.TaskStatusCode != taskSchedule.Status)
                {
                    var statusChangeReceipt = await taskContract.SetStatusRequestAndWaitForReceiptAsync((byte)task.TaskStatusCode, ToUnixEpoch(DateTime.Now));
                    WatchTransaction($"Tx {statusChangeReceipt.TransactionHash}", $"Task {task.TaskCode} status change. Gas {statusChangeReceipt.GasUsed} Wei");
                    totalGasUsed += statusChangeReceipt.GasUsed.ToUlong();
                }

                if (task.UnitCharge != FromEthDecimalStorage(taskSchedule.UnitCharge, EVM_CHARGE_DP) || task.TaxRate != FromEthDecimalStorage(taskSchedule.TaxRate, EVM_TAX_RATE_DP))
                {
                    var priceChangeReceipt = await taskContract.PriceChangeRequestAndWaitForReceiptAsync(ToEthDecimalStorage(task.UnitCharge, EVM_CHARGE_DP), ToEthDecimalStorage(task.TaxRate, EVM_TAX_RATE_DP));
                    WatchTransaction($"Tx {priceChangeReceipt.TransactionHash}", $"Task {task.TaskCode} price/tax rate change. Gas {priceChangeReceipt.GasUsed} Wei");
                    totalGasUsed += priceChangeReceipt.GasUsed.ToUlong();
                }

                if (task.ActionOn != FromUnixEpoch((long)taskSchedule.ActionOn) || task.Quantity != FromEthDecimalStorage(taskSchedule.QuantityOrdered, EVM_QUANTITY_DP))
                {
                    var rescheduleReceipt = await taskContract.RescheduleRequestAndWaitForReceiptAsync(new BigInteger(ToUnixEpoch(task.ActionOn)), ToEthDecimalStorage(task.Quantity, EVM_QUANTITY_DP));
                    WatchTransaction($"Tx {rescheduleReceipt.TransactionHash}", $"Task {task.TaskCode} reschedule. Gas {rescheduleReceipt.GasUsed} Wei");
                    totalGasUsed += rescheduleReceipt.GasUsed.ToUlong();
                }

                string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(task.AccountCode);
                var targetConsortium = new OrgService(web3, targetConsortiumAddress);
                var notificationReceipt = await targetConsortium.TaskNotificationRequestAndWaitForReceiptAsync(taskContractAddress);
                WatchTransaction($"Tx {notificationReceipt.TransactionHash}", $"Account {task.AccountCode} notified at {targetConsortiumAddress}. Gas {notificationReceipt.GasUsed} Wei");
                totalGasUsed += notificationReceipt.GasUsed.ToUlong();

                WatchTransaction(taskContractAddress, $"Task {task.TaskCode} successfully updated for { Web3.Convert.FromWei(new BigInteger(totalGasUsed))} ETH");

                return await TCNode.TaskTransmitted(task.TaskCode);
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }

        async Task<bool> TaskAllocation(string accountCode, string allocContractAddress)
        {
            try
            {                                
                var taskContract = new TaskService(web3, allocContractAddress);
                var header = await taskContract.GetHeaderQueryAsync();
                var schedule = await taskContract.GetScheduleQueryAsync();

                tbAllocation allocation = new tbAllocation
                {
                    ContractAddress = allocContractAddress,
                    AccountCode = accountCode,
                    AllocationCode = header.ActivityCode,
                    AllocationDescription = header.Description,
                    TaskCode = header.TaskCode,
                    TaskTitle = header.Title,
                    ActionOn = FromUnixEpoch((long)schedule.ActionOn),
                    CashModeCode = header.Polarity,
                    UnitOfCharge = header.UnitOfCharge,
                    UnitOfMeasure = header.UnitOfMeasure,
                    QuantityDelivered = FromEthDecimalStorage(schedule.QuantityDelivered, EVM_QUANTITY_DP),
                    QuantityOrdered = FromEthDecimalStorage(schedule.QuantityOrdered, EVM_QUANTITY_DP),
                    TaxRate = FromEthDecimalStorage(schedule.TaxRate, EVM_TAX_RATE_DP),
                    TaskStatusCode = schedule.Status,
                    UnitCharge = (decimal)FromEthDecimalStorage(schedule.UnitCharge, EVM_CHARGE_DP),
                    InsertedOn = DateTime.Now
                };

                if (await TCNode.TaskAllocation(allocation))
                {
                    WatchEvent($"Tx {allocContractAddress}", $"Task notification {header.TaskCode} from {accountCode} allocated.");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }

        }
        #endregion

        #region CashCodes
        async Task<bool> CashCodeMirrors(OrgService consortium)
        {
            try
            {
                bool success = true;
                string accountCode = string.Empty;
                OrgService targetConsortium = null;

                var cashCodes = from tb in tcNode.vwCashCodeMirrors orderby tb.AccountCode, tb.ChargeCode select tb;

                foreach (var cashCode in cashCodes)
                {
                    if (accountCode != cashCode.AccountCode)
                    {
                        string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(cashCode.AccountCode);
                        targetConsortium = new OrgService(web3, targetConsortiumAddress);
                        accountCode = cashCode.AccountCode;
                    }

                    var mirrorReceipt = await targetConsortium.CashCodeMirrorRequestAndWaitForReceiptAsync(cashCode.ChargeCode, cashCode.CashCode);
                    WatchTransaction($"Tx {mirrorReceipt.TransactionHash}", $"{cashCode.CashCode} mirrored to {cashCode.ChargeCode}. Gas {mirrorReceipt.GasUsed} Wei");

                    if (!await TCNode.CashCodeTransmitted(cashCode.AccountCode, cashCode.ChargeCode))
                        success = false;
                }

                return success;
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }
        #endregion

        #region Invoices
        async Task<bool> InvoiceDeployment(OrgService consortium, vwInvoiceDeployment invoice)
        {
            try
            {                

                string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(invoice.AccountCode);                

                var deploymentReceipt = await InvoiceService.DeployContractAndWaitForReceiptAsync(web3, new InvoiceDeployment(InvoiceByteCode));
                WatchTransaction($"Tx {deploymentReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} deployed. Gas {deploymentReceipt.GasUsed} Wei");

                var invoiceContract = new InvoiceService(web3, deploymentReceipt.ContractAddress);
                Nethereum.RPC.Eth.DTOs.TransactionReceipt headerReceipt = await invoiceContract.SetHeaderRequestAndWaitForReceiptAsync(invoice.InvoiceNumber, invoice.ContractNumber ?? string.Empty,
                        (byte)invoice.InvoicePolarity, (byte)invoice.PaymentPolarity, (byte)invoice.InvoiceStatusCode, ToUnixEpoch(invoice.DueOn), ToUnixEpoch(invoice.InvoicedOn),
                        ToEthDecimalStorage(invoice.InvoiceValue, EVM_CHARGE_DP), ToEthDecimalStorage(invoice.TaxValue, EVM_CHARGE_DP),
                        invoice.PaymentTerms ?? string.Empty, invoice.UnitOfCharge ?? string.Empty);
                WatchTransaction($"Tx {headerReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} header set. Gas {headerReceipt.GasUsed} Wei");

                var registerReceipt = await consortium.InvoiceNewRequestAndWaitForReceiptAsync(invoice.AccountCode, invoice.InvoiceNumber, deploymentReceipt.ContractAddress);
                WatchTransaction($"Tx {registerReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} registered with consortium. Gas {registerReceipt.GasUsed} Wei");

                ulong totalGasUsed = deploymentReceipt.GasUsed.ToUlong() + headerReceipt.GasUsed.ToUlong() + registerReceipt.GasUsed.ToUlong();

                totalGasUsed += await InvoiceDeploymentDetails(consortium, invoiceContract, invoice);

                if (invoice.PaymentAddress?.Length > 0)
                {
                    var paymentAddressReceipt = await invoiceContract.SetPaymentAddressRequestAndWaitForReceiptAsync(invoice.PaymentAddress);
                    WatchTransaction($"Tx {paymentAddressReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} payment address ({invoice.PaymentAddress}). Gas {paymentAddressReceipt.GasUsed} Wei");
                    totalGasUsed += paymentAddressReceipt.GasUsed.ToUlong();
                }

                var targetConsortium = new OrgService(web3, targetConsortiumAddress);

                if (invoice.ContractNumber != null)
                {
                    var mirrorReceipt = await targetConsortium.InvoiceMirrorRequestAndWaitForReceiptAsync(invoice.ContractNumber, deploymentReceipt.ContractAddress);
                    WatchTransaction($"Tx {mirrorReceipt.TransactionHash}", $"Account {invoice.AccountCode} notified of invoice {invoice.InvoiceNumber}->{invoice.ContractNumber} mirror at {targetConsortiumAddress}. Gas {mirrorReceipt.GasUsed} Wei");
                    totalGasUsed += mirrorReceipt.GasUsed.ToUlong();
                }
                else
                {
                    var notificationReceipt = await targetConsortium.InvoiceNotificationRequestAndWaitForReceiptAsync(deploymentReceipt.ContractAddress);
                    WatchTransaction($"Tx {notificationReceipt.TransactionHash}", $"Account {invoice.AccountCode} notified of invoice {invoice.InvoiceNumber} at {targetConsortiumAddress}. Gas {notificationReceipt.GasUsed} Wei");
                    totalGasUsed += notificationReceipt.GasUsed.ToUlong();
                }


                WatchTransaction(deploymentReceipt.ContractAddress, $"Invoice contract {invoice.InvoiceNumber} successful deployed for {totalGasUsed} Wei");

                return await TCNode.InvoiceTransmitted(invoice.InvoiceNumber);
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }

        private async Task<ulong> InvoiceDeploymentDetails(OrgService consortium, InvoiceService invoiceContract, vwInvoiceDeployment invoice)
        {
            ulong totalGasUsed = 0;

            var items = from tb in TCNode.vwInvoiceDeploymentItems where tb.InvoiceNumber == invoice.InvoiceNumber select tb;

            foreach (var item in items)
            {
                var itemReceipt = await invoiceContract.AddItemRequestAndWaitForReceiptAsync(item.ChargeCode, item.ChargeDescription,
                    ToEthDecimalStorage(item.InvoiceValue, EVM_CHARGE_DP), ToEthDecimalStorage(item.TaxValue, EVM_CHARGE_DP), item.TaxCode);
                WatchTransaction($"Tx {itemReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} item {item.ChargeCode} set. Gas {itemReceipt.GasUsed} Wei");
                totalGasUsed += itemReceipt.GasUsed.ToUlong();
            }

            var tasks = from tb in TCNode.tbInvoiceTasks where tb.InvoiceNumber == invoice.InvoiceNumber select tb;

            foreach (var task in tasks)
            {
                string taskContractAddress = await consortium.TaskContractQueryAsync(task.TaskCode);
                var taskReceipt = await invoiceContract.AddTaskRequestAndWaitForReceiptAsync(taskContractAddress,
                        ToEthDecimalStorage(task.Quantity, EVM_QUANTITY_DP), ToEthDecimalStorage(task.InvoiceValue, EVM_CHARGE_DP),
                        ToEthDecimalStorage(task.TaxValue, EVM_CHARGE_DP), task.TaxCode);
                WatchTransaction($"Tx {taskReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} task {task.TaskCode} set. Gas {taskReceipt.GasUsed} Wei");
                totalGasUsed += taskReceipt.GasUsed.ToUlong();
            }

            return totalGasUsed;            
        }

        async Task<bool> InvoiceUpdate(OrgService consortium, vwInvoiceUpdate invoice)
        {
            try
            {
                ulong totalGasUsed = 0;

                var invoiceContractAddress = await consortium.InvoiceOwnerContractQueryAsync(invoice.AccountCode, invoice.InvoiceNumber);
                var invoiceContract = new InvoiceService(web3, invoiceContractAddress);
                var invoiceParams = await invoiceContract.GetParamsQueryAsync();

                if (invoice.DueOn != FromUnixEpoch((long)invoiceParams.DueOn))
                {
                    var scheduleReceipt = await invoiceContract.RescheduleRequestAndWaitForReceiptAsync(ToUnixEpoch(invoice.DueOn));
                    WatchTransaction($"Tx {scheduleReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} rescheduled to {invoice.DueOn}. Gas {scheduleReceipt.GasUsed} Wei");
                    totalGasUsed += scheduleReceipt.GasUsed.ToUlong();
                }

                decimal contractPaidValue = FromEthDecimalStorage(invoiceParams.PaidValue, EVM_CHARGE_DP);
                decimal contractPaidTaxValue = FromEthDecimalStorage(invoiceParams.PaidTaxValue, EVM_CHARGE_DP);

                if ((invoice.PaidTaxValue + invoice.PaidValue) != (contractPaidValue + contractPaidTaxValue))
                {
                    var paymentReceipt = await invoiceContract.PaymentRequestAndWaitForReceiptAsync((byte)invoice.InvoiceStatusCode,
                            ToEthDecimalStorage(invoice.PaidValue, EVM_CHARGE_DP), ToEthDecimalStorage(invoice.PaidTaxValue, EVM_CHARGE_DP));
                    WatchTransaction($"Tx {paymentReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} payment of {invoice.PaidTaxValue + invoice.PaidValue}. Gas {paymentReceipt.GasUsed} Wei");
                    totalGasUsed += paymentReceipt.GasUsed.ToUlong();
                }
                else if (invoice.InvoiceStatusCode != invoiceParams.Status)
                {
                    var statusReceipt = await invoiceContract.StatusChangeRequestAndWaitForReceiptAsync((byte)invoice.InvoiceStatusCode);
                    WatchTransaction($"Tx {statusReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} status update ({invoice.InvoiceStatusCode}). Gas {statusReceipt.GasUsed} Wei");
                    totalGasUsed += statusReceipt.GasUsed.ToUlong();
                }

                if (invoice.PaymentAddress != null && invoice.PaymentAddress?.ToString() != invoiceParams.PaymentAddress?.ToString())
                {
                    if (invoice.PaymentAddress.Length > 0)
                    {
                        var paymentAddressReceipt = await invoiceContract.SetPaymentAddressRequestAndWaitForReceiptAsync(invoice.PaymentAddress);
                        WatchTransaction($"Tx {paymentAddressReceipt.TransactionHash}", $"Invoice {invoice.InvoiceNumber} payment address ({invoice.PaymentAddress}). Gas {paymentAddressReceipt.GasUsed} Wei");
                        totalGasUsed += paymentAddressReceipt.GasUsed.ToUlong();
                    }
                }

                string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(invoice.AccountCode);
                var targetConsortium = new OrgService(web3, targetConsortiumAddress);
                var notificationReceipt = await targetConsortium.InvoiceNotificationRequestAndWaitForReceiptAsync(invoiceContractAddress);
                WatchTransaction($"Tx {notificationReceipt.TransactionHash}", $"Account {invoice.AccountCode} notified of invoice {invoice.InvoiceNumber} update at {targetConsortiumAddress}. Gas {notificationReceipt.GasUsed} Wei");
                totalGasUsed += notificationReceipt.GasUsed.ToUlong();

                WatchTransaction(invoiceContractAddress, $"Invoice contract {invoice.InvoiceNumber} successful updated for {totalGasUsed} Wei");

                return await TCNode.InvoiceTransmitted(invoice.InvoiceNumber);
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }

        async Task<bool> InvoiceMirror(OrgService consortium, string accountCode, string invoiceContractAddress)
        {
            try
            {
                var invoiceContract = new InvoiceService(web3, invoiceContractAddress);
                var invoiceHeader = await invoiceContract.GetHeaderQueryAsync();
                var invoiceParams = await invoiceContract.GetParamsQueryAsync();

                if (invoiceHeader.MirrorNumber?.Length > 0)
                {
                    if (TCNode.tbInvoices.Where(i => i.InvoiceNumber == invoiceHeader.MirrorNumber).Any())
                    {
                        string targetConsortiumAddress = await consortium.GetConsortiumQueryAsync(accountCode);
                        var targetConsortium = new OrgService(web3, targetConsortiumAddress);

                        if (await consortium.InvoiceIsMirroredQueryAsync(accountCode, invoiceHeader.MirrorNumber))
                        {
                            string ownerContract = await consortium.InvoiceOwnerContractQueryAsync(accountCode, invoiceHeader.MirrorNumber);
                            if (!await targetConsortium.InvoiceIsMirroredQueryAsync(await targetConsortium.EoaAccountCodeQueryAsync(PublicKey), invoiceHeader.InvoiceNumber))
                            {
                                var mirrorReceipt = await targetConsortium.InvoiceMirrorRequestAndWaitForReceiptAsync(invoiceHeader.InvoiceNumber, ownerContract);
                                WatchTransaction($"Tx {mirrorReceipt.TransactionHash}", $"Account {accountCode} notified of invoice {invoiceHeader.MirrorNumber}->{invoiceHeader.InvoiceNumber} mirror at {targetConsortiumAddress}. Gas {mirrorReceipt.GasUsed} Wei");
                            }
                        }
                    }
                }

                bool hasDetails = TCNode.tbInvoiceMirrors.Where(m => m.ContractAddress == invoiceContractAddress).Any();

                tbInvoiceMirror mirror = new tbInvoiceMirror
                {
                    AccountCode = accountCode,
                    ContractAddress = invoiceContractAddress,
                    InvoiceNumber = invoiceHeader.InvoiceNumber,
                    DueOn = FromUnixEpoch((long)invoiceParams.DueOn),
                    InvoicedOn = FromUnixEpoch((long)invoiceParams.InvoicedOn),
                    InvoiceStatusCode = invoiceParams.Status,
                    InvoiceTypeCode = (short)TCNode.GetInvoiceType((CashMode)invoiceHeader.InvoicePolarity, (CashMode)invoiceHeader.PaymentPolarity),
                    InvoiceValue = FromEthDecimalStorage(invoiceParams.InvoiceValue, EVM_CHARGE_DP),
                    InvoiceTax = FromEthDecimalStorage(invoiceParams.TaxValue, EVM_CHARGE_DP),
                    PaidValue = FromEthDecimalStorage(invoiceParams.PaidValue, EVM_CHARGE_DP),
                    PaidTaxValue = FromEthDecimalStorage(invoiceParams.PaidTaxValue, EVM_CHARGE_DP),
                    PaymentAddress = invoiceParams.PaymentAddress,
                    PaymentTerms = invoiceHeader.PaymentTerms,
                    UnitOfCharge = invoiceHeader.UnitOfCharge,
                    InsertedOn = DateTime.Now
                };

                if (await TCNode.InvoiceMirror(mirror))
                {
                    WatchEvent($"Tx {invoiceContractAddress}", $"Invoice notification {mirror.InvoiceNumber} from {accountCode} mirrored.");

                    if (!hasDetails)
                    {
                        if (TCNode.tbInvoices.Where(i => i.InvoiceNumber == invoiceHeader.MirrorNumber).Any())
                        {
                            if (!await TCNode.InvoiceMirrorReference(invoiceContractAddress, invoiceHeader.MirrorNumber))
                                WatchEvent($"Tx {invoiceContractAddress}", $"Invoice notification {mirror.InvoiceNumber} from {accountCode} mapping to {invoiceHeader.MirrorNumber} failed.");
                        }

                        hasDetails = await InvoiceMirrorDetails(invoiceContract);
                    }

                    TCNode.SubmitChanges();

                    return hasDetails;
                }
                else
                    return false;
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }

        async Task<bool> InvoiceMirrorDetails(InvoiceService invoiceContract)
        {
            try
            {
                bool result = true;

                BigInteger taskCount = await invoiceContract.GetTaskCountQueryAsync();
                for (int taskIndex = 0; taskIndex < (int)taskCount; taskIndex++)
                {
                    var task = await invoiceContract.GetTaskQueryAsync(taskIndex);
                    var taskContract = new TaskService(web3, task.ContractAddress);
                    var taskHeader = await taskContract.GetHeaderQueryAsync();

                    tbMirrorTask mirrorTask = new tbMirrorTask
                    {
                        ContractAddress = invoiceContract.ContractHandler.ContractAddress,
                        TaskCode = taskHeader.TaskCode,
                        Quantity = FromEthDecimalStorage(task.Quantity, EVM_QUANTITY_DP),
                        InvoiceValue = FromEthDecimalStorage(task.InvoiceValue, EVM_CHARGE_DP),
                        TaxCode = task.TaxCode
                    };

                    if (!await TCNode.InvoiceMirrorTask(mirrorTask))
                        result = false;

                };

                BigInteger itemCount = await invoiceContract.GetItemCountQueryAsync();
                for (int itemIndex = 0; itemIndex < (int)itemCount; itemIndex++)
                {
                    var item = await invoiceContract.GetItemQueryAsync(itemIndex);

                    tbMirrorItem mirrorItem = new tbMirrorItem
                    {
                        ContractAddress = invoiceContract.ContractHandler.ContractAddress,
                        ChargeCode = item.ChargeCode,
                        ChargeDescription = item.ChargeDescription,
                        InvoiceValue = FromEthDecimalStorage(item.InvoiceValue, EVM_CHARGE_DP),
                        TaxValue = FromEthDecimalStorage(item.TaxValue, EVM_CHARGE_DP),
                        TaxCode = item.TaxCode
                    };

                    if (!await TCNode.InvoiceMirrorItem(mirrorItem))
                        result = false;
                };

                return result;
            }
            catch (Exception err)
            {
                WatchEvent("ERR", $"{MethodInfo.GetCurrentMethod().Name}: {err.Source} - {err.Message}");
                return false;
            }
        }

        #endregion

        #region log
        void EthEvent(string tag, string description)
        {
            OnEthEvent?.Invoke(new EthEventArgs(tag, description));
        }

        void EthTransaction(string tag, string description)
        {
            OnEthTransaction?.Invoke(new EthEventArgs(tag, description));
        }

        void WatchEvent(string tag, string description)
        {
            OnWatchEvent?.Invoke(new EthEventArgs(tag, description));
        }

        void WatchTransaction(string tag, string description)
        {
            OnWatchTransaction?.Invoke(new EthEventArgs(tag, description));
        }

        #endregion


    }
}
