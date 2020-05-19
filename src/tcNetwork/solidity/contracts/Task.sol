/*
Trade Control Network
Task contract
Author IAM

github.com/tradecontrol/tc-network

Licence GNU General Public License v3.0
https://www.gnu.org/licenses/gpl-3.0.en.html

*/
pragma solidity >=0.4.21 <=0.5.12;

import "./mortal.sol";

contract Task is mortal {

    enum TaskStatus { Pending, Open, Closed, Charged, Cancelled }
    enum TaskState { Instantiated, Initialised }
    enum CashPolarity { Expense, Income }

    address targetConsortium;

    TaskState state;
    TaskStatus status;

    string taskCode;
    string activityCode;
    string description;
    string title;

    CashPolarity polarity;
    string unitOfMeasure;
    string unitOfCharge;

    uint actionOn;
    uint unitCharge;
    uint quantityOrdered;
    uint taxRate;
    uint quantityDelivered;

    event OnPriceChange(string taskCode);
    event OnRescheduled(string taskCode, uint quantity, uint actionOn);
    event OnDelivery(string taskCode, uint quantity, uint actionedOn);
    event OnStatusChange(string taskCode, TaskStatus status, uint actionedOn);

    //initialise
    constructor () public {
        state = TaskState.Instantiated;
        polarity = CashPolarity.Expense;
    }

    function Owner() public view returns (address eoa) { return owner; }

    function Initialise(string memory _taskCode, address _targetConsortium,
            CashPolarity _polarity, string memory _activityCode, string memory _description, string memory _title,
            TaskStatus _status, uint _actionOn, uint _unitCharge, uint _quantity, uint _taxRate,
            string memory _unitOfMeasure, string memory _unitOfCharge) public onlyOwner {
        require(state == TaskState.Instantiated, "Task header is already asigned");
        taskCode = _taskCode;
        targetConsortium = _targetConsortium;
        polarity = _polarity;
        activityCode = _activityCode;
        description = _description;
        title = _title;
        status = _status;
        actionOn = _actionOn;
        unitCharge = _unitCharge;
        quantityOrdered = _quantity;
        taxRate = _taxRate;
        quantityDelivered = 0;
        unitOfMeasure = _unitOfMeasure;
        unitOfCharge = _unitOfCharge;
        state = TaskState.Initialised;
    }

    function IsInitialised() public view returns (bool isInitialised) { return (state == TaskState.Initialised); }

    // ** properties **
    function GetHeader() public view returns
        (string memory _taskCode, address _targetConsortium, CashPolarity _polarity,
            string memory _activityCode, string memory _description, string memory _title,
            string memory _unitOfMeasure, string memory _unitOfCharge) {
        require (state == TaskState.Initialised, "Task not initialised!");
        _taskCode = taskCode;
        _targetConsortium = targetConsortium;
        _polarity = polarity;
        _activityCode = activityCode;
        _description = description;
        _title = title;
        _unitOfMeasure = unitOfMeasure;
        _unitOfCharge = unitOfCharge;
    }

    function GetSchedule() public view returns
        (TaskStatus _status, uint _actionOn, uint _unitCharge, uint _quantityOrdered, uint _taxRate, uint _quantityDelivered)
    {
        require (state == TaskState.Initialised, "Task not initialised!");
        _status = status;
        _actionOn = actionOn;
        _unitCharge = unitCharge;
        _quantityOrdered = quantityOrdered;
        _taxRate = taxRate;
        _quantityDelivered = quantityDelivered;
   }

    // ** process task **
    function PriceChange(uint _unitCharge, uint _taxRate) public onlyOwner {
        require (state == TaskState.Initialised, "Task not initialised");
        unitCharge = _unitCharge;
        taxRate = _taxRate;
        emit OnPriceChange(taskCode);
    }

    function Reschedule(uint _actionOn, uint _quantity) public onlyOwner {
        require (state == TaskState.Initialised, "Task not scheduled");
        actionOn = _actionOn;
        quantityOrdered = _quantity;
        emit OnRescheduled(taskCode, _quantity, _actionOn);
    }

    function Delivered(uint _actionOn, uint _quantity) public onlyOwner {
        require (state == TaskState.Initialised, "Task not scheduled");
        quantityDelivered += _quantity;
        emit OnDelivery(taskCode, _quantity, _actionOn);
    }

    function SetStatus(TaskStatus _status, uint _actionedOn) public onlyOwner  {
        require (state == TaskState.Initialised, "Task has no initial status");
        status = _status;
        emit OnStatusChange(taskCode, status, _actionedOn);
    }

}