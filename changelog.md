# Change Log

The following record logs changes to the full release of the Trade Control network. Checked items are included in the latest master commit.

## 1.1.0

First release May 2020

## 1.2.0

Integration with the [Trade Control bitcoin](https://github.com/tradecontrol/bitcoin) payment system.

- [x] [invoice contract](src/tcNetwork/solidity/contracts/Invoice.sol) payment address and assignment event
- [x] communicate the bitcoin payment address for invoiced receipts
- [x] write the bitcoin address for paying invoice mirrors

## 1.2.1

- [x] Project re-name
- [x] fix - replace task view with table access in TCWeb3.InvoiceDeploymentDetails()