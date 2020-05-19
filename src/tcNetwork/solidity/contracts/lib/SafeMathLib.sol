pragma solidity >=0.4.21 <=0.5.12;

library SafeMathLib {
  function times(uint a, uint b) internal pure returns (uint) {
    uint c = a * b;
    assert(a == 0 || c / a == b);
    return c;
  }

  function minus(uint a, uint b) internal pure returns (uint) {
    assert(b <= a);
    return a - b;
  }

  function plus(uint a, uint b) internal pure returns (uint) {
    uint c = a + b;
    assert(c>=a && c>=b);
    return c;
  }
}
