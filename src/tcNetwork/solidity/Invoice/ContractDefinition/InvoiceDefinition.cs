using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace Solidity.Contracts.Invoice.ContractDefinition
{


    public partial class InvoiceDeployment : InvoiceDeploymentBase
    {
        public InvoiceDeployment() : base(BYTECODE) { }
        public InvoiceDeployment(string byteCode) : base(byteCode) { }
    }

    public class InvoiceDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b50600080546002805461ff00191690556001600160a01b031916331760ff60a01b1916740100000000000000000000000000000000000000001760ff60a81b19167501000000000000000000000000000000000000000000178155611cad90819061007b90396000f3fe608060405234801561001057600080fd5b50600436106100ea5760003560e01c80638bb397be1161008c578063b4ba11cf11610066578063b4ba11cf1461087b578063caf614a414610940578063d9ae37c714610960578063faf8101d14610acf576100ea565b80638bb397be1461042f5780639bb6a8981461060b578063b35e6d5214610873576100ea565b80634d431003116100c85780634d4310031461019057806350675ed6146101aa578063514ebda71461026d57806383197ef014610427576100ea565b806315494e9b146100ef5780632d2ba2e61461011d5780632d83626f14610173575b600080fd5b61011b6004803603606081101561010557600080fd5b5060ff8135169060208101359060400135610aeb565b005b610125610caa565b6040518088600381111561013557fe5b60ff16815260200187815260200186815260200185815260200184815260200183815260200182815260200197505050505050505060405180910390f35b61011b6004803603602081101561018957600080fd5b5035610d33565b610198610e14565b60408051918252519081900360200190f35b6101c7600480360360208110156101c057600080fd5b5035610e1b565b60405180866001600160a01b03166001600160a01b0316815260200185815260200184815260200183815260200180602001828103825283818151815260200191508051906020019080838360005b8381101561022e578181015183820152602001610216565b50505050905090810190601f16801561025b5780820380516001836020036101000a031916815260200191505b50965050505050505060405180910390f35b61011b600480360360a081101561028357600080fd5b810190602081018135600160201b81111561029d57600080fd5b8201836020820111156102af57600080fd5b803590602001918460018302840111600160201b831117156102d057600080fd5b91908080601f0160208091040260200160405190810160405280939291908181526020018383808284376000920191909152509295949360208101935035915050600160201b81111561032257600080fd5b82018360208201111561033457600080fd5b803590602001918460018302840111600160201b8311171561035557600080fd5b91908080601f0160208091040260200160405190810160405280939291908181526020018383808284376000920191909152509295843595602086013595919450925060608101915060400135600160201b8111156103b357600080fd5b8201836020820111156103c557600080fd5b803590602001918460018302840111600160201b831117156103e657600080fd5b91908080601f016020809104026020016040519081016040528093929190818152602001838380828437600092019190915250929550610fe4945050505050565b61011b611126565b61043761117d565b60405180806020018060200187600181111561044f57fe5b60ff16815260200186600181111561046357fe5b60ff168152602001806020018060200185810385528b818151815260200191508051906020019080838360005b838110156104a8578181015183820152602001610490565b50505050905090810190601f1680156104d55780820380516001836020036101000a031916815260200191505b5085810384528a5181528a516020918201918c019080838360005b838110156105085781810151838201526020016104f0565b50505050905090810190601f1680156105355780820380516001836020036101000a031916815260200191505b50858103835287518152875160209182019189019080838360005b83811015610568578181015183820152602001610550565b50505050905090810190601f1680156105955780820380516001836020036101000a031916815260200191505b50858103825286518152865160209182019188019080838360005b838110156105c85781810151838201526020016105b0565b50505050905090810190601f1680156105f55780820380516001836020036101000a031916815260200191505b509a505050505050505050505060405180910390f35b61011b600480360361016081101561062257600080fd5b810190602081018135600160201b81111561063c57600080fd5b82018360208201111561064e57600080fd5b803590602001918460018302840111600160201b8311171561066f57600080fd5b91908080601f0160208091040260200160405190810160405280939291908181526020018383808284376000920191909152509295949360208101935035915050600160201b8111156106c157600080fd5b8201836020820111156106d357600080fd5b803590602001918460018302840111600160201b831117156106f457600080fd5b91908080601f016020809104026020016040519081016040528093929190818152602001838380828437600092019190915250929560ff8535811696602087013582169660408101359092169550606082013594506080820135935060a08201359260c08301359290919061010081019060e00135600160201b81111561077a57600080fd5b82018360208201111561078c57600080fd5b803590602001918460018302840111600160201b831117156107ad57600080fd5b91908080601f0160208091040260200160405190810160405280939291908181526020018383808284376000920191909152509295949360208101935035915050600160201b8111156107ff57600080fd5b82018360208201111561081157600080fd5b803590602001918460018302840111600160201b8311171561083257600080fd5b91908080601f01602080910402602001604051908101604052809392919081815260200183838082843760009201919091525092955061143e945050505050565b6101986115c1565b61011b600480360360a081101561089157600080fd5b6001600160a01b038235169160208101359160408201359160608101359181019060a081016080820135600160201b8111156108cc57600080fd5b8201836020820111156108de57600080fd5b803590602001918460018302840111600160201b831117156108ff57600080fd5b91908080601f0160208091040260200160405190810160405280939291908181526020018383808284376000920191909152509295506115c7945050505050565b61011b6004803603602081101561095657600080fd5b503560ff1661176e565b61097d6004803603602081101561097657600080fd5b5035611881565b60405180806020018060200186815260200185815260200180602001848103845289818151815260200191508051906020019080838360005b838110156109ce5781810151838201526020016109b6565b50505050905090810190601f1680156109fb5780820380516001836020036101000a031916815260200191505b5084810383528851815288516020918201918a019080838360005b83811015610a2e578181015183820152602001610a16565b50505050905090810190601f168015610a5b5780820380516001836020036101000a031916815260200191505b50848103825285518152855160209182019187019080838360005b83811015610a8e578181015183820152602001610a76565b50505050905090810190601f168015610abb5780820380516001836020036101000a031916815260200191505b509850505050505050505060405180910390f35b610ad7611aa9565b604080519115158252519081900360200190f35b6000546001600160a01b03163314610b345760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b600060028054610100900460ff1690811115610b4c57fe5b1415610b91576040805162461bcd60e51b815260206004820152600f60248201526e4e6f7420636f6e666967757265642160881b604482015290519081900360640190fd5b600954600854610ba69163ffffffff611ac816565b610bb6838363ffffffff611ac816565b1115610bf35760405162461bcd60e51b8152600401808060200182810382526028815260200180611c236028913960400191505060405180910390fd5b600a829055600b8190557fb910ad1a904e6b93c1dd63490b406f9f34a1a557a805849ef3fc062b829c9c50610c2e838363ffffffff611ac816565b610c62610c41858563ffffffff611ac816565b600954600854610c569163ffffffff611ac816565b9063ffffffff611aea16565b6040805192835260208301919091528051918290030190a160025460ff166003811115610c8b57fe5b836003811115610c9757fe5b14610ca557610ca58361176e565b505050565b60008080808080808060028054610100900460ff1690811115610cc957fe5b1415610d07576040805162461bcd60e51b81526020600482015260086024820152674e6f74207365742160c01b604482015290519081900360640190fd5b5050600254600754600654600854600954600a54600b5460ff9096169b949a5092985090965094509250565b6000546001600160a01b03163314610d7c5760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b600060028054610100900460ff1690811115610d9457fe5b1415610dd9576040805162461bcd60e51b815260206004820152600f60248201526e4e6f7420636f6e666967757265642160881b604482015290519081900360640190fd5b60078190556040805182815290517f0453176aa5601d8c9c77d4318bfb6ef8636fc3805bf5261e01373f04f766daf29181900360200190a150565b600d545b90565b600080808060606002808054610100900460ff1690811115610e3957fe5b148015610e475750600c5486105b610e98576040805162461bcd60e51b815260206004820152601760248201527f5461736b2063616e6e6f74206265206c6f636174656421000000000000000000604482015290519081900360640190fd5b600c8681548110610ea557fe5b6000918252602090912060059091020154600c80546001600160a01b0390921696509087908110610ed257fe5b9060005260206000209060050201600101600001549350600c8681548110610ef657fe5b9060005260206000209060050201600101600101549250600c8681548110610f1a57fe5b9060005260206000209060050201600101600201549150600c8681548110610f3e57fe5b6000918252602091829020600460059092020101805460408051601f6002600019610100600187161502019094169390930492830185900485028101850190915281815292830182828015610fd45780601f10610fa957610100808354040283529160200191610fd4565b820191906000526020600020905b815481529060010190602001808311610fb757829003601f168201915b5050505050905091939590929450565b6000546001600160a01b0316331461102d5760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b611035611afc565b8581526020808201869052604080830180516000905280519092018690528151018490525160600182905260028054819061ff001916610100820217905550600d8054600181018083556000929092528251805184926006027fd7b6990105719101dabeb77144f2a3385c8033acd3af97e9423a695e81ad1eb501916110c091839160200190611b22565b5060208281015180516110d99260018501920190611b22565b50604082810151805160028401908155602080830151600386015592820151600485015560608201518051929391926111189260058701920190611b22565b505050505050505050505050565b6000546001600160a01b0316331461116f5760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b6000546001600160a01b0316ff5b60608060008082808260028054610100900460ff169081111561119c57fe5b14156111da576040805162461bcd60e51b81526020600482015260086024820152674e6f74207365742160c01b604482015290519081900360640190fd5b6003805460408051602060026001851615610100026000190190941693909304601f810184900484028201840190925281815292918301828280156112605780601f1061123557610100808354040283529160200191611260565b820191906000526020600020905b81548152906001019060200180831161124357829003601f168201915b505060048054604080516020601f60026000196101006001881615020190951694909404938401819004810282018101909252828152969c50919450925084019050828280156112f15780601f106112c6576101008083540402835291602001916112f1565b820191906000526020600020905b8154815290600101906020018083116112d457829003601f168201915b505060005460058054604080516020601f60026000196001871615610100020190951694909404938401819004810282018101909252828152979c5060ff600160a01b850481169c50600160a81b90940490931699509094509250840190508282801561139f5780601f106113745761010080835404028352916020019161139f565b820191906000526020600020905b81548152906001019060200180831161138257829003601f168201915b505060018054604080516020601f6002600019610100878916150201909516949094049384018190048102820181019092528281529698509194509250840190508282801561142f5780601f106114045761010080835404028352916020019161142f565b820191906000526020600020905b81548152906001019060200180831161141257829003601f168201915b50505050509050909192939495565b6000546001600160a01b031633146114875760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b600060028054610100900460ff169081111561149f57fe5b146114db5760405162461bcd60e51b8152600401808060200182810382526021815260200180611c026021913960400191505060405180910390fd5b8a516114ee9060039060208e0190611b22565b5089516115029060049060208d0190611b22565b50600080548a919060ff60a01b1916600160a01b83600181111561152257fe5b02179055506000805489919060ff60a81b1916600160a81b83600181111561154657fe5b02179055506002805488919060ff1916600183600381111561156457fe5b021790555060078690556006859055600884905560098390558151611590906005906020850190611b22565b5080516115a4906001906020840190611b22565b50506002805461ff00191661010017905550505050505050505050565b600c5490565b6000546001600160a01b031633146116105760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b611618611ba0565b6001600160a01b038616815260208082018051879052805190910185905280516040018490525160600182905260028054819061ff001916610100820217905550600c80546001810180835560009290925282517fdf6966c971051c3d54ec59162606531493a51404a002842f56009d7e5cf4a8c7600590920291820180546001600160a01b0319166001600160a01b0390921691909117815560208085015180517fdf6966c971051c3d54ec59162606531493a51404a002842f56009d7e5cf4a8c88501908155818301517fdf6966c971051c3d54ec59162606531493a51404a002842f56009d7e5cf4a8c986015560408201517fdf6966c971051c3d54ec59162606531493a51404a002842f56009d7e5cf4a8ca86015560608201518051889693949293611118937fdf6966c971051c3d54ec59162606531493a51404a002842f56009d7e5cf4a8cb909101920190611b22565b6000546001600160a01b031633146117b75760405162461bcd60e51b815260040180806020018281038252602e815260200180611c4b602e913960400191505060405180910390fd5b600160028054610100900460ff16908111156117cf57fe5b1015611814576040805162461bcd60e51b815260206004820152600f60248201526e4e6f7420636f6e666967757265642160881b604482015290519081900360640190fd5b6002805482919060ff1916600183600381111561182d57fe5b02179055506002546040517ff356a63ad9a5ef0acd0323a72632c8518f20400d12f024dc969ac9848523bb429160ff16908082600381111561186b57fe5b60ff16815260200191505060405180910390a150565b6060806000806060600d868154811061189657fe5b6000918252602091829020600690910201805460408051601f60026000196101006001871615020190941693909304928301859004850281018501909152818152928301828280156119295780601f106118fe57610100808354040283529160200191611929565b820191906000526020600020905b81548152906001019060200180831161190c57829003601f168201915b50505050509450600d868154811061193d57fe5b90600052602060002090600602016001018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156119e25780601f106119b7576101008083540402835291602001916119e2565b820191906000526020600020905b8154815290600101906020018083116119c557829003601f168201915b50505050509350600d86815481106119f657fe5b9060005260206000209060060201600201600101549250600d8681548110611a1a57fe5b9060005260206000209060060201600201600201549150600d8681548110611a3e57fe5b6000918252602091829020600560069092020101805460408051601f6002600019610100600187161502019094169390930492830185900485028101850190915281815292830182828015610fd45780601f10610fa957610100808354040283529160200191610fd4565b60006002808054610100900460ff1690811115611ac257fe5b14905090565b6000828201838110801590611add5750828110155b611ae357fe5b9392505050565b600082821115611af657fe5b50900390565b60405180606001604052806060815260200160608152602001611b1d611bbf565b905290565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10611b6357805160ff1916838001178555611b90565b82800160010185558215611b90579182015b82811115611b90578251825591602001919060010190611b75565b50611b9c929150611be7565b5090565b604051806040016040528060006001600160a01b03168152602001611b1d5b6040518060800160405280600081526020016000815260200160008152602001606081525090565b610e1891905b80821115611b9c5760008155600101611bed56fe496e766f6963652068656164657220697320616c726561647920617369676e65645061796d656e7420657863656564732074686520746f74616c20696e766f6963652076616c7565214f6e6c792074686520636f6e7472616374206f776e65722063616e2063616c6c20746869732066756e6374696f6ea265627a7a723158206189c170f9c5ef48edf820b314c9232487c3f39b1b2cbd4a80ec24529a6af11d64736f6c634300050c0032";
        public InvoiceDeploymentBase() : base(BYTECODE) { }
        public InvoiceDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class AddItemFunction : AddItemFunctionBase { }

    [Function("AddItem")]
    public class AddItemFunctionBase : FunctionMessage
    {
        [Parameter("string", "_chargeCode", 1)]
        public virtual string ChargeCode { get; set; }
        [Parameter("string", "_chargeDescription", 2)]
        public virtual string ChargeDescription { get; set; }
        [Parameter("uint256", "_invoiceValue", 3)]
        public virtual BigInteger InvoiceValue { get; set; }
        [Parameter("uint256", "_taxValue", 4)]
        public virtual BigInteger TaxValue { get; set; }
        [Parameter("string", "_taxCode", 5)]
        public virtual string TaxCode { get; set; }
    }

    public partial class AddTaskFunction : AddTaskFunctionBase { }

    [Function("AddTask")]
    public class AddTaskFunctionBase : FunctionMessage
    {
        [Parameter("address", "_contractAddress", 1)]
        public virtual string ContractAddress { get; set; }
        [Parameter("uint256", "_quantity", 2)]
        public virtual BigInteger Quantity { get; set; }
        [Parameter("uint256", "_invoiceValue", 3)]
        public virtual BigInteger InvoiceValue { get; set; }
        [Parameter("uint256", "_taxValue", 4)]
        public virtual BigInteger TaxValue { get; set; }
        [Parameter("string", "_taxCode", 5)]
        public virtual string TaxCode { get; set; }
    }

    public partial class GetHeaderFunction : GetHeaderFunctionBase { }

    [Function("GetHeader", typeof(GetHeaderOutputDTO))]
    public class GetHeaderFunctionBase : FunctionMessage
    {

    }

    public partial class GetItemFunction : GetItemFunctionBase { }

    [Function("GetItem", typeof(GetItemOutputDTO))]
    public class GetItemFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_itemIndex", 1)]
        public virtual BigInteger ItemIndex { get; set; }
    }

    public partial class GetItemCountFunction : GetItemCountFunctionBase { }

    [Function("GetItemCount", "uint256")]
    public class GetItemCountFunctionBase : FunctionMessage
    {

    }

    public partial class GetParamsFunction : GetParamsFunctionBase { }

    [Function("GetParams", typeof(GetParamsOutputDTO))]
    public class GetParamsFunctionBase : FunctionMessage
    {

    }

    public partial class GetTaskFunction : GetTaskFunctionBase { }

    [Function("GetTask", typeof(GetTaskOutputDTO))]
    public class GetTaskFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_taskIndex", 1)]
        public virtual BigInteger TaskIndex { get; set; }
    }

    public partial class GetTaskCountFunction : GetTaskCountFunctionBase { }

    [Function("GetTaskCount", "uint256")]
    public class GetTaskCountFunctionBase : FunctionMessage
    {

    }

    public partial class IsInitialisedFunction : IsInitialisedFunctionBase { }

    [Function("IsInitialised", "bool")]
    public class IsInitialisedFunctionBase : FunctionMessage
    {

    }

    public partial class PaymentFunction : PaymentFunctionBase { }

    [Function("Payment")]
    public class PaymentFunctionBase : FunctionMessage
    {
        [Parameter("uint8", "_status", 1)]
        public virtual byte Status { get; set; }
        [Parameter("uint256", "_paidValue", 2)]
        public virtual BigInteger PaidValue { get; set; }
        [Parameter("uint256", "_paidTaxValue", 3)]
        public virtual BigInteger PaidTaxValue { get; set; }
    }

    public partial class RescheduleFunction : RescheduleFunctionBase { }

    [Function("Reschedule")]
    public class RescheduleFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_dueOn", 1)]
        public virtual BigInteger DueOn { get; set; }
    }

    public partial class SetHeaderFunction : SetHeaderFunctionBase { }

    [Function("SetHeader")]
    public class SetHeaderFunctionBase : FunctionMessage
    {
        [Parameter("string", "_invoiceNumber", 1)]
        public virtual string InvoiceNumber { get; set; }
        [Parameter("string", "_mirrorNumber", 2)]
        public virtual string MirrorNumber { get; set; }
        [Parameter("uint8", "_invoicePolarity", 3)]
        public virtual byte InvoicePolarity { get; set; }
        [Parameter("uint8", "_paymentPolarity", 4)]
        public virtual byte PaymentPolarity { get; set; }
        [Parameter("uint8", "_status", 5)]
        public virtual byte Status { get; set; }
        [Parameter("uint256", "_dueOn", 6)]
        public virtual BigInteger DueOn { get; set; }
        [Parameter("uint256", "_invoicedOn", 7)]
        public virtual BigInteger InvoicedOn { get; set; }
        [Parameter("uint256", "_invoiceValue", 8)]
        public virtual BigInteger InvoiceValue { get; set; }
        [Parameter("uint256", "_taxValue", 9)]
        public virtual BigInteger TaxValue { get; set; }
        [Parameter("string", "_paymentTerms", 10)]
        public virtual string PaymentTerms { get; set; }
        [Parameter("string", "_unitOfCharge", 11)]
        public virtual string UnitOfCharge { get; set; }
    }

    public partial class StatusChangeFunction : StatusChangeFunctionBase { }

    [Function("StatusChange")]
    public class StatusChangeFunctionBase : FunctionMessage
    {
        [Parameter("uint8", "_status", 1)]
        public virtual byte Status { get; set; }
    }

    public partial class DestroyFunction : DestroyFunctionBase { }

    [Function("destroy")]
    public class DestroyFunctionBase : FunctionMessage
    {

    }

    public partial class OnDueChangeEventDTO : OnDueChangeEventDTOBase { }

    [Event("OnDueChange")]
    public class OnDueChangeEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "nowDue", 1, false )]
        public virtual BigInteger NowDue { get; set; }
    }

    public partial class OnPaymentEventDTO : OnPaymentEventDTOBase { }

    [Event("OnPayment")]
    public class OnPaymentEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "paymentValue", 1, false )]
        public virtual BigInteger PaymentValue { get; set; }
        [Parameter("uint256", "outstandingAmount", 2, false )]
        public virtual BigInteger OutstandingAmount { get; set; }
    }

    public partial class OnStatusChangeEventDTO : OnStatusChangeEventDTOBase { }

    [Event("OnStatusChange")]
    public class OnStatusChangeEventDTOBase : IEventDTO
    {
        [Parameter("uint8", "status", 1, false )]
        public virtual byte Status { get; set; }
    }





    public partial class GetHeaderOutputDTO : GetHeaderOutputDTOBase { }

    [FunctionOutput]
    public class GetHeaderOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "_invoiceNumber", 1)]
        public virtual string InvoiceNumber { get; set; }
        [Parameter("string", "_mirrorNumber", 2)]
        public virtual string MirrorNumber { get; set; }
        [Parameter("uint8", "_invoicePolarity", 3)]
        public virtual byte InvoicePolarity { get; set; }
        [Parameter("uint8", "_paymentPolarity", 4)]
        public virtual byte PaymentPolarity { get; set; }
        [Parameter("string", "_paymentTerms", 5)]
        public virtual string PaymentTerms { get; set; }
        [Parameter("string", "_unitOfCharge", 6)]
        public virtual string UnitOfCharge { get; set; }
    }

    public partial class GetItemOutputDTO : GetItemOutputDTOBase { }

    [FunctionOutput]
    public class GetItemOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "_chargeCode", 1)]
        public virtual string ChargeCode { get; set; }
        [Parameter("string", "_chargeDescription", 2)]
        public virtual string ChargeDescription { get; set; }
        [Parameter("uint256", "_invoiceValue", 3)]
        public virtual BigInteger InvoiceValue { get; set; }
        [Parameter("uint256", "_taxValue", 4)]
        public virtual BigInteger TaxValue { get; set; }
        [Parameter("string", "_taxCode", 5)]
        public virtual string TaxCode { get; set; }
    }

    public partial class GetItemCountOutputDTO : GetItemCountOutputDTOBase { }

    [FunctionOutput]
    public class GetItemCountOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "_itemCount", 1)]
        public virtual BigInteger ItemCount { get; set; }
    }

    public partial class GetParamsOutputDTO : GetParamsOutputDTOBase { }

    [FunctionOutput]
    public class GetParamsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint8", "_status", 1)]
        public virtual byte Status { get; set; }
        [Parameter("uint256", "_dueOn", 2)]
        public virtual BigInteger DueOn { get; set; }
        [Parameter("uint256", "_invoicedOn", 3)]
        public virtual BigInteger InvoicedOn { get; set; }
        [Parameter("uint256", "_invoiceValue", 4)]
        public virtual BigInteger InvoiceValue { get; set; }
        [Parameter("uint256", "_taxValue", 5)]
        public virtual BigInteger TaxValue { get; set; }
        [Parameter("uint256", "_paidValue", 6)]
        public virtual BigInteger PaidValue { get; set; }
        [Parameter("uint256", "_paidTaxValue", 7)]
        public virtual BigInteger PaidTaxValue { get; set; }
    }

    public partial class GetTaskOutputDTO : GetTaskOutputDTOBase { }

    [FunctionOutput]
    public class GetTaskOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "_contractAddress", 1)]
        public virtual string ContractAddress { get; set; }
        [Parameter("uint256", "_quantity", 2)]
        public virtual BigInteger Quantity { get; set; }
        [Parameter("uint256", "_invoiceValue", 3)]
        public virtual BigInteger InvoiceValue { get; set; }
        [Parameter("uint256", "_taxValue", 4)]
        public virtual BigInteger TaxValue { get; set; }
        [Parameter("string", "_taxCode", 5)]
        public virtual string TaxCode { get; set; }
    }

    public partial class GetTaskCountOutputDTO : GetTaskCountOutputDTOBase { }

    [FunctionOutput]
    public class GetTaskCountOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "_taskCount", 1)]
        public virtual BigInteger TaskCount { get; set; }
    }

    public partial class IsInitialisedOutputDTO : IsInitialisedOutputDTOBase { }

    [FunctionOutput]
    public class IsInitialisedOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "isInitialised", 1)]
        public virtual bool IsInitialised { get; set; }
    }










}
