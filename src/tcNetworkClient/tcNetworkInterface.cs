using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.Data.Linq;

using TradeControl.Node;

namespace TradeControl.Network
{
    public partial class MainWindow : Window
    {
        static TCWeb3 web3Watcher;

        EthEventHandler watchEvent;
        EthEventHandler watchTransaction;
        EthEventHandler watchCloseError;

        bool isDbConnected = false;
        bool isWeb3Connected = false;

        private void ShowConnections()
        {
            const string isEmpty = "_";

            lbConnections.Content = string.Format(Properties.Resources.Connections,
                    DatabaseName.Length > 0 ? DatabaseName : isEmpty,
                    NetworkProvider.Length > 0 ? NetworkProvider : isEmpty);
        }

        #region sql
        private bool SqlServerConnect()
        {
            try
            {
                Cursor = Cursors.Wait;
               
                string connection = string.Empty;

                StopWatching();
                isDbConnected = false;

                using (TCNodeConfig tcnode = new TCNodeConfig(
                        SqlServerName,
                        Authentication,
                        SqlUserName,
                        DatabaseName,
                        Password))
                {
                    if (tcnode.Authenticated)
                    {
                        if (tcnode.IsTCNode)
                        {
                            lbNodeVersion.Content = tcnode.InstalledVersion;

                            if (tcnode.InstalledVersion < TCWeb3.NetworkNodeVersion)
                            {
                                lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
                                lbConnectionStatus.Text = string.Format(Properties.Resources.NodeVersionIncompatible, TCWeb3.NetworkNodeVersion.ToString());
                                lbNodeIncompatibleWarning.Content = lbConnectionStatus.Text;
                                lbNodeIncompatibleWarning.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Blue);
                                lbConnectionStatus.Text = string.Format(Properties.Resources.ConnectionSucceeded, DatabaseName);
                                lbNodeIncompatibleWarning.Visibility = Visibility.Hidden;
                                TransactionList.Items.Clear();
                                EventList.Items.Clear();
                                isDbConnected = true;
                                connection = tcnode.ConnectionString;                                
                            }
                        }
                        else
                        {
                            lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
                            lbConnectionStatus.Text = Properties.Resources.UnrecognisedDatasource;
                        }
                    }
                    else
                    {
                        lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
                        lbConnectionStatus.Text = Properties.Resources.ConnectionFailed;
                    }
                }

                cbMembers.Items.Clear();
                cbCandidates.Items.Clear();
                btnAddMember.IsEnabled = false;
                ActiveAccounts.Items.Clear();
                PassiveAccounts.Items.Clear();

                if (isDbConnected)
                {
                    NetworkProvider = string.Empty;
                    PublicKey = string.Empty;
                    PrivateKey = string.Empty;
                    ConsortiumAddress = string.Empty;
                    tbMemberEOA.Text = string.Empty;
                    tbMemberConsortium.Text = string.Empty;

                    TCNodeNetwork nodeNetwork = new TCNodeNetwork(connection);
                    
                    foreach (var member in nodeNetwork.Members)
                    {
                        cbMembers.Items.Add(member.Key);
                        PassiveAccounts.Items.Add(member.Key);
                    }

                    foreach (var candidate in nodeNetwork.Candidates)
                        cbCandidates.Items.Add(candidate.Key);

                    Title = nodeNetwork.NetworkName;

                    var providers = (from tb in nodeNetwork.tbEths select tb.NetworkProvider).ToList<string>();
                    cbNetworkProvider.Items.Clear();
                    foreach (string provider in providers)
                    {
                        cbNetworkProvider.Items.Add(provider);
                        if (NetworkProvider.Length == 0)
                            NetworkProvider = provider;
                    }
                }
            }
            catch (Exception err)
            {
                lbConnectionStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
            finally
            {
                ShowConnections();
                Cursor = Cursors.Arrow;
            }

            return isDbConnected;

        }

        private string NodeConnectionString
        {
            get
            {
                try
                {
                    TCNodeConfig tcNode = new TCNodeConfig(SqlServerName, Authentication, SqlUserName, DatabaseName, Password);
                    return tcNode.ConnectionString;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return string.Empty;
                }
            }
        }

        private void LoadNetworkProvider()
        {
            try
            {
                TCNodeNetwork nodeNetwork = new TCNodeNetwork(NodeConnectionString);
                var provider = (from tb in nodeNetwork.tbEths where tb.NetworkProvider == cbNetworkProvider.Text select tb).FirstOrDefault();
                if (provider != null)
                {
                    PublicKey = provider.PublicKey;
                    PrivateKey = provider.PrivateKey;
                    ConsortiumAddress = provider.ConsortiumAddress;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region sql properties
        AuthenticationMode Authentication
        {
            get
            {
                return (AuthenticationMode)cbAuthenticationMode.SelectedIndex;
            }
            set
            {
                cbAuthenticationMode.SelectedIndex = (int)value;
            }
        }

        private string SqlServerName
        {
            get
            {
                return cbSqlServerName.Text;
            }
            set
            {
                cbSqlServerName.Text = value;
            }
        }

        private string DatabaseName
        {
            get
            {
                return cbDatabaseName.Text;
            }
            set
            {
                cbDatabaseName.Text = value;
            }
        }
        private string SqlUserName
        {
            get
            {
                return tbSqlUserName.Text;
            }
            set
            {
                tbSqlUserName.Text = value;
            }
        }      

        #endregion

        #region web3
        private async void Web3Connect()
        {
            try
            {
                Cursor = Cursors.Wait;

                StopWatching();

                isWeb3Connected = false;

                List<string> accounts = new List<string>();

                EOAtransfer.Items.Clear();

                using (TCWeb3 web3 = new TCWeb3(NetworkProvider))
                {
                    accounts = await web3.Accounts();

                    if (PublicKey.Length == 0)
                        PublicKey = await web3.Coinbase(); 
                }

                if (Properties.Settings.Default.ShowTransferEth)
                {
                    foreach (string account in accounts)
                    {
                        if (account.ToUpper() != PublicKey.ToUpper())
                            EOAtransfer.Items.Add(account);
                    }
                }

                btnDeployConsortium.IsEnabled = (ConsortiumAddress.Length == 0);                
                networkStatus.Text = string.Format(Properties.Resources.ConnectionSucceeded, NetworkProvider);
                networkStatus.Foreground = new SolidColorBrush(Colors.Blue);                

                isWeb3Connected = await SaveNetworkProvider(); 
            }
            catch (Exception err)
            {
                btnDeployConsortium.IsEnabled = false;
                btnStart.IsEnabled = false;
                networkStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                networkStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
            finally
            {
                Cursor = Cursors.Arrow;
                ShowConnections();
            }
        }

        private async Task<bool> SaveNetworkProvider()
        {
            try
            {
                using (TCNodeNetwork network = new TCNodeNetwork(NodeConnectionString))
                {
                    return await network.AddNetworkProvider(NetworkProvider, PublicKey, PrivateKey, ConsortiumAddress);
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void ShowBalance()
        {
            try
            {
                TCWeb3 web3 = new TCWeb3(NetworkProvider);

                decimal yourBalance = await web3.GetAccountBalance(PublicKey);
                var theirBalance = await web3.GetAccountBalance(EOAtransfer.Text);
                accountBalances.Text = string.Format(Properties.Resources.AccountBalances, Math.Round(yourBalance, 2), Math.Round(theirBalance, 1));
            }
            catch (Exception err)
            {
                transferStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                transferStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private async void TransferEth()
        {
            try
            {
                if (PrivateKey.Length == 0)
                {
                    transferStatus.Text = Properties.Resources.PrivateKeyRequired;
                    transferStatus.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }

                using (TCWeb3 web3 = new TCWeb3(NetworkProvider, PublicKey, PrivateKey))
                {
                    bool setGasPrice = (bool)useTransferGasPrice.IsChecked;
                    bool setGas = (bool)useTransferGas.IsChecked;
                    decimal amount = decimal.Parse(transferEth.Text);
                    decimal gasPrice = decimal.Parse(transferGasPrice.Text);
                    long gas = long.Parse(transferGas.Text);
                    string transactionHash;

                    decimal yourBalance = await web3.GetAccountBalance(PublicKey);

                    if (yourBalance < amount)
                        transferStatus.Text = string.Format(Properties.Resources.InsufficientFunds, amount);
                    else
                    {
                        if (setGasPrice && !setGas)
                            transactionHash = await web3.TransferEther(EOAtransfer.Text, amount, gasPrice);
                        else if (setGasPrice && setGas)
                            transactionHash = await web3.TransferEther(EOAtransfer.Text, amount, gasPrice, gas);
                        else
                            transactionHash = await web3.TransferEther(EOAtransfer.Text, amount);

                        transferStatus.Text = string.Format(Properties.Resources.FundsTransfered, amount, transactionHash);
                        transferStatus.Foreground = new SolidColorBrush(Colors.Blue);
                        ShowBalance();
                    }
                }
            }
            catch (Exception err)
            {
                transferStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                transferStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        #endregion

        #region consortium
        async void DeployConsortium()
        {
            try
            {
                if (isDbConnected)
                {
                    using (TCWeb3 web3 = new TCWeb3(NetworkProvider, PublicKey, PrivateKey))
                    {
                        web3.OnEthTransaction += Web3_OnEthTransaction;
                        ConsortiumAddress = await web3.DeployConsortium();
                        btnDeployConsortium.IsEnabled = ConsortiumAddress.Length == 0;
                    }

                    await SaveNetworkProvider();
                }
            }
            catch (Exception err)
            {
                networkStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                networkStatus.Foreground = new SolidColorBrush(Colors.Red);
            }

        }

        async void NewMembership()
        {
            try
            {
                using (TCWeb3 web3 = new TCWeb3(NetworkProvider, PublicKey, PrivateKey, ConsortiumAddress))
                {
                    web3.OnEthTransaction += Web3_OnEthTransaction;
                    web3.TCNode = new TCNodeNetwork(NodeConnectionString);
                    string accountCode = NewAccountCode;
                    var result = await web3.AddMember(accountCode, NewEOA, NewConsortium);
                    if (result)
                    {
                        cbMembers.Items.Add(accountCode);
                        cbMembers.SelectedItem = accountCode;
                        cbCandidates.Items.Remove(accountCode);
                        PassiveAccounts.Items.Add(accountCode);
                        tbNewConsortium.Text = string.Empty;
                        tbNewEOA.Text = string.Empty;
                    }
                    else
                        MessageBox.Show("New membership application has failed", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception err)
            {
                networkStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                networkStatus.Foreground = new SolidColorBrush(Colors.Red);
            }

        }

        async void GetMemberConsortium()
        {
            try
            {
                using (TCWeb3 web3 = new TCWeb3(NetworkProvider, PublicKey, PrivateKey, ConsortiumAddress))
                {
                    tbMemberConsortium.Text = await web3.GetConsortium(MemberAccountCode);
                    tbMemberEOA.Text = await web3.GetEOA(MemberAccountCode);
                    cbIsAuthorised.IsChecked = await web3.GetAuthorisation(MemberAccountCode);
                }
            }
            catch (Exception err)
            {
                networkStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                networkStatus.Foreground = new SolidColorBrush(Colors.Red);
            }

        }

        async void SetAuthorisation()
        {
            try
            {
                using (TCWeb3 web3 = new TCWeb3(NetworkProvider, PublicKey, PrivateKey))
                {
                    web3.OnEthTransaction += Web3_OnEthTransaction;
                    bool isAuthorised = await web3.GetAuthorisation(MemberAccountCode);
                    var result = await web3.SetAuthorisation(MemberAccountCode, !isAuthorised);
                    if (result.Length > 0)
                        IsAuthorised = !isAuthorised;

                        
                }

            }
            catch (Exception err)
            {
                networkStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                networkStatus.Foreground = new SolidColorBrush(Colors.Red);
            }

        }
        #endregion

        #region watcher
        async void StartWatching()
        {
            try
            { 
                if (isDbConnected && isWeb3Connected)
                {
                    web3Watcher = new TCWeb3(NetworkProvider, PublicKey, PrivateKey, ConsortiumAddress);
                    web3Watcher.TCNode = new TCNodeNetwork(NodeConnectionString);
                    web3Watcher.OnEthEvent += new EthEventHandler(Web3_OnEthEvent);
                    web3Watcher.OnEthTransaction += new EthEventHandler(Web3_OnEthTransaction);
                    web3Watcher.OnWatchEvent += new EthEventHandler(Web3Watcher_OnWatchEvent);
                    web3Watcher.OnWatchTransaction += new EthEventHandler(Web3Watcher_OnWatchTransaction);
                    web3Watcher.OnWatchCloseError += new EthEventHandler(Web3Watcher_OnWatchCloseError);

                    watchEvent = new EthEventHandler(WatchEvent);
                    watchTransaction = new EthEventHandler(WatchTransaction);
                    watchCloseError = new EthEventHandler(WatchCloseError);

                    foreach (string accountCode in PassiveAccounts.Items)
                        web3Watcher.AddPassiveAccount(accountCode);

                    foreach (string accountCode in ActiveAccounts.Items)
                        web3Watcher.AddActiveAccount(accountCode);

                    bool isStarted = await web3Watcher.StartWatching();
                    if (isStarted)
                    { 
                        btnStart.IsEnabled = false;
                        btnStop.IsEnabled = true;
                        networkState.IsIndeterminate = true;
                        networkState.Visibility = Visibility.Visible;
                        tabsMain.SelectedItem = pageTransactions;
                    }
                    else
                        MessageBox.Show("Start failure. Consult the events for details", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err)
            {
                networkStatus.Text = $"{err.Source}.{err.TargetSite.Name}: {err.Message}";
                networkStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void Web3Watcher_OnWatchCloseError(EthEventArgs e)
        {
            Dispatcher.BeginInvoke(watchCloseError, new object[] { e });
        }

        void WatchCloseError(EthEventArgs e)
        {
            AddEvent(e);
            StopWatching();
        }

        void StopWatching()
        {
            web3Watcher?.StopWatching();
            web3Watcher?.Dispose();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            networkState.IsIndeterminate = false;
            networkState.Visibility = Visibility.Hidden;
        }
        #endregion

        #region log
        void AddTransaction(EthEventArgs e)
        {
            TransactionList.Items.Add($"{e.TransactedOn}: {e.Description} [{e.Tag}]");
        }

        void AddEvent(EthEventArgs e)
        {
            EventList.Items.Add($"{e.TransactedOn}: {e.Description} [{e.Tag}]");
        }

        private void Web3Watcher_OnWatchTransaction(EthEventArgs e)
        {
            Dispatcher.BeginInvoke(watchTransaction, new object[] { e });
        }

        private void Web3Watcher_OnWatchEvent(EthEventArgs e)
        {
            Dispatcher.BeginInvoke(watchEvent, new object[] { e });
        }

        void WatchTransaction(EthEventArgs e)
        {
            AddTransaction(e);
        }

        void WatchEvent(EthEventArgs e)
        {
            AddEvent(e);
        }

        private void Web3_OnEthTransaction(EthEventArgs e)
        {
            AddTransaction(e);
        }


        private void Web3_OnEthEvent(EthEventArgs e)
        {
            AddEvent(e); 
        }
        #endregion
    }
}
