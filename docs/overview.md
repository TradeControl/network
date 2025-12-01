# Trading Network

Connecting [Trade Control](https://github.com/tradecontrol/sqlnode) nodes with [Ethereum](https://ethereum.org).

## Documentation

- [Network Installation](./installation.md)
- [Network Tutorial](./network.md)
- [Technical Spec](./tc_network_spec.md)
 
## Supply Chains

Whilst Trade Control works fine in stand-alone mode, the capability for each node to be connected in a network is intrinsic to the [schema design](https://github.com/tradecontrol/sqlnode). The network enables businesses to create private consortiums and public markets where borderless trading patterns are more-or-less instantly reflected over their supply-chains.

A network consists of connected nodes. To qualify as a node, outputs must be accepted recursively as inputs. In Trade Control, each business instance is called a node because it is designed to be connected to other instances in a network. For this to work, inputs must perfectly match outputs, so there can be no native customer or supplier accounts; no sales, purchase or works orders. Instead demand and supply present a mirror interface and order types are modelled using a principle of Cash Polarity.  Once nodes are connected, the network operates as though it is a single distributed world-wide trading platform.

The method by which Trade Control connects nodes is taken from the planning systems of the manufacturing industry sector, known as [allocations](./network.md#finally)

## Software Network

The Ethereum Virtual Machine (EVM) enables coders to write programs, called Smart Contracts, and execute them on a global peer-to-peer network running on the Ethereum blockchain. Implementing the Trade Control network on the EVM enables businesses to increase the efficiency of their operations in many ways, including:

1. Immutably and safely record transactions in Ethereum's public blockchain
2. Dispense with the traditional commercial communication protocols (legality aside)
3. Improve the integrity and responsiveness of the Company Statement
3. Deliver the necessary communications for supply-chain scheduling

## Currency

There is a minimal charge for using the EVM on the Ethereum Main Net, paid for in the utilities own currency called Eth. Its purpose is to prevent attackers consuming infinite CPU time on this publicly available world computer and pay fees to the network administrators (called miners). Although financial transactions in Ethereum are therefore built into the blockchain, they are not generally used by business because the platform is not a payment system for a digital currency. 

Ethereum supports HD Wallets. Therefore, now that we have the contracts and a network interface, it would be relatively straight forward to use Eth as a currency of exchange, or even, experimentally, a Unit of Account. We will not do that partly for the reasons stated, but also because Trade Control uses a [Bitcoin HD Wallet](https://github.com/tradecontrol/bitcoin) instead.

Because we are only interested in the P2P world computer dimensions of Ethereum, the Trade Control network [can be installed on any public blockchain](./installation.md#public-network) running an EVM outside the Main Net.  On these networks the ETH is free because it is without value. However, the Main Net is recommended for commercial use, since it provides security, persistence and universal connectivity.


