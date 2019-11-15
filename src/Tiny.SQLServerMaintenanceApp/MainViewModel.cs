using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Tiny.SQLServerMaintenanceApp
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isBusy;
        private string _connectionString;
        public MainViewModel()
        {
            GetFragmentationCommand = new RelayCommand(GetFragmentation);
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                Set(ref _connectionString, value);
            }
        }

        private async void GetFragmentation()
        {
            var sqlServerMaintenanceClient = new SqlServerMaintenanceClient(ConnectionString);

            IsBusy = true;
            try
            {
                var framentation = await sqlServerMaintenanceClient.GetFragmentationAsync();
            }
            catch (System.Exception)
            {
            }

            IsBusy = false;
        }

        public RelayCommand GetFragmentationCommand { get; }
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                Set(ref _isBusy, value);
            }
        }
    }
}
