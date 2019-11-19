using GalaSoft.MvvmLight.CommandWpf;

namespace Tiny.SQLServerMaintenanceApp
{
    public class ServerModel : AppViewModelBase
    {
        internal ServerModel(Server server)
        {
            ServerName = server.ServerName;
            ConnectionString = server.ConnectionString;
            DeleteCommand = new RelayCommand(Delete);
        }

        public string ServerName { get; private set; }
        public string ConnectionString { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        private void Delete()
        {
            MessengerInstance.Send<DeleteServerMessage>(new DeleteServerMessage(this));
        }
    }
}
