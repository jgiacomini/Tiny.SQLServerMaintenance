using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tiny.SQLServerMaintenanceApp
{
    public class MainViewModel : AppViewModelBase
    {
        private string _connectionString;
        public MainViewModel()
        {
            GetFragmentationCommand = new RelayCommand(GetFragmentation);
            FixFragmentationCommand = new RelayCommand(FixFragmentation);
            Fragmentations = new ObservableCollection<FragmentationModel>();

            // AppSettings.Default.ConnectionString = "Server=tcp:nu-mini-db-server-ci.database.windows.net,1433;Initial Catalog=nu-mini-db-prod_Copy_temoin;Persist Security Info=False;User ID=numiniadmin;Password=bjs8974OOFezofi2142vn;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
            _connectionString = AppSettings.Default.ConnectionString;
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                if (Set(ref _connectionString, value))
                {
                    AppSettings.Default.ConnectionString = value;
                    AppSettings.Default.Save();
                }
            }
        }

        private async void GetFragmentation()
        {
            var sqlServerMaintenanceClient = new SqlServerMaintenanceClient(ConnectionString);

            IsBusy = true;
            try
            {
                var fragmentations = await sqlServerMaintenanceClient.GetFragmentationAsync();
                Fragmentations.Clear();
                foreach (var item in fragmentations)
                {
                    Fragmentations.Add(new FragmentationModel(item));
                }
            }
            catch (System.Exception)
            {
            }

            IsBusy = false;
        }

        public ObservableCollection<FragmentationModel> Fragmentations { get; private set; }

        public RelayCommand GetFragmentationCommand { get; }

        public RelayCommand FixFragmentationCommand { get; }

        private async void FixFragmentation()
        {
            var sqlServerMaintenanceClient = new SqlServerMaintenanceClient(ConnectionString);
            IsBusy = true;

            foreach (var fragmentation in Fragmentations)
            {
                if (fragmentation.MaxFragmentation > 50)
                {
                    await sqlServerMaintenanceClient.RebuilFragmentationAsync(fragmentation.SchemaName, fragmentation.TableName).ConfigureAwait(false);
                }
                else if (fragmentation.MaxFragmentation > 30)
                {
                    await sqlServerMaintenanceClient.ReorganizeFragmentationAsync(fragmentation.SchemaName, fragmentation.TableName).ConfigureAwait(false);
                }
            }

            IsBusy = false;
        }
    }
}