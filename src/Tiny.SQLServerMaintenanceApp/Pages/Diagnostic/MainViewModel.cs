using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;

namespace Tiny.SQLServerMaintenanceApp
{
    public class MainViewModel : AppViewModelBase
    {
        private string _connectionString;
        public MainViewModel()
        {
            GetFragmentationCommand = new RelayCommand(GetFragmentation);
            Fragmentations = new ObservableCollection<FragmentationModel>();
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
    }
}
