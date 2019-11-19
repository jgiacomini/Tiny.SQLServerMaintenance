using GalaSoft.MvvmLight.CommandWpf;

namespace Tiny.SQLServerMaintenanceApp
{
    public class ServerModel : AppViewModelBase
    {
        private readonly Server _server;
        internal ServerModel(Server server)
        {
            _server = server;
            ServerName = server.ServerName;
            ConnectionString = server.ConnectionString;
            DeleteCommand = new RelayCommand(Delete);
        }

        internal Server GetData()
        {
            return _server;
        }

        public string ServerName { get; }
        public string ConnectionString { get; }
        public RelayCommand DeleteCommand { get; }

        private void Delete()
        {
            MessengerInstance.Send<DeleteServerMessage>(new DeleteServerMessage(this));
        }
    }
}
