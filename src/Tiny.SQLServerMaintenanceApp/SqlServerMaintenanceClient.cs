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
        private const string SQL = @"SELECT avg_fragmentation_in_percent AS FragmentationInPercent,
                OBJECT_SCHEMA_NAME(Stats.object_id) AS SchemaName,
                OBJECT_NAME (Stats.object_id) AS TableName,
                Indexes.name IndexName
                FROM sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID('dbo.T_CLIENT_CLI') , NULL, NULL , 'LIMITED') AS Stats
                INNER JOIN sys.indexes AS Indexes ON Stats.object_id = Indexes.object_id AND Stats.index_id = Indexes.index_id
                ORDER BY avg_fragmentation_in_percent DESC";

        private readonly string _connectionString;

        public SqlServerMaintenanceClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Statistiques>> GetFragmentationAsync(CancellationToken cancellationToken = default)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(SQL, connection);
                await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                List<Statistiques> result = new List<Statistiques>();
                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            result.Add(new Statistiques()
                            {
                                FragmentationInPercent = (double)reader["FragmentationInPercent"],
                                SchemaName = reader["SchemaName"] == DBNull.Value ? null : (string)reader["SchemaName"],
                                TableName = (string)reader["TableName"],
                                IndexName = (string)reader["IndexName"],
                            });
                        }
                    }
                }

                return result;
            }

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
