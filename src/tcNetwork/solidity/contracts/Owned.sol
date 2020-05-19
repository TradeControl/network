// Version of Solidity compiler this program was written for
pragma solidity >=0.4.21 <=0.5.12;

contract owned {
	address payable owner;
	// Contract constructor: set owner
	constructor() public {
		owner = msg.sender;
	}
	// Access control modifier
	modifier onlyOwner {
	    require(msg.sender == owner, "Only the contract owner can call this function");
	    _;
	}
}