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
using Solidity.Contracts.Task.ContractDefinition;

namespace Solidity.Contracts.Task
{
    public partial class TaskService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TaskDeployment taskDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TaskDeployment>().SendRequestAndWaitForReceiptAsync(taskDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TaskDeployment taskDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TaskDeployment>().SendRequestAsync(taskDeployment);
        }

        public static async Task<TaskService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TaskDeployment taskDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, taskDeployment, cancellationTokenSource);
            return new TaskService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TaskService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> DeliveredRequestAsync(DeliveredFunction deliveredFunction)
        {
             return ContractHandler.SendRequestAsync(deliveredFunction);
        }

        public Task<TransactionReceipt> DeliveredRequestAndWaitForReceiptAsync(DeliveredFunction deliveredFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(deliveredFunction, cancellationToken);
        }

        public Task<string> DeliveredRequestAsync(BigInteger actionOn, BigInteger quantity)
        {
            var deliveredFunction = new DeliveredFunction();
                deliveredFunction.ActionOn = actionOn;
                deliveredFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(deliveredFunction);
        }

        public Task<TransactionReceipt> DeliveredRequestAndWaitForReceiptAsync(BigInteger actionOn, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var deliveredFunction = new DeliveredFunction();
                deliveredFunction.ActionOn = actionOn;
                deliveredFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(deliveredFunction, cancellationToken);
        }

        public Task<GetHeaderOutputDTO> GetHeaderQueryAsync(GetHeaderFunction getHeaderFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHeaderFunction, GetHeaderOutputDTO>(getHeaderFunction, blockParameter);
        }

        public Task<GetHeaderOutputDTO> GetHeaderQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetHeaderFunction, GetHeaderOutputDTO>(null, blockParameter);
        }

        public Task<GetScheduleOutputDTO> GetScheduleQueryAsync(GetScheduleFunction getScheduleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetScheduleFunction, GetScheduleOutputDTO>(getScheduleFunction, blockParameter);
        }

        public Task<GetScheduleOutputDTO> GetScheduleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetScheduleFunction, GetScheduleOutputDTO>(null, blockParameter);
        }

        public Task<string> InitialiseRequestAsync(InitialiseFunction initialiseFunction)
        {
             return ContractHandler.SendRequestAsync(initialiseFunction);
        }

        public Task<TransactionReceipt> InitialiseRequestAndWaitForReceiptAsync(InitialiseFunction initialiseFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(initialiseFunction, cancellationToken);
        }

        public Task<string> InitialiseRequestAsync(string taskCode, string targetConsortium, byte polarity, string activityCode, string description, string title, byte status, BigInteger actionOn, BigInteger unitCharge, BigInteger quantity, BigInteger taxRate, string unitOfMeasure, string unitOfCharge)
        {
            var initialiseFunction = new InitialiseFunction();
                initialiseFunction.TaskCode = taskCode;
                initialiseFunction.TargetConsortium = targetConsortium;
                initialiseFunction.Polarity = polarity;
                initialiseFunction.ActivityCode = activityCode;
                initialiseFunction.Description = description;
                initialiseFunction.Title = title;
                initialiseFunction.Status = status;
                initialiseFunction.ActionOn = actionOn;
                initialiseFunction.UnitCharge = unitCharge;
                initialiseFunction.Quantity = quantity;
                initialiseFunction.TaxRate = taxRate;
                initialiseFunction.UnitOfMeasure = unitOfMeasure;
                initialiseFunction.UnitOfCharge = unitOfCharge;
            
             return ContractHandler.SendRequestAsync(initialiseFunction);
        }

        public Task<TransactionReceipt> InitialiseRequestAndWaitForReceiptAsync(string taskCode, string targetConsortium, byte polarity, string activityCode, string description, string title, byte status, BigInteger actionOn, BigInteger unitCharge, BigInteger quantity, BigInteger taxRate, string unitOfMeasure, string unitOfCharge, CancellationTokenSource cancellationToken = null)
        {
            var initialiseFunction = new InitialiseFunction();
                initialiseFunction.TaskCode = taskCode;
                initialiseFunction.TargetConsortium = targetConsortium;
                initialiseFunction.Polarity = polarity;
                initialiseFunction.ActivityCode = activityCode;
                initialiseFunction.Description = description;
                initialiseFunction.Title = title;
                initialiseFunction.Status = status;
                initialiseFunction.ActionOn = actionOn;
                initialiseFunction.UnitCharge = unitCharge;
                initialiseFunction.Quantity = quantity;
                initialiseFunction.TaxRate = taxRate;
                initialiseFunction.UnitOfMeasure = unitOfMeasure;
                initialiseFunction.UnitOfCharge = unitOfCharge;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(initialiseFunction, cancellationToken);
        }

        public Task<bool> IsInitialisedQueryAsync(IsInitialisedFunction isInitialisedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsInitialisedFunction, bool>(isInitialisedFunction, blockParameter);
        }

        
        public Task<bool> IsInitialisedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsInitialisedFunction, bool>(null, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> PriceChangeRequestAsync(PriceChangeFunction priceChangeFunction)
        {
             return ContractHandler.SendRequestAsync(priceChangeFunction);
        }

        public Task<TransactionReceipt> PriceChangeRequestAndWaitForReceiptAsync(PriceChangeFunction priceChangeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(priceChangeFunction, cancellationToken);
        }

        public Task<string> PriceChangeRequestAsync(BigInteger unitCharge, BigInteger taxRate)
        {
            var priceChangeFunction = new PriceChangeFunction();
                priceChangeFunction.UnitCharge = unitCharge;
                priceChangeFunction.TaxRate = taxRate;
            
             return ContractHandler.SendRequestAsync(priceChangeFunction);
        }

        public Task<TransactionReceipt> PriceChangeRequestAndWaitForReceiptAsync(BigInteger unitCharge, BigInteger taxRate, CancellationTokenSource cancellationToken = null)
        {
            var priceChangeFunction = new PriceChangeFunction();
                priceChangeFunction.UnitCharge = unitCharge;
                priceChangeFunction.TaxRate = taxRate;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(priceChangeFunction, cancellationToken);
        }

        public Task<string> RescheduleRequestAsync(RescheduleFunction rescheduleFunction)
        {
             return ContractHandler.SendRequestAsync(rescheduleFunction);
        }

        public Task<TransactionReceipt> RescheduleRequestAndWaitForReceiptAsync(RescheduleFunction rescheduleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(rescheduleFunction, cancellationToken);
        }

        public Task<string> RescheduleRequestAsync(BigInteger actionOn, BigInteger quantity)
        {
            var rescheduleFunction = new RescheduleFunction();
                rescheduleFunction.ActionOn = actionOn;
                rescheduleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(rescheduleFunction);
        }

        public Task<TransactionReceipt> RescheduleRequestAndWaitForReceiptAsync(BigInteger actionOn, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var rescheduleFunction = new RescheduleFunction();
                rescheduleFunction.ActionOn = actionOn;
                rescheduleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(rescheduleFunction, cancellationToken);
        }

        public Task<string> SetStatusRequestAsync(SetStatusFunction setStatusFunction)
        {
             return ContractHandler.SendRequestAsync(setStatusFunction);
        }

        public Task<TransactionReceipt> SetStatusRequestAndWaitForReceiptAsync(SetStatusFunction setStatusFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setStatusFunction, cancellationToken);
        }

        public Task<string> SetStatusRequestAsync(byte status, BigInteger actionedOn)
        {
            var setStatusFunction = new SetStatusFunction();
                setStatusFunction.Status = status;
                setStatusFunction.ActionedOn = actionedOn;
            
             return ContractHandler.SendRequestAsync(setStatusFunction);
        }

        public Task<TransactionReceipt> SetStatusRequestAndWaitForReceiptAsync(byte status, BigInteger actionedOn, CancellationTokenSource cancellationToken = null)
        {
            var setStatusFunction = new SetStatusFunction();
                setStatusFunction.Status = status;
                setStatusFunction.ActionedOn = actionedOn;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setStatusFunction, cancellationToken);
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
