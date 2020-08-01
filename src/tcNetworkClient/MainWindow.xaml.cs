#define PRODUCTION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using TradeControl.Node;

namespace TradeControl.Network
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region properties
        string NetworkProvider { get { return cbNetworkProvider.Text; } set { cbNetworkProvider.Text = value; } }
        string PublicKey { get { return tbPublicKey.Text; } set { tbPublicKey.Text = value; } }
        string PrivateKey { get { return tbPrivateKey.Password; } set { tbPrivateKey.Password = value; } }
        

        string Password { get { return pbPassword.Password; } }

        string ConsortiumAddress { get { return tbConsortiumAddress.Text; } set { tbConsortiumAddress.Text = value;  } }
        string NewAccountCode {  get { return cbCandidates.SelectedIndex >= 0 ? (string)cbCandidates.SelectedItem : string.Empty; } }
        string MemberAccountCode {  get { return cbMembers.SelectedIndex >= 0 ? (string)cbMembers.SelectedItem : string.Empty; } }
        bool IsAuthorised { get { return (bool)cbIsAuthorised.IsChecked; } set { cbIsAuthorised.IsChecked = value; } }
        string NewEOA {  get { return tbNewEOA.Text; } }
        string NewConsortium {  get { return tbNewConsortium.Text;  } }
        #endregion

        #region sql events
        private void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            SqlServerConnect();
        }

        private void BtnServers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                cbSqlServerName.Items.Clear();

                List<string> localServers = TCNodeConfig.SqlServers;

                if (localServers.Count > 0)
                {
                    foreach (string serverName in localServers)
                        cbSqlServerName.Items.Add(serverName);

                    if (cbSqlServerName.Text.Length == 0)
                        cbSqlServerName.Text = localServers[0];
                }
            }
            catch (Exception err)
            {
                lbConnectionStatus.Text = err.Message;
                lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void CbDatabaseName_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                cbDatabaseName.Items.Clear();

                TCNodeConfig tcnode = new TCNodeConfig(
                    this.SqlServerName,
                    this.Authentication,
                    this.SqlUserName,
                    this.DatabaseName,
                    this.Password
                    );

                List<string> localDatabases = tcnode.SqlDatabases;

                foreach (string database in localDatabases)
                    cbDatabaseName.Items.Add(database);
            }
            catch (Exception err)
            {
                lbConnectionStatus.Text = err.Message;
                lbConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void cbAuthenticationMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridCredentials != null)
            {
                gridCredentials.IsEnabled = this.Authentication == AuthenticationMode.SqlServer;
            }
        }
        #endregion

        #region eth events
        private void btnBalance_Click(object sender, RoutedEventArgs e)
        {
            ShowBalance();
        }

        private void useTransferGasPrice_Checked(object sender, RoutedEventArgs e)
        {
            transferGasPrice.IsEnabled = (bool) useTransferGasPrice.IsChecked;
            if (!(bool)useTransferGasPrice.IsChecked)
                useTransferGas.IsChecked = false;
        }

        private void useTransferGas_Checked(object sender, RoutedEventArgs e)
        {
            transferGas.IsEnabled = (bool)useTransferGas.IsChecked;
            if ((bool)useTransferGas.IsChecked)
                useTransferGasPrice.IsChecked = true;
        }

        private void btnTransferEth_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Okay to transfer {transferEth.Text} ETH to this account?", this.Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                TransferEth();
        }

        private void btnRpcConnect_Click(object sender, RoutedEventArgs e)
        {
            Web3Connect();
        }

        private void EOAtransfer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowBalance();
        }

        private void cbTransferEth_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowTransferEth = (bool)cbTransferEth.IsChecked;
            pageTransferEth.Visibility = Properties.Settings.Default.ShowTransferEth ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region form events
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowConnections();
                
                using (TCNodeConfig config = new TCNodeConfig())
                {
                    SqlServerName = config.SqlServerName;
                    Authentication = config.Authentication;
                    SqlUserName = config.SqlUserName;
#if PRODUCTION
                    DatabaseName = config.DatabaseName;
#endif
                }

#if PRODUCTION
                if (DatabaseName.Length > 0)
                    if (SqlServerConnect() && NetworkProvider.Length > 0)
                        Web3Connect();
#endif

                cbTransferEth.IsChecked = Properties.Settings.Default.ShowTransferEth;
                pageTransferEth.Visibility = Properties.Settings.Default.ShowTransferEth ? Visibility.Visible : Visibility.Collapsed;

                
            }
            catch (Exception err)
            {
                MessageBox.Show($"{err.Source}.{err.TargetSite.Name}: {err.Message}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #endregion

        #region consortium events
        private void btnDeployConsortium_Click(object sender, RoutedEventArgs e)
        {
            DeployConsortium();
        }

        private void tbConsortiumAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnDeployConsortium.IsEnabled = tbConsortiumAddress.Text.Length == 0;
        }

        private void SetAddMemberEnabled()
        {
            btnAddMember.IsEnabled = (cbCandidates.SelectedIndex >= 0) && (tbNewEOA.Text.Length > 0) && (tbNewConsortium.Text.Length > 0);
        }

        private void cbCandidates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAddMemberEnabled();
        }

        private void tbNewAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetAddMemberEnabled();
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            NewMembership();
        }

        private void cbMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMembers.SelectedIndex >= 0)
                GetMemberConsortium();
        }

        private void btnAuthorise_Click(object sender, RoutedEventArgs e)
        {
            if (cbMembers.SelectedIndex >= 0)
                SetAuthorisation();
        }

        private void cbNetworkProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadNetworkProvider();
        }

        private void btnLoadKeys_Click(object sender, RoutedEventArgs e)
        {
            LoadNetworkProvider();
        }

        private async void btnSaveKeys_Click(object sender, RoutedEventArgs e)
        {
            if (await SaveNetworkProvider())
                MessageBox.Show($"Keys written to {DatabaseName}", Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
#endregion

#region watcher
        private void PassiveAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Active Mode is not currenctly supported", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);

            /*
            if (btnStart.IsEnabled && PassiveAccounts.SelectedIndex >= 0)
            {
                ActiveAccounts.Items.Add(PassiveAccounts.SelectedItem);
                PassiveAccounts.Items.Remove(PassiveAccounts.SelectedItem);
            }
            */
        }

        private void ActiveAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*
            if (btnStart.IsEnabled && ActiveAccounts.SelectedIndex >= 0)
            {
                PassiveAccounts.Items.Add(ActiveAccounts.SelectedItem);
                ActiveAccounts.Items.Remove(ActiveAccounts.SelectedItem);
            }
            */
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartWatching();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopWatching();
        }



        #endregion

    }
}
