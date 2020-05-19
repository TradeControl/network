using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Solidity.Contracts.Org.ContractDefinition;

namespace Solidity.Contracts.Org
{
    public partial class OrgService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, OrgDeployment orgDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<OrgDeployment>().SendRequestAndWaitForReceiptAsync(orgDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, OrgDeployment orgDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<OrgDeployment>().SendRequestAsync(orgDeployment);
        }

        public static async Task<OrgService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, OrgDeployment orgDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, orgDeployment, cancellationTokenSource);
            return new OrgService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public OrgService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> ActivityMirrorRequestAsync(ActivityMirrorFunction activityMirrorFunction)
        {
             return ContractHandler.SendRequestAsync(activityMirrorFunction);
        }

        public Task<TransactionReceipt> ActivityMirrorRequestAndWaitForReceiptAsync(ActivityMirrorFunction activityMirrorFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(activityMirrorFunction, cancellationToken);
        }

        public Task<string> ActivityMirrorRequestAsync(string activityCode, string allocationCode)
        {
            var activityMirrorFunction = new ActivityMirrorFunction();
                activityMirrorFunction.ActivityCode = activityCode;
                activityMirrorFunction.AllocationCode = allocationCode;
            
             return ContractHandler.SendRequestAsync(activityMirrorFunction);
        }

        public Task<TransactionReceipt> ActivityMirrorRequestAndWaitForReceiptAsync(string activityCode, string allocationCode, CancellationTokenSource cancellationToken = null)
        {
            var activityMirrorFunction = new ActivityMirrorFunction();
                activityMirrorFunction.ActivityCode = activityCode;
                activityMirrorFunction.AllocationCode = allocationCode;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(activityMirrorFunction, cancellationToken);
        }

        public Task<string> AllocationCodeQueryAsync(AllocationCodeFunction allocationCodeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AllocationCodeFunction, string>(allocationCodeFunction, blockParameter);
        }

        
        public Task<string> AllocationCodeQueryAsync(string eoa, string activityCode, BlockParameter blockParameter = null)
        {
            var allocationCodeFunction = new AllocationCodeFunction();
                allocationCodeFunction.Eoa = eoa;
                allocationCodeFunction.ActivityCode = activityCode;
            
            return ContractHandler.QueryAsync<AllocationCodeFunction, string>(allocationCodeFunction, blockParameter);
        }

        public Task<string> AuthoriseRequestAsync(AuthoriseFunction authoriseFunction)
        {
             return ContractHandler.SendRequestAsync(authoriseFunction);
        }

        public Task<TransactionReceipt> AuthoriseRequestAndWaitForReceiptAsync(AuthoriseFunction authoriseFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(authoriseFunction, cancellationToken);
        }

        public Task<string> AuthoriseRequestAsync(string accountCode, bool isAuthorised)
        {
            var authoriseFunction = new AuthoriseFunction();
                authoriseFunction.AccountCode = accountCode;
                authoriseFunction.IsAuthorised = isAuthorised;
            
             return ContractHandler.SendRequestAsync(authoriseFunction);
        }

        public Task<TransactionReceipt> AuthoriseRequestAndWaitForReceiptAsync(string accountCode, bool isAuthorised, CancellationTokenSource cancellationToken = null)
        {
            var authoriseFunction = new AuthoriseFunction();
                authoriseFunction.AccountCode = accountCode;
                authoriseFunction.IsAuthorised = isAuthorised;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(authoriseFunction, cancellationToken);
        }

        public Task<string> CashCodeMirrorRequestAsync(CashCodeMirrorFunction cashCodeMirrorFunction)
        {
             return ContractHandler.SendRequestAsync(cashCodeMirrorFunction);
        }

        public Task<TransactionReceipt> CashCodeMirrorRequestAndWaitForReceiptAsync(CashCodeMirrorFunction cashCodeMirrorFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(cashCodeMirrorFunction, cancellationToken);
        }

        public Task<string> CashCodeMirrorRequestAsync(string cashCode, string chargeCode)
        {
            var cashCodeMirrorFunction = new CashCodeMirrorFunction();
                cashCodeMirrorFunction.CashCode = cashCode;
                cashCodeMirrorFunction.ChargeCode = chargeCode;
            
             return ContractHandler.SendRequestAsync(cashCodeMirrorFunction);
        }

        public Task<TransactionReceipt> CashCodeMirrorRequestAndWaitForReceiptAsync(string cashCode, string chargeCode, CancellationTokenSource cancellationToken = null)
        {
            var cashCodeMirrorFunction = new CashCodeMirrorFunction();
                cashCodeMirrorFunction.CashCode = cashCode;
                cashCodeMirrorFunction.ChargeCode = chargeCode;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(cashCodeMirrorFunction, cancellationToken);
        }

        public Task<string> ChargeCodeQueryAsync(ChargeCodeFunction chargeCodeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ChargeCodeFunction, string>(chargeCodeFunction, blockParameter);
        }

        
        public Task<string> ChargeCodeQueryAsync(string eoa, string cashCode, BlockParameter blockParameter = null)
        {
            var chargeCodeFunction = new ChargeCodeFunction();
                chargeCodeFunction.Eoa = eoa;
                chargeCodeFunction.CashCode = cashCode;
            
            return ContractHandler.QueryAsync<ChargeCodeFunction, string>(chargeCodeFunction, blockParameter);
        }

        public Task<string> EoaAccountCodeQueryAsync(EoaAccountCodeFunction eoaAccountCodeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EoaAccountCodeFunction, string>(eoaAccountCodeFunction, blockParameter);
        }

        
        public Task<string> EoaAccountCodeQueryAsync(string eoa, BlockParameter blockParameter = null)
        {
            var eoaAccountCodeFunction = new EoaAccountCodeFunction();
                eoaAccountCodeFunction.Eoa = eoa;
            
            return ContractHandler.QueryAsync<EoaAccountCodeFunction, string>(eoaAccountCodeFunction, blockParameter);
        }

        public Task<string> GetConsortiumQueryAsync(GetConsortiumFunction getConsortiumFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetConsortiumFunction, string>(getConsortiumFunction, blockParameter);
        }

        
        public Task<string> GetConsortiumQueryAsync(string accountCode, BlockParameter blockParameter = null)
        {
            var getConsortiumFunction = new GetConsortiumFunction();
                getConsortiumFunction.AccountCode = accountCode;
            
            return ContractHandler.QueryAsync<GetConsortiumFunction, string>(getConsortiumFunction, blockParameter);
        }

        public Task<string> GetEOAQueryAsync(GetEOAFunction getEOAFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetEOAFunction, string>(getEOAFunction, blockParameter);
        }

        
        public Task<string> GetEOAQueryAsync(string accountCode, BlockParameter blockParameter = null)
        {
            var getEOAFunction = new GetEOAFunction();
                getEOAFunction.AccountCode = accountCode;
            
            return ContractHandler.QueryAsync<GetEOAFunction, string>(getEOAFunction, blockParameter);
        }

        public Task<bool> InvoiceIsMirroredQueryAsync(InvoiceIsMirroredFunction invoiceIsMirroredFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<InvoiceIsMirroredFunction, bool>(invoiceIsMirroredFunction, blockParameter);
        }

        
        public Task<bool> InvoiceIsMirroredQueryAsync(string accountCode, string invoiceNumber, BlockParameter blockParameter = null)
        {
            var invoiceIsMirroredFunction = new InvoiceIsMirroredFunction();
                invoiceIsMirroredFunction.AccountCode = accountCode;
                invoiceIsMirroredFunction.InvoiceNumber = invoiceNumber;
            
            return ContractHandler.QueryAsync<InvoiceIsMirroredFunction, bool>(invoiceIsMirroredFunction, blockParameter);
        }

        public Task<string> InvoiceMirrorRequestAsync(InvoiceMirrorFunction invoiceMirrorFunction)
        {
             return ContractHandler.SendRequestAsync(invoiceMirrorFunction);
        }

        public Task<TransactionReceipt> InvoiceMirrorRequestAndWaitForReceiptAsync(InvoiceMirrorFunction invoiceMirrorFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(invoiceMirrorFunction, cancellationToken);
        }

        public Task<string> InvoiceMirrorRequestAsync(string invoiceNumber, string mirrorContract)
        {
            var invoiceMirrorFunction = new InvoiceMirrorFunction();
                invoiceMirrorFunction.InvoiceNumber = invoiceNumber;
                invoiceMirrorFunction.MirrorContract = mirrorContract;
            
             return ContractHandler.SendRequestAsync(invoiceMirrorFunction);
        }

        public Task<TransactionReceipt> InvoiceMirrorRequestAndWaitForReceiptAsync(string invoiceNumber, string mirrorContract, CancellationTokenSource cancellationToken = null)
        {
            var invoiceMirrorFunction = new InvoiceMirrorFunction();
                invoiceMirrorFunction.InvoiceNumber = invoiceNumber;
                invoiceMirrorFunction.MirrorContract = mirrorContract;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(invoiceMirrorFunction, cancellationToken);
        }

        public Task<string> InvoiceMirrorContractQueryAsync(InvoiceMirrorContractFunction invoiceMirrorContractFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<InvoiceMirrorContractFunction, string>(invoiceMirrorContractFunction, blockParameter);
        }

        
        public Task<string> InvoiceMirrorContractQueryAsync(string accountCode, string invoiceNumber, BlockParameter blockParameter = null)
        {
            var invoiceMirrorContractFunction = new InvoiceMirrorContractFunction();
                invoiceMirrorContractFunction.AccountCode = accountCode;
                invoiceMirrorContractFunction.InvoiceNumber = invoiceNumber;
            
            return ContractHandler.QueryAsync<InvoiceMirrorContractFunction, string>(invoiceMirrorContractFunction, blockParameter);
        }

        public Task<string> InvoiceNewRequestAsync(InvoiceNewFunction invoiceNewFunction)
        {
             return ContractHandler.SendRequestAsync(invoiceNewFunction);
        }

        public Task<TransactionReceipt> InvoiceNewRequestAndWaitForReceiptAsync(InvoiceNewFunction invoiceNewFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(invoiceNewFunction, cancellationToken);
        }

        public Task<string> InvoiceNewRequestAsync(string accountCode, string invoiceNumber, string invoiceContract)
        {
            var invoiceNewFunction = new InvoiceNewFunction();
                invoiceNewFunction.AccountCode = accountCode;
                invoiceNewFunction.InvoiceNumber = invoiceNumber;
                invoiceNewFunction.InvoiceContract = invoiceContract;
            
             return ContractHandler.SendRequestAsync(invoiceNewFunction);
        }

        public Task<TransactionReceipt> InvoiceNewRequestAndWaitForReceiptAsync(string accountCode, string invoiceNumber, string invoiceContract, CancellationTokenSource cancellationToken = null)
        {
            var invoiceNewFunction = new InvoiceNewFunction();
                invoiceNewFunction.AccountCode = accountCode;
                invoiceNewFunction.InvoiceNumber = invoiceNumber;
                invoiceNewFunction.InvoiceContract = invoiceContract;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(invoiceNewFunction, cancellationToken);
        }

        public Task<string> InvoiceNotificationRequestAsync(InvoiceNotificationFunction invoiceNotificationFunction)
        {
             return ContractHandler.SendRequestAsync(invoiceNotificationFunction);
        }

        public Task<TransactionReceipt> InvoiceNotificationRequestAndWaitForReceiptAsync(InvoiceNotificationFunction invoiceNotificationFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(invoiceNotificationFunction, cancellationToken);
        }

        public Task<string> InvoiceNotificationRequestAsync(string mirrorContract)
        {
            var invoiceNotificationFunction = new InvoiceNotificationFunction();
                invoiceNotificationFunction.MirrorContract = mirrorContract;
            
             return ContractHandler.SendRequestAsync(invoiceNotificationFunction);
        }

        public Task<TransactionReceipt> InvoiceNotificationRequestAndWaitForReceiptAsync(string mirrorContract, CancellationTokenSource cancellationToken = null)
        {
            var invoiceNotificationFunction = new InvoiceNotificationFunction();
                invoiceNotificationFunction.MirrorContract = mirrorContract;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(invoiceNotificationFunction, cancellationToken);
        }

        public Task<string> InvoiceOwnerContractQueryAsync(InvoiceOwnerContractFunction invoiceOwnerContractFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<InvoiceOwnerContractFunction, string>(invoiceOwnerContractFunction, blockParameter);
        }

        
        public Task<string> InvoiceOwnerContractQueryAsync(string accountCode, string invoiceNumber, BlockParameter blockParameter = null)
        {
            var invoiceOwnerContractFunction = new InvoiceOwnerContractFunction();
                invoiceOwnerContractFunction.AccountCode = accountCode;
                invoiceOwnerContractFunction.InvoiceNumber = invoiceNumber;
            
            return ContractHandler.QueryAsync<InvoiceOwnerContractFunction, string>(invoiceOwnerContractFunction, blockParameter);
        }

        public Task<bool> IsAuthorisedQueryAsync(IsAuthorisedFunction isAuthorisedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsAuthorisedFunction, bool>(isAuthorisedFunction, blockParameter);
        }

        
        public Task<bool> IsAuthorisedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsAuthorisedFunction, bool>(null, blockParameter);
        }

        public Task<bool> IsAuthorisedAccountQueryAsync(IsAuthorisedAccountFunction isAuthorisedAccountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsAuthorisedAccountFunction, bool>(isAuthorisedAccountFunction, blockParameter);
        }

        
        public Task<bool> IsAuthorisedAccountQueryAsync(string accountCode, BlockParameter blockParameter = null)
        {
            var isAuthorisedAccountFunction = new IsAuthorisedAccountFunction();
                isAuthorisedAccountFunction.AccountCode = accountCode;
            
            return ContractHandler.QueryAsync<IsAuthorisedAccountFunction, bool>(isAuthorisedAccountFunction, blockParameter);
        }

        public Task<string> NewMemberRequestAsync(NewMemberFunction newMemberFunction)
        {
             return ContractHandler.SendRequestAsync(newMemberFunction);
        }

        public Task<TransactionReceipt> NewMemberRequestAndWaitForReceiptAsync(NewMemberFunction newMemberFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(newMemberFunction, cancellationToken);
        }

        public Task<string> NewMemberRequestAsync(string eoa, string orgContract, string accountCode, bool isAuthorised)
        {
            var newMemberFunction = new NewMemberFunction();
                newMemberFunction.Eoa = eoa;
                newMemberFunction.OrgContract = orgContract;
                newMemberFunction.AccountCode = accountCode;
                newMemberFunction.IsAuthorised = isAuthorised;
            
             return ContractHandler.SendRequestAsync(newMemberFunction);
        }

        public Task<TransactionReceipt> NewMemberRequestAndWaitForReceiptAsync(string eoa, string orgContract, string accountCode, bool isAuthorised, CancellationTokenSource cancellationToken = null)
        {
            var newMemberFunction = new NewMemberFunction();
                newMemberFunction.Eoa = eoa;
                newMemberFunction.OrgContract = orgContract;
                newMemberFunction.AccountCode = accountCode;
                newMemberFunction.IsAuthorised = isAuthorised;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(newMemberFunction, cancellationToken);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> SenderAccountCodeQueryAsync(SenderAccountCodeFunction senderAccountCodeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SenderAccountCodeFunction, string>(senderAccountCodeFunction, blockParameter);
        }

        
        public Task<string> SenderAccountCodeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SenderAccountCodeFunction, string>(null, blockParameter);
        }

        public Task<string> TaskContractQueryAsync(TaskContractFunction taskContractFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TaskContractFunction, string>(taskContractFunction, blockParameter);
        }

        
        public Task<string> TaskContractQueryAsync(string taskCode, BlockParameter blockParameter = null)
        {
            var taskContractFunction = new TaskContractFunction();
                taskContractFunction.TaskCode = taskCode;
            
            return ContractHandler.QueryAsync<TaskContractFunction, string>(taskContractFunction, blockParameter);
        }

        public Task<string> TaskNewRequestAsync(TaskNewFunction taskNewFunction)
        {
             return ContractHandler.SendRequestAsync(taskNewFunction);
        }

        public Task<TransactionReceipt> TaskNewRequestAndWaitForReceiptAsync(TaskNewFunction taskNewFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(taskNewFunction, cancellationToken);
        }

        public Task<string> TaskNewRequestAsync(string taskCode, string accountCode, string taskContract)
        {
            var taskNewFunction = new TaskNewFunction();
                taskNewFunction.TaskCode = taskCode;
                taskNewFunction.AccountCode = accountCode;
                taskNewFunction.TaskContract = taskContract;
            
             return ContractHandler.SendRequestAsync(taskNewFunction);
        }

        public Task<TransactionReceipt> TaskNewRequestAndWaitForReceiptAsync(string taskCode, string accountCode, string taskContract, CancellationTokenSource cancellationToken = null)
        {
            var taskNewFunction = new TaskNewFunction();
                taskNewFunction.TaskCode = taskCode;
                taskNewFunction.AccountCode = accountCode;
                taskNewFunction.TaskContract = taskContract;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(taskNewFunction, cancellationToken);
        }

        public Task<string> TaskNotificationRequestAsync(TaskNotificationFunction taskNotificationFunction)
        {
             return ContractHandler.SendRequestAsync(taskNotificationFunction);
        }

        public Task<TransactionReceipt> TaskNotificationRequestAndWaitForReceiptAsync(TaskNotificationFunction taskNotificationFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(taskNotificationFunction, cancellationToken);
        }

        public Task<string> TaskNotificationRequestAsync(string mirrorContract)
        {
            var taskNotificationFunction = new TaskNotificationFunction();
                taskNotificationFunction.MirrorContract = mirrorContract;
            
             return ContractHandler.SendRequestAsync(taskNotificationFunction);
        }

        public Task<TransactionReceipt> TaskNotificationRequestAndWaitForReceiptAsync(string mirrorContract, CancellationTokenSource cancellationToken = null)
        {
            var taskNotificationFunction = new TaskNotificationFunction();
                taskNotificationFunction.MirrorContract = mirrorContract;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(taskNotificationFunction, cancellationToken);
        }

        public Task<string> DestroyRequestAsync(DestroyFunction destroyFunction)
        {
             return ContractHandler.SendRequestAsync(destroyFunction);
        }

        public Task<string> DestroyRequestAsync()
        {
             return ContractHandler.SendRequestAsync<DestroyFunction>();
        }

        public Task<TransactionReceipt> DestroyRequestAndWaitForReceiptAsync(DestroyFunction destroyFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(destroyFunction, cancellationToken);
        }

        public Task<TransactionReceipt> DestroyRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<DestroyFunction>(null, cancellationToken);
        }
    }
}
