# Trade Control - Network

Connecting [Trade Control](https://github.com/tradecontrol/tc-nodecore) nodes with [Ethereum](https://github.com/ethereum/wiki/wiki).


## Overview

### Supply Chains

The capability for Trade Control to be connected in a [network](https://github.com/iamonnox/tradecontrol/blob/master/docs/tc_functions.md#networks) is intrinsic to the schema design. Its practical implementation allows businesses to create private consortiums and public markets where borderless trading patterns are more-or-less instantly reflected over their supply-chains.

A network consists of connected nodes. To qualify as a node, outputs must be accepted recursively as inputs, and Trade Control achieves this by modelling workflows in terms of polarity. This is explained in [the accompanying paper](https://github.com/iamonnox/tradecontrol/blob/master/docs/tc_functions.md#cash-polarity). Here we demonstrate how it works practically and implement a feature set that can be used successfully in your business. Once nodes are connected, the network operates as though it is a single distributed world-wide trading platform.

> **NOTE**
> 
> This facility is not necessary for you to start using [the app](https://github.com/tradecontrol/tc-office) in your business, because it also works fine in stand-alone mode.  

### Software Network

The Ethereum Virtual Machine (EVM) enables coders to write programs, called Contracts, and execute them on a global peer-to-peer network running on the Ethereum blockchain. Implementing the Trade Control network on the EVM enables businesses to increase the efficiency of their operations in many ways, including:

1. Immutably and safely record transactions in Ethereum's public blockchain
2. Dispense with the traditional commercial communication protocols (legality aside)
3. Improve the integrity and responsiveness of the [Company Statement](https://github.com/tradecontrol/tc-powerbi/blob/master/readme.md#statements)
3. Deliver the necessary communications for [supply-chain scheduling](https://github.com/iamonnox/tradecontrol/blob/master/docs/tc_functions.md#supply-and-demand)

### Currency

There is a minimal charge for using the EVM on the Ethereum Main Net, paid for in the utilities own currency called Eth. Its purpose is to prevent attackers consuming infinite CPU time on this publicly available world computer and pay fees to the network administrators (called miners). Although financial transactions in Ethereum are therefore built into the blockchain, they are not generally used by business because the platform is not a payment system for a digital currency. 

Ethereum supports HD Wallets. Therefore, now that we have the contracts and a network interface, it would be relatively straight forward to use Eth as a currency of exchange, or even, experimentally, a Unit of Account. We will not do that partly for the reasons stated, but also because Trade Control uses a [Bitcoin HD Wallet](https://github.com/tradecontrol/tc-bitcoin) instead.

Because we are only interested in the P2P world computer dimensions of Ethereum, the Trade Control network [can be installed on any public blockchain](#public-network) running an EVM outside the Main Net.  On these networks the ETH is free because it is without value.

## Documentation

- [Network Demo](docs/tc_network_demo.md)
- [Technical Spec](docs/tc_network_spec.md)

## Installation

For the latest changes and current version, consult the [Change Log](changelog.md)

### Dependencies

The Trade Control Network is serviced by changes to the following repositories:

- [tc-node](https://github.com/tradecontrol/tc-nodecore) >= 3.27.1
- [tc-office](https://github.com/tradecontrol/tc-office) >= 3.13.0

### Installer

[Network Interface Installation](src/installation/tcNetworkSetup.zip)

### Test Environment

The [Network Demo](docs/tc_network_demo.md) requires the following. 

To install the test environment:

- [NVM for Windows](https://github.com/nvm-sh/nvm)
- [Ganache](https://github.com/trufflesuite/ganache)
- Sql Server Instance (a locally authenticated server is best)
- Trade Control

### Development Environment

- Visual Studio with Sql Server components
- [Nethereum](https://github.com/Nethereum/Nethereum)
- [Truffle](https://www.trufflesuite.com) (for console-based contract interaction)
- Visual Studio Code + Solidity extension (contract compilation and c# interface)
- SSMS.

### Public Network

[Infura](https://infura.io/)

## Donations

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=C55YGUTBJ4N36)

## Licence

The Trade Control Code licence is issued by Trade Control Ltd under a [GNU General Public Licence v3.0](https://www.gnu.org/licenses/gpl-3.0.en.html) 

Trade Control Documentation by Trade Control Ltd is licenced under a [Creative Commons Attribution-ShareAlike 4.0 International License](http://creativecommons.org/licenses/by-sa/4.0/) 

![Creative Commons](https://i.creativecommons.org/l/by-sa/4.0/88x31.png) 

