using System.Collections.Generic;
using System.Linq;

namespace Tiny.SQLServerMaintenanceApp
{
    public class Fragmentation
    {
        private List<Index> _indices;

        public Fragmentation(string schemaName, string tableName)
        {
            SchemaName = schemaName;
            TableName = tableName;
            _indices = new List<Index>();
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public IEnumerable<Index> Indexes { get { return _indices; } }

        public void Add(Index index)
        {
            _indices.Add(index);
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

    public class Index
    {
        public Index(string indexName, double fragmentationInPercent)
        {
            IndexName = indexName;
            FragmentationInPercent = fragmentationInPercent;
        }

        public string IndexName { get; private set; }
        public double FragmentationInPercent { get; private set; }
    }
}
