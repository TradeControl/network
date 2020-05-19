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

namespace Solidity.Contracts.OrgLib.ContractDefinition
{


    public partial class OrgLibDeployment : OrgLibDeploymentBase
    {
        public OrgLibDeployment() : base(BYTECODE) { }
        public OrgLibDeployment(string byteCode) : base(byteCode) { }
    }

    public class OrgLibDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "60556023600b82828239805160001a607314601657fe5b30600052607381538281f3fe73000000000000000000000000000000000000000030146080604052600080fdfea265627a7a72315820c253f70d71ef47664b54bef2626c849f8bf04999801e7f2d1238b225f9c3f32b64736f6c634300050c0032";
        public OrgLibDeploymentBase() : base(BYTECODE) { }
        public OrgLibDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class OnAuthorisationEventDTO : OnAuthorisationEventDTOBase { }

    [Event("OnAuthorisation")]
    public class OnAuthorisationEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("bool", "isAuthorised", 2, false )]
        public virtual bool IsAuthorised { get; set; }
    }

    public partial class OnInvoiceAddEventDTO : OnInvoiceAddEventDTOBase { }

    [Event("OnInvoiceAdd")]
    public class OnInvoiceAddEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("string", "invoiceNumber", 2, false )]
        public virtual string InvoiceNumber { get; set; }
    }

    public partial class OnInvoiceMirroredEventDTO : OnInvoiceMirroredEventDTOBase { }

    [Event("OnInvoiceMirrored")]
    public class OnInvoiceMirroredEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("address", "mirroredInvoice", 2, false )]
        public virtual string MirroredInvoice { get; set; }
        [Parameter("string", "invoiceNumber", 3, false )]
        public virtual string InvoiceNumber { get; set; }
    }

    public partial class OnInvoiceNotifyEventDTO : OnInvoiceNotifyEventDTOBase { }

    [Event("OnInvoiceNotify")]
    public class OnInvoiceNotifyEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("address", "invoice", 2, false )]
        public virtual string Invoice { get; set; }
    }

    public partial class OnTaskAddEventDTO : OnTaskAddEventDTOBase { }

    [Event("OnTaskAdd")]
    public class OnTaskAddEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("string", "taskCode", 2, false )]
        public virtual string TaskCode { get; set; }
    }

    public partial class OnTaskMirroredEventDTO : OnTaskMirroredEventDTOBase { }

    [Event("OnTaskMirrored")]
    public class OnTaskMirroredEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("address", "mirroredTask", 2, false )]
        public virtual string MirroredTask { get; set; }
        [Parameter("string", "taskCode", 3, false )]
        public virtual string TaskCode { get; set; }
    }

    public partial class OnTaskNotifyEventDTO : OnTaskNotifyEventDTOBase { }

    [Event("OnTaskNotify")]
    public class OnTaskNotifyEventDTOBase : IEventDTO
    {
        [Parameter("address", "eoa", 1, true )]
        public virtual string Eoa { get; set; }
        [Parameter("address", "task", 2, false )]
        public virtual string Task { get; set; }
    }
}
