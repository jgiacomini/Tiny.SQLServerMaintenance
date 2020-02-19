using System.Collections.Generic;
using System.Linq;

namespace Tiny.SQLServerMaintenanceApp
{
    public class Fragmentation
    {
        private List<Index> _indexes;

        public Fragmentation(string schemaName, string tableName)
        {
            SchemaName = schemaName;
            TableName = tableName;
            _indexes = new List<Index>();
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public IEnumerable<Index> Indexes { get { return _indexes; } }

        public void Add(Index index)
        {
            _indexes.Add(index);
        }

        public double MaxFragmentation
        {
            get
            {
                if (Indexes == null || !Indexes.Any())
                {
                    return 0;
                }

                return Indexes.Max(f => f.FragmentationInPercent);
            }
        }
    }
}
