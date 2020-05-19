/*
Trade Control Network
Org contract
Author IAM

github.com/tradecontrol/tc-network

Licence GNU General Public License v3.0
https://www.gnu.org/licenses/gpl-3.0.en.html

*/
pragma solidity >=0.4.21 <=0.5.12;

import "./mortal.sol";
import "./OrgLib.sol";

contract Org is mortal
{
    using OrgLib for OrgLib.Orgs;

    OrgLib.Orgs orgs;

    event OnAuthorisation (address indexed eoa, bool isAuthorised);
    event OnActivityMirror (address indexed eoa, string activityCode, string allocationCode);
    event OnCashCodeMirror (address indexed eoa, string cashCode, string chargeCode);
    event OnTaskDeploy (address indexed eoa, string taskCode);
    event OnInvoiceDeploy (address indexed eoa, string invoiceNumber);
    event OnTaskNotify (address indexed eoa, address mirrorContract);
    event OnInvoiceNotify (address indexed eoa, address mirrorContract);

    function Owner() public view returns (address eoa) { return owner; }

    // ** Accounts **
    function NewMember(address _eoa, address _orgContract, string memory _accountCode, bool _isAuthorised) public onlyOwner {
        orgs.NewMember(_eoa, _orgContract, _accountCode, _isAuthorised);
        emit OnAuthorisation(_eoa, _isAuthorised);
    }

    function Authorise(string memory _accountCode, bool _isAuthorised) public onlyOwner {
        orgs.Authorise(_accountCode, _isAuthorised);
        emit OnAuthorisation(orgs.GetEOA(_accountCode), _isAuthorised);
    }

    function EoaAccountCode(address _eoa) public view returns (string memory _accountCode) {
        if (orgs.IsAuthorised(_eoa))
            _accountCode = orgs.GetAccountCode(_eoa);
    }

    function SenderAccountCode() public view returns (string memory _accountCode) {
        return EoaAccountCode(msg.sender);
    }

    function GetConsortium(string memory _accountCode) public view returns (address _consortium) {
        return orgs.GetConsortium(_accountCode);
    }

    function GetEOA (string memory _accountCode) public view returns (address _eoa) {
        return orgs.GetEOA(_accountCode);
    }

    function IsAuthorisedAccount(string memory _accountCode) public view returns (bool _isAuthorised) {
        return orgs.IsAuthorised(_accountCode);
    }

    function IsAuthorised() public view returns (bool _isAuthorised) {
        return orgs.IsAuthorised(msg.sender);
    }

    // ** Activities **
    function ActivityMirror(string memory _activityCode, string memory _allocationCode) public  {
        require (orgs.IsAuthorised(msg.sender), "Un-authorised account");
        orgs.ActivityMirror(msg.sender, _activityCode, _allocationCode);
        emit OnActivityMirror(msg.sender, _activityCode, _allocationCode);
    }

    function AllocationCode(address _eoa, string memory _activityCode) public view returns (string memory _allocationCode) {
        require (orgs.IsAuthorised(msg.sender) || msg.sender == owner, "Un-authorised account");
        _allocationCode = orgs.AllocationCode(_eoa, _activityCode);
    }

    // ** Cash Codes **
    function CashCodeMirror(string memory _cashCode, string memory _chargeCode) public  {
        require (orgs.IsAuthorised(msg.sender), "Un-authorised account");
        orgs.CashCodeMirror(msg.sender, _cashCode, _chargeCode);
        emit OnCashCodeMirror(msg.sender, _cashCode, _chargeCode);
    }

    function ChargeCode(address _eoa, string memory _cashCode) public view returns (string memory _chargeCode) {
        require (orgs.IsAuthorised(msg.sender) || msg.sender == owner, "Un-authorised account");
        _chargeCode = orgs.AllocationCode(_eoa, _cashCode);
    }

    // ** Tasks **
    function TaskNew (string memory _taskCode, string memory _accountCode, address _taskContract) public onlyOwner {
        orgs.TaskNew(_taskCode, _taskContract);
        emit OnTaskDeploy(orgs.GetEOA(_accountCode), _taskCode);
    }

    function TaskNotification(address _mirrorContract) public {
        require (orgs.IsAuthorised(msg.sender), "Un-authorised account");
        emit OnTaskNotify(msg.sender, _mirrorContract);
    }

    function TaskContract(string memory _taskCode) public view returns (address ownerContract) {
        return orgs.TaskContract(_taskCode);
    }

    // ** Invoices **
    function InvoiceNew (string memory _accountCode, string memory _invoiceNumber, address _invoiceContract) public onlyOwner {
        orgs.InvoiceNew(_accountCode, _invoiceNumber, _invoiceContract);
        emit OnInvoiceDeploy(orgs.GetEOA(_accountCode), _invoiceNumber);
    }

    function InvoiceNotification(address _mirrorContract) public {
        require (orgs.IsAuthorised(msg.sender), "Un-authorised account");
        emit OnInvoiceNotify(msg.sender, _mirrorContract);
    }

    function InvoiceMirror(string memory _invoiceNumber, address _mirrorContract) public {
        orgs.InvoiceMirror(_invoiceNumber, _mirrorContract);
        emit OnInvoiceNotify(msg.sender, _mirrorContract);
    }

    function InvoiceIsMirrored(string memory _accountCode,  string memory _invoiceNumber)
                                public view returns (bool isMirrored) {
        return orgs.InvoiceIsMirrored(_accountCode, _invoiceNumber);
    }

    function InvoiceOwnerContract(string memory _accountCode,  string memory _invoiceNumber)
                                public view returns (address ownerContract) {
        return orgs.InvoiceOwnerContract(_accountCode, _invoiceNumber);
    }

    function InvoiceMirrorContract(string memory _accountCode,  string memory _invoiceNumber)
                                public view returns (address ownerContract) {
        return orgs.InvoiceMirrorContract(_accountCode, _invoiceNumber);
    }
}