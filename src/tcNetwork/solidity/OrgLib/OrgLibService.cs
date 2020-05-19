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
using Solidity.Contracts.OrgLib.ContractDefinition;

namespace Solidity.Contracts.OrgLib
{
    public partial class OrgLibService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, OrgLibDeployment orgLibDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<OrgLibDeployment>().SendRequestAndWaitForReceiptAsync(orgLibDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, OrgLibDeployment orgLibDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<OrgLibDeployment>().SendRequestAsync(orgLibDeployment);
        }

        public static async Task<OrgLibService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, OrgLibDeployment orgLibDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, orgLibDeployment, cancellationTokenSource);
            return new OrgLibService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public OrgLibService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }


    }
}
