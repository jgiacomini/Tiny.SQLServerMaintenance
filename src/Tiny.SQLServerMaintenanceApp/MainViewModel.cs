using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;

namespace Tiny.SQLServerMaintenanceApp
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isBusy;
        private string _connectionString;
        public MainViewModel()
        {
            GetFragmentationCommand = new RelayCommand(GetFragmentation);
            _connectionString = AppSettings.Default.ConnectionString;
            Fragmentations = new ObservableCollection<FramgmentationModel>();
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
                    Fragmentations.Add(new FramgmentationModel(item));
                }
            }
            catch (System.Exception)
            {
            }

            IsBusy = false;
        }

        public ObservableCollection<FramgmentationModel> Fragmentations { get; private set; }

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
