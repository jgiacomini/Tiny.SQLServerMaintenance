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
            Framgmentations = new ObservableCollection<FramgmentationModel>();
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
                Framgmentations.Clear();
                foreach (var item in fragmentations)
                {
                    Framgmentations.Add(new FramgmentationModel(item));
                }
            }
            catch (System.Exception)
            {
            }

            IsBusy = false;
        }

        public ObservableCollection<FramgmentationModel> Framgmentations { get; private set; }

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

    public class FramgmentationModel : ObservableObject
    {
        public FramgmentationModel(Statistiques statistiques)
        {
            FragmentationInPercent = statistiques.FragmentationInPercent;
            SchemaName = statistiques.SchemaName;
            TableName = statistiques.TableName;
            IndexName = statistiques.IndexName;
        }

        public double FragmentationInPercent { get; private set; }
        public string SchemaName { get; private set; }
        public string TableName { get; private set; }
        public string IndexName { get; private set; }
    }
}
