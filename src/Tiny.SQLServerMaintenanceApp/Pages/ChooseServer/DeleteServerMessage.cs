namespace Tiny.SQLServerMaintenanceApp
{
    public class DeleteServerMessage
    {
        public DeleteServerMessage(ServerModel server)
        {
            Server = server;
        }

        public ServerModel Server { get; }
    }
}
