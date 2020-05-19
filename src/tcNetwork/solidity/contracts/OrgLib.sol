/*
Trade Control Network
Org library (embedded)
Author IAM

github.com/tradecontrol/tc-network

Licence GNU General Public License v3.0
https://www.gnu.org/licenses/gpl-3.0.en.html

*/
pragma solidity >=0.4.21 <=0.5.12;

library OrgLib
{
    struct Account {
        address EOA;
        bool IsAuthorised;
    }

    struct Consortium {
        address OrgContract;
        string AccountCode;
    }

    struct Invoice {
        address OwnerContract;
        address MirrorContract;
        bool IsMirrored;
    }

    struct Orgs {
        mapping (string => Account) Accounts;
        mapping (address => Consortium) Consortiums;
        mapping (address => mapping (string => string)) Activities;
        mapping (address => mapping (string => string)) CashCodes;
        mapping (string => address) Tasks;
        mapping (string => mapping (string => Invoice)) Invoices;
    }

    // ** Consortium **
    function NewMember(Orgs storage _self, address _eoa, address _orgContract, string memory _accountCode, bool _isAuthorised) internal {
        _self.Consortiums[_eoa].AccountCode = _accountCode;
        _self.Consortiums[_eoa].OrgContract = _orgContract;
        _self.Accounts[_accountCode].IsAuthorised = _isAuthorised;
        _self.Accounts[_accountCode].EOA = _eoa;
    }

    function Authorise (Orgs storage _self, string memory _accountCode, bool _isAuthorised) internal {
        _self.Accounts[_accountCode].IsAuthorised = _isAuthorised;
    }

    function IsAuthorised(Orgs storage _self, address _eoa) internal view returns (bool _isAuthorised) {
        return _self.Accounts[_self.Consortiums[_eoa].AccountCode].IsAuthorised;
    }

    function IsAuthorised(Orgs storage _self, string memory _accountCode) internal view returns (bool _isAuthorised) {
        return _self.Accounts[_accountCode].IsAuthorised;
    }

    function GetAccountCode (Orgs storage _self, address _eoa) internal view returns (string memory _accountCode) {
        return _self.Consortiums[_eoa].AccountCode;
    }

    function GetAccountCode (Orgs storage _self) internal view returns (string memory _accountCode) {
        return _self.Consortiums[msg.sender].AccountCode;
    }

    function GetConsortium (Orgs storage _self, string memory _accountCode) internal view returns (address _consortium) {
        return  _self.Consortiums[_self.Accounts[_accountCode].EOA].OrgContract;
    }

    function GetEOA (Orgs storage _self, string memory _accountCode) internal view returns (address _eoa) {
        return _self.Accounts[_accountCode].EOA;
    }

    // ** Activities **
    function ActivityMirror(Orgs storage _self, address _eoa,
        string memory _activityCode, string memory _allocationCode) internal
    {
        _self.Activities[_eoa][_activityCode] = _allocationCode;
    }

    function AllocationCode(Orgs storage _self, address _eoa,
        string memory _activityCode) internal view returns (string memory allocationCode) {
        allocationCode = _self.Activities[_eoa][_activityCode];
    }

    // ** Cash Codes **
    function CashCodeMirror(Orgs storage _self, address _eoa,
        string memory _cashCode, string memory _chargeCode) internal
    {
        _self.Activities[_eoa][_cashCode] = _chargeCode;
    }

    function ChargeCode(Orgs storage _self, address _eoa,
        string memory _cashCode) internal view returns (string memory chargeCode) {
        chargeCode = _self.CashCodes[_eoa][_cashCode];
    }


    // ** Tasks **
    function TaskNew (Orgs storage _self, string memory _taskCode, address _taskContract) internal
    {
        _self.Tasks[_taskCode] = _taskContract;
    }

    function TaskContract(Orgs storage _self,
                        string memory _taskCode) internal view returns (address ownerContract) {
        ownerContract = _self.Tasks[_taskCode];
    }


    // ** Invoices **
    function InvoiceNew (Orgs storage _self, string memory _accountCode, string memory _invoiceNumber, address _invoiceContract) internal
    {
        _self.Invoices[_accountCode][_invoiceNumber] = Invoice(_invoiceContract, _invoiceContract, false);
    }

    function InvoiceMirror(Orgs storage _self, string memory _invoiceNumber, address _invoiceContract) internal
    {
        require (_self.Accounts[_self.Consortiums[msg.sender].AccountCode].IsAuthorised, "Un-authorised account");
        string memory accountCode;
        accountCode = _self.Consortiums[msg.sender].AccountCode;
        require (!_self.Invoices[accountCode][_invoiceNumber].IsMirrored, "Invoice is already mirrored");

        _self.Invoices[accountCode][_invoiceNumber].MirrorContract = _invoiceContract;
        _self.Invoices[accountCode][_invoiceNumber].IsMirrored = true;
    }

    function InvoiceIsMirrored (Orgs storage _self, string memory _accountCode,
                        string memory _invoiceNumber) internal view returns (bool isMirrored) {
        isMirrored = _self.Invoices[_accountCode][_invoiceNumber].IsMirrored;
    }

    function InvoiceOwnerContract(Orgs storage _self, string memory _accountCode,
                    string memory _invoiceNumber) internal view returns (address ownerContract) {
        ownerContract = _self.Invoices[_accountCode][_invoiceNumber].OwnerContract;
    }

    function InvoiceMirrorContract(Orgs storage _self, string memory _accountCode,
                    string memory _invoiceNumber) internal view returns (address mirrorContract) {
        mirrorContract = _self.Invoices[_accountCode][_invoiceNumber].MirrorContract;
    }
}