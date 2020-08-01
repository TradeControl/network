/*
Trade Control Network
Invoice contract
Author IAM

github.com/tradecontrol/tc-network

Licence GNU General Public License v3.0
https://www.gnu.org/licenses/gpl-3.0.en.html

*/
pragma solidity >=0.4.21 <=0.5.12;

import "./mortal.sol";
import "./lib/SafeMathLib.sol";

contract Invoice is mortal {
    using SafeMathLib for uint;

    struct Detail {
        uint Quantity;
        uint InvoiceValue;
        uint TaxValue;
        string TaxCode;
    }

    struct Task {
        address ContractAddress;
        Detail Charge;
    }

    struct Item {
        string ChargeCode;
        string ChargeDescription;
        Detail Charge;
    }

    enum InvoiceStatus { Pending, Invoiced, PartiallyPaid, Paid }
    enum InvoiceState { Instantiated, Header, Details }
    enum CashPolarity { Expense, Income }

    CashPolarity invoicePolarity;
    CashPolarity paymentPolarity;
    string unitOfCharge;

    InvoiceStatus status;
    InvoiceState state;

    string invoiceNumber;
    string mirrorNumber;
    string paymentTerms;
    string paymentAddress;

    uint invoicedOn;
    uint dueOn;

    uint invoiceValue;
    uint taxValue;
    uint paidValue;
    uint paidTaxValue;

    Task[] tasks;
    Item[] items;

    event OnPayment (uint paymentValue, uint outstandingAmount);
    event OnStatusChange (InvoiceStatus status);
    event OnDueChange (uint nowDue);
    event OnPaymentAddress (string paymentAddress);

    constructor () public {
        state = InvoiceState.Instantiated;
        invoicePolarity = CashPolarity.Income;
        paymentPolarity = CashPolarity.Income;
    }

    // ** initialisation **
    function SetHeader(string memory _invoiceNumber, string memory _mirrorNumber,
                            CashPolarity _invoicePolarity, CashPolarity _paymentPolarity,
                            InvoiceStatus _status, uint _dueOn,
                            uint _invoicedOn, uint _invoiceValue, uint _taxValue,
                            string memory _paymentTerms, string memory _unitOfCharge) public onlyOwner {
        require(state == InvoiceState.Instantiated, "Invoice header is already asigned");
        invoiceNumber = _invoiceNumber;
        mirrorNumber = _mirrorNumber;
        invoicePolarity = _invoicePolarity;
        paymentPolarity = _paymentPolarity;
        status = _status;
        dueOn = _dueOn;
        invoicedOn = _invoicedOn;
        invoiceValue = _invoiceValue;
        taxValue = _taxValue;
        paymentTerms = _paymentTerms;
        unitOfCharge = _unitOfCharge;
        state = InvoiceState.Header;
        paymentAddress = "";
    }


    function AddTask(address _contractAddress,
            uint _quantity, uint _invoiceValue, uint _taxValue, string memory _taxCode) public onlyOwner {
        Task memory task;
        task.ContractAddress = _contractAddress;
        task.Charge.Quantity = _quantity;
        task.Charge.InvoiceValue = _invoiceValue;
        task.Charge.TaxValue = _taxValue;
        task.Charge.TaxCode = _taxCode;
        state = InvoiceState.Details;
        tasks.push(task);
    }

    function AddItem(string memory _chargeCode, string memory _chargeDescription,
            uint _invoiceValue, uint _taxValue, string memory _taxCode) public onlyOwner {
        Item memory item;
        item.ChargeCode = _chargeCode;
        item.ChargeDescription = _chargeDescription;
        item.Charge.Quantity = 0;
        item.Charge.InvoiceValue = _invoiceValue;
        item.Charge.TaxValue = _taxValue;
        item.Charge.TaxCode = _taxCode;
        state = InvoiceState.Details;
        items.push(item);
    }

    function IsInitialised() public view returns (bool isInitialised) { return (state == InvoiceState.Details); }

    // ** properties **
    function GetHeader() public view returns (
            string memory _invoiceNumber, string memory _mirrorNumber,
                CashPolarity _invoicePolarity, CashPolarity _paymentPolarity,
                string memory _paymentTerms, string memory _unitOfCharge) {
        require (state != InvoiceState.Instantiated, "Not set!");
        _invoiceNumber = invoiceNumber;
        _mirrorNumber = mirrorNumber;
        _invoicePolarity = invoicePolarity;
        _paymentPolarity = paymentPolarity;
        _paymentTerms = paymentTerms;
        _unitOfCharge = unitOfCharge;
    }

    function GetParams() public view returns (InvoiceStatus _status, uint _dueOn, uint _invoicedOn,
            uint _invoiceValue, uint _taxValue, uint _paidValue, uint _paidTaxValue, string memory _paymentAddress) {
        require (state != InvoiceState.Instantiated, "Not set!");
        _status = status;
        _dueOn = dueOn;
        _invoicedOn = invoicedOn;
        _invoiceValue = invoiceValue;
        _taxValue = taxValue;
        _paidValue = paidValue;
        _paidTaxValue = paidTaxValue;
        _paymentAddress = paymentAddress;
    }

    function GetTaskCount() public view returns (uint _taskCount) {
        return tasks.length;
    }

    function GetTask(uint _taskIndex) public view returns
                (address _contractAddress, uint _quantity, uint _invoiceValue, uint _taxValue, string memory _taxCode) {
        require (state == InvoiceState.Details && _taskIndex < tasks.length, "Task cannot be located!");
        _contractAddress = tasks[_taskIndex].ContractAddress;
        _quantity = tasks[_taskIndex].Charge.Quantity;
        _invoiceValue = tasks[_taskIndex].Charge.InvoiceValue;
        _taxValue = tasks[_taskIndex].Charge.TaxValue;
        _taxCode = tasks[_taskIndex].Charge.TaxCode;
    }

    function GetItemCount() public view returns (uint _itemCount) {
        return items.length;
    }

    function GetItem(uint _itemIndex) public view returns
        (string memory _chargeCode, string memory _chargeDescription,
            uint _invoiceValue, uint _taxValue, string memory _taxCode) {
        _chargeCode = items[_itemIndex].ChargeCode;
        _chargeDescription = items[_itemIndex].ChargeDescription;
        _invoiceValue = items[_itemIndex].Charge.InvoiceValue;
        _taxValue = items[_itemIndex].Charge.TaxValue;
        _taxCode = items[_itemIndex].Charge.TaxCode;
    }

    // ** process **
    function SetPaymentAddress (string memory _paymentAddress) public onlyOwner {
        require (state != InvoiceState.Instantiated, "Not configured!");
        paymentAddress = _paymentAddress;
        emit OnPaymentAddress(_paymentAddress);
    }

    function Reschedule(uint _dueOn) public onlyOwner {
        require (state != InvoiceState.Instantiated, "Not configured!");
        dueOn = _dueOn;
        emit OnDueChange(_dueOn);
    }

    function Payment(InvoiceStatus _status, uint _paidValue, uint _paidTaxValue) public onlyOwner {
        require (state != InvoiceState.Instantiated, "Not configured!");
        require (_paidValue.plus(_paidTaxValue) <= (invoiceValue.plus(taxValue)), "Payment exceeds the total invoice value!");
        paidValue = _paidValue;
        paidTaxValue = _paidTaxValue;
        emit OnPayment (_paidValue.plus(_paidTaxValue), invoiceValue.plus(taxValue).minus(_paidValue.plus(_paidTaxValue)));

        if (_status != status)
            StatusChange(_status);
    }

    function StatusChange(InvoiceStatus _status) public onlyOwner {
        require (state >= InvoiceState.Header, "Not configured!");
        status = _status;
        emit OnStatusChange (status);
    }
}