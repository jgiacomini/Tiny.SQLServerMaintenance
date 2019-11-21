using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace Tiny.SQLServerMaintenanceApp
{
    public class ChooseServerViewModel : AppViewModelBase
    {
        public ChooseServerViewModel()
        {
            var serversJson = AppSettings.Default.Servers;

            List<Server> servers = null;

            if (!string.IsNullOrWhiteSpace(serversJson))
            {
                servers = JsonSerializer.Deserialize<List<Server>>(serversJson);
            }

            if (servers == null)
            {
                servers = new List<Server>();
            }

            Servers = new ObservableCollection<ServerModel>();
            foreach (var item in servers)
            {
                Servers.Add(new ServerModel(item));
            }

            MessengerInstance.Register<DeleteServerMessage>(this, DeleteServer);
            AddCommand = new RelayCommand(Add);
        }

        public ObservableCollection<ServerModel> Servers { get; set; }

        private void DeleteServer(DeleteServerMessage deleteServerMessage)
        {
            Servers.Remove(deleteServerMessage.Server);
            SaveServers();
        }

        private void SaveServers()
        {
            AppSettings.Default.Servers = JsonSerializer.Serialize(Servers.Select(s => s.GetData()).ToList());
            AppSettings.Default.Save();
        }

        private string _serverName;
        public string ServerName
        {
            get
            {
                return _serverName;
            }
            set
            {
                Set(ref _serverName, value);
            }
        }

        private string _connectionString;
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

        private void Add()
        {
            Servers.Add(new ServerModel(new Server() { ConnectionString = ConnectionString, ServerName = ServerName }));
            SaveServers();
        }

        public RelayCommand AddCommand { get; }
    }
}
