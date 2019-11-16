using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tiny.SQLServerMaintenanceApp
{
    public class SqlServerMaintenanceClient
    {
        private const string FragmentationSQL = @"SELECT avg_fragmentation_in_percent AS FragmentationInPercent,
                OBJECT_SCHEMA_NAME(Stats.object_id) AS SchemaName,
                OBJECT_NAME (Stats.object_id) AS TableName,
                Indexes.name IndexName
                FROM sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID('dbo.T_CLIENT_CLI') , NULL, NULL , 'LIMITED') AS Stats
                INNER JOIN sys.indexes AS Indexes ON Stats.object_id = Indexes.object_id AND Stats.index_id = Indexes.index_id
                ORDER BY avg_fragmentation_in_percent DESC";

        private const string NbConnectionsSQL = @"SELECT 
                DB_NAME(dbid) as DBName, 
                COUNT(dbid) as NumberOfConnections,
                loginame as LoginName
            FROM
                sys.sysprocesses
            WHERE 
                dbid > 0
            GROUP BY 
                dbid, loginame";

        private const string ConnectionsDetailsSQL = "sp_who2 'Active'";

        private readonly string _connectionString;

        public SqlServerMaintenanceClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Statistiques>> GetFragmentationAsync(CancellationToken cancellationToken = default)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(FragmentationSQL, connection);
                command.CommandTimeout = (int)TimeSpan.FromDays(1).TotalSeconds;
                await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var result = new List<Statistiques>();
                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            result.Add(new Statistiques()
                            {
                                FragmentationInPercent = (double)reader["FragmentationInPercent"],
                                SchemaName = ReadString(reader["SchemaName"]),
                                TableName = ReadString(reader["TableName"]),
                                IndexName = ReadString(reader["IndexName"]),
                            });
                        }
                    }
                }

                return result;
            }
        }

        private static string ReadString(object value)
        {
            return value == DBNull.Value ? null : (string)value;
        }
    }

    public class Statistiques
    {
        public double FragmentationInPercent { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
    }
}
