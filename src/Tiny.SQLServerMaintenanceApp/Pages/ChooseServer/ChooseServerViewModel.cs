using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                JsonSerializer.Deserialize<List<Server>>(serversJson);
            }

            if (servers == null)
            {
                servers = new List<Server>();
            }

            foreach (var item in servers)
            {
                Servers.Add(new ServerModel(item));
            }

            MessengerInstance.Register<DeleteServerMessage>(this, DeleteServer);
        }

        public ObservableCollection<ServerModel> Servers { get; set; }

        private void DeleteServer(DeleteServerMessage deleteServerMessage)
        {
            Servers.Remove(deleteServerMessage.Server);
        }
    }
}
