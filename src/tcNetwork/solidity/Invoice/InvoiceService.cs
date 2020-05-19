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
using Solidity.Contracts.Invoice.ContractDefinition;

namespace Solidity.Contracts.Invoice
{
    public partial class InvoiceService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, InvoiceDeployment invoiceDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<InvoiceDeployment>().SendRequestAndWaitForReceiptAsync(invoiceDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, InvoiceDeployment invoiceDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<InvoiceDeployment>().SendRequestAsync(invoiceDeployment);
        }

        public static async Task<InvoiceService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, InvoiceDeployment invoiceDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, invoiceDeployment, cancellationTokenSource);
            return new InvoiceService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public InvoiceService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> AddItemRequestAsync(AddItemFunction addItemFunction)
        {
             return ContractHandler.SendRequestAsync(addItemFunction);
        }

        public Task<TransactionReceipt> AddItemRequestAndWaitForReceiptAsync(AddItemFunction addItemFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addItemFunction, cancellationToken);
        }

        public Task<string> AddItemRequestAsync(string chargeCode, string chargeDescription, BigInteger invoiceValue, BigInteger taxValue, string taxCode)
        {
            var addItemFunction = new AddItemFunction();
                addItemFunction.ChargeCode = chargeCode;
                addItemFunction.ChargeDescription = chargeDescription;
                addItemFunction.InvoiceValue = invoiceValue;
                addItemFunction.TaxValue = taxValue;
                addItemFunction.TaxCode = taxCode;
            
             return ContractHandler.SendRequestAsync(addItemFunction);
        }

        public Task<TransactionReceipt> AddItemRequestAndWaitForReceiptAsync(string chargeCode, string chargeDescription, BigInteger invoiceValue, BigInteger taxValue, string taxCode, CancellationTokenSource cancellationToken = null)
        {
            var addItemFunction = new AddItemFunction();
                addItemFunction.ChargeCode = chargeCode;
                addItemFunction.ChargeDescription = chargeDescription;
                addItemFunction.InvoiceValue = invoiceValue;
                addItemFunction.TaxValue = taxValue;
                addItemFunction.TaxCode = taxCode;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addItemFunction, cancellationToken);
        }

        public Task<string> AddTaskRequestAsync(AddTaskFunction addTaskFunction)
        {
             return ContractHandler.SendRequestAsync(addTaskFunction);
        }

        public Task<TransactionReceipt> AddTaskRequestAndWaitForReceiptAsync(AddTaskFunction addTaskFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addTaskFunction, cancellationToken);
        }

        public Task<string> AddTaskRequestAsync(string contractAddress, BigInteger quantity, BigInteger invoiceValue, BigInteger taxValue, string taxCode)
        {
            var addTaskFunction = new AddTaskFunction();
                addTaskFunction.ContractAddress = contractAddress;
                addTaskFunction.Quantity = quantity;
                addTaskFunction.InvoiceValue = invoiceValue;
                addTaskFunction.TaxValue = taxValue;
                addTaskFunction.TaxCode = taxCode;
            
             return ContractHandler.SendRequestAsync(addTaskFunction);
        }

        public Task<TransactionReceipt> AddTaskRequestAndWaitForReceiptAsync(string contractAddress, BigInteger quantity, BigInteger invoiceValue, BigInteger taxValue, string taxCode, CancellationTokenSource cancellationToken = null)
        {
            var addTaskFunction = new AddTaskFunction();
                addTaskFunction.ContractAddress = contractAddress;
                addTaskFunction.Quantity = quantity;
                addTaskFunction.InvoiceValue = invoiceValue;
                addTaskFunction.TaxValue = taxValue;
                addTaskFunction.TaxCode = taxCode;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addTaskFunction, cancellationToken);
        }

        public Task<GetHeaderOutputDTO> GetHeaderQueryAsync(GetHeaderFunction getHeaderFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHeaderFunction, GetHeaderOutputDTO>(getHeaderFunction, blockParameter);
        }

        public Task<GetHeaderOutputDTO> GetHeaderQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHeaderFunction, GetHeaderOutputDTO>(null, blockParameter);
        }

        public Task<GetItemOutputDTO> GetItemQueryAsync(GetItemFunction getItemFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetItemFunction, GetItemOutputDTO>(getItemFunction, blockParameter);
        }

        public Task<GetItemOutputDTO> GetItemQueryAsync(BigInteger itemIndex, BlockParameter blockParameter = null)
        {
            var getItemFunction = new GetItemFunction();
                getItemFunction.ItemIndex = itemIndex;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetItemFunction, GetItemOutputDTO>(getItemFunction, blockParameter);
        }

        public Task<BigInteger> GetItemCountQueryAsync(GetItemCountFunction getItemCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetItemCountFunction, BigInteger>(getItemCountFunction, blockParameter);
        }

        
        public Task<BigInteger> GetItemCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetItemCountFunction, BigInteger>(null, blockParameter);
        }

        public Task<GetParamsOutputDTO> GetParamsQueryAsync(GetParamsFunction getParamsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetParamsFunction, GetParamsOutputDTO>(getParamsFunction, blockParameter);
        }

        public Task<GetParamsOutputDTO> GetParamsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetParamsFunction, GetParamsOutputDTO>(null, blockParameter);
        }

        public Task<GetTaskOutputDTO> GetTaskQueryAsync(GetTaskFunction getTaskFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetTaskFunction, GetTaskOutputDTO>(getTaskFunction, blockParameter);
        }

        public Task<GetTaskOutputDTO> GetTaskQueryAsync(BigInteger taskIndex, BlockParameter blockParameter = null)
        {
            var getTaskFunction = new GetTaskFunction();
                getTaskFunction.TaskIndex = taskIndex;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetTaskFunction, GetTaskOutputDTO>(getTaskFunction, blockParameter);
        }

        public Task<BigInteger> GetTaskCountQueryAsync(GetTaskCountFunction getTaskCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTaskCountFunction, BigInteger>(getTaskCountFunction, blockParameter);
        }

        
        public Task<BigInteger> GetTaskCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTaskCountFunction, BigInteger>(null, blockParameter);
        }

        public Task<bool> IsInitialisedQueryAsync(IsInitialisedFunction isInitialisedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsInitialisedFunction, bool>(isInitialisedFunction, blockParameter);
        }

        
        public Task<bool> IsInitialisedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsInitialisedFunction, bool>(null, blockParameter);
        }

        public Task<string> PaymentRequestAsync(PaymentFunction paymentFunction)
        {
             return ContractHandler.SendRequestAsync(paymentFunction);
        }

        public Task<TransactionReceipt> PaymentRequestAndWaitForReceiptAsync(PaymentFunction paymentFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(paymentFunction, cancellationToken);
        }

        public Task<string> PaymentRequestAsync(byte status, BigInteger paidValue, BigInteger paidTaxValue)
        {
            var paymentFunction = new PaymentFunction();
                paymentFunction.Status = status;
                paymentFunction.PaidValue = paidValue;
                paymentFunction.PaidTaxValue = paidTaxValue;
            
             return ContractHandler.SendRequestAsync(paymentFunction);
        }

        public Task<TransactionReceipt> PaymentRequestAndWaitForReceiptAsync(byte status, BigInteger paidValue, BigInteger paidTaxValue, CancellationTokenSource cancellationToken = null)
        {
            var paymentFunction = new PaymentFunction();
                paymentFunction.Status = status;
                paymentFunction.PaidValue = paidValue;
                paymentFunction.PaidTaxValue = paidTaxValue;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(paymentFunction, cancellationToken);
        }

        public Task<string> RescheduleRequestAsync(RescheduleFunction rescheduleFunction)
        {
             return ContractHandler.SendRequestAsync(rescheduleFunction);
        }

        public Task<TransactionReceipt> RescheduleRequestAndWaitForReceiptAsync(RescheduleFunction rescheduleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(rescheduleFunction, cancellationToken);
        }

        public Task<string> RescheduleRequestAsync(BigInteger dueOn)
        {
            var rescheduleFunction = new RescheduleFunction();
                rescheduleFunction.DueOn = dueOn;
            
             return ContractHandler.SendRequestAsync(rescheduleFunction);
        }

        public Task<TransactionReceipt> RescheduleRequestAndWaitForReceiptAsync(BigInteger dueOn, CancellationTokenSource cancellationToken = null)
        {
            var rescheduleFunction = new RescheduleFunction();
                rescheduleFunction.DueOn = dueOn;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(rescheduleFunction, cancellationToken);
        }

        public Task<string> SetHeaderRequestAsync(SetHeaderFunction setHeaderFunction)
        {
             return ContractHandler.SendRequestAsync(setHeaderFunction);
        }

        public Task<TransactionReceipt> SetHeaderRequestAndWaitForReceiptAsync(SetHeaderFunction setHeaderFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setHeaderFunction, cancellationToken);
        }

        public Task<string> SetHeaderRequestAsync(string invoiceNumber, string mirrorNumber, byte invoicePolarity, byte paymentPolarity, byte status, BigInteger dueOn, BigInteger invoicedOn, BigInteger invoiceValue, BigInteger taxValue, string paymentTerms, string unitOfCharge)
        {
            var setHeaderFunction = new SetHeaderFunction();
                setHeaderFunction.InvoiceNumber = invoiceNumber;
                setHeaderFunction.MirrorNumber = mirrorNumber;
                setHeaderFunction.InvoicePolarity = invoicePolarity;
                setHeaderFunction.PaymentPolarity = paymentPolarity;
                setHeaderFunction.Status = status;
                setHeaderFunction.DueOn = dueOn;
                setHeaderFunction.InvoicedOn = invoicedOn;
                setHeaderFunction.InvoiceValue = invoiceValue;
                setHeaderFunction.TaxValue = taxValue;
                setHeaderFunction.PaymentTerms = paymentTerms;
                setHeaderFunction.UnitOfCharge = unitOfCharge;
            
             return ContractHandler.SendRequestAsync(setHeaderFunction);
        }

        public Task<TransactionReceipt> SetHeaderRequestAndWaitForReceiptAsync(string invoiceNumber, string mirrorNumber, byte invoicePolarity, byte paymentPolarity, byte status, BigInteger dueOn, BigInteger invoicedOn, BigInteger invoiceValue, BigInteger taxValue, string paymentTerms, string unitOfCharge, CancellationTokenSource cancellationToken = null)
        {
            var setHeaderFunction = new SetHeaderFunction();
                setHeaderFunction.InvoiceNumber = invoiceNumber;
                setHeaderFunction.MirrorNumber = mirrorNumber;
                setHeaderFunction.InvoicePolarity = invoicePolarity;
                setHeaderFunction.PaymentPolarity = paymentPolarity;
                setHeaderFunction.Status = status;
                setHeaderFunction.DueOn = dueOn;
                setHeaderFunction.InvoicedOn = invoicedOn;
                setHeaderFunction.InvoiceValue = invoiceValue;
                setHeaderFunction.TaxValue = taxValue;
                setHeaderFunction.PaymentTerms = paymentTerms;
                setHeaderFunction.UnitOfCharge = unitOfCharge;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setHeaderFunction, cancellationToken);
        }

        public Task<string> StatusChangeRequestAsync(StatusChangeFunction statusChangeFunction)
        {
             return ContractHandler.SendRequestAsync(statusChangeFunction);
        }

        public Task<TransactionReceipt> StatusChangeRequestAndWaitForReceiptAsync(StatusChangeFunction statusChangeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(statusChangeFunction, cancellationToken);
        }

        public Task<string> StatusChangeRequestAsync(byte status)
        {
            var statusChangeFunction = new StatusChangeFunction();
                statusChangeFunction.Status = status;
            
             return ContractHandler.SendRequestAsync(statusChangeFunction);
        }

        public Task<TransactionReceipt> StatusChangeRequestAndWaitForReceiptAsync(byte status, CancellationTokenSource cancellationToken = null)
        {
            var statusChangeFunction = new StatusChangeFunction();
                statusChangeFunction.Status = status;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(statusChangeFunction, cancellationToken);
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
