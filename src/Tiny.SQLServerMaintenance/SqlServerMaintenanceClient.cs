using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private const string CompatibilityMode = "SELECT name, compatibility_level FROM sys.databases;";

        private const string ConnectionsDetailsSQL = "sp_who2 'Active'";
        private const string CompatibilityLvl = "SELECT compatibility_level FROM[sys].[databases]";
        private const string ChangeCompatibilityLvl = "ALTER DATABASE database_name SET COMPATIBILITY_LEVEL = 140;";

        //// private const string "select @@version as version";

        private readonly string _connectionString;

        public SqlServerMaintenanceClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Fragmentation>> GetFragmentationAsync(CancellationToken cancellationToken = default)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(FragmentationSQL, connection);
                command.CommandTimeout = (int)TimeSpan.FromDays(1).TotalSeconds;
                await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var result = new List<Fragmentation>();
                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var schemaName = ReadString(reader["SchemaName"]);
                            var tableName = ReadString(reader["TableName"]);
                            Fragmentation fragmentation = result.FirstOrDefault(f => f.SchemaName == schemaName && f.TableName == tableName);

                            if (fragmentation == null)
                            {
                                fragmentation = new Fragmentation(schemaName, tableName);
                                result.Add(fragmentation);
                            }

                            fragmentation.Add(new Index(ReadString(reader["IndexName"]), (double)reader["FragmentationInPercent"]));
                        }
                    }
                }

                return result;
            }
        }

        public async Task<int> RebuilFragmentationAsync(string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(FragmentationSQL, connection);
                command.CommandTimeout = (int)TimeSpan.FromDays(1).TotalSeconds;
                await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                // laisse 20% de place par page pour éviter que la fragmentation revienne vite
                command.CommandText = $"ALTER INDEX ALL ON [{schemaName}].[{tableName}] REBUILD WITH (FILLFACTOR = 80);";
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task<int> ReorganizeFragmentationAsync(string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(FragmentationSQL, connection);
                command.CommandTimeout = (int)TimeSpan.FromDays(1).TotalSeconds;
                await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                // laisse 20% de place par page pour éviter que la fragmentation revienne vite
                command.CommandText = $"ALTER INDEX ALL ON [{schemaName}].[{tableName}] REORGANIZE";
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        private static string ReadString(object value)
        {
            return value == DBNull.Value ? null : (string)value;
        }
    }
}
