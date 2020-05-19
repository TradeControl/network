pragma solidity >=0.4.21 <=0.5.12;

import './owned.sol';

contract mortal is owned {
	// Contract destructor
	function destroy() public onlyOwner {
		selfdestruct(owner);
	}
}