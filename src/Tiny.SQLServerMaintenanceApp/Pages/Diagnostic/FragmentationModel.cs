using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tiny.SQLServerMaintenanceApp
{
    public class FragmentationModel : ObservableObject
    {
        private readonly ObservableCollection<Index> _indices;
        public FragmentationModel(Fragmentation statistiques)
        {
            SchemaName = statistiques.SchemaName;
            TableName = statistiques.TableName;
            _indices = new ObservableCollection<Index>();
            _indices.Add(new Index(statistiques.IndexName, statistiques.FragmentationInPercent));
        }

        public string SchemaName { get; private set; }
        public string TableName { get; private set; }
        public IEnumerable<Index> Indexes { get { return _indices; } }

        public void Add(Index index)
        {
            _indices.Add(index);
            RaisePropertyChanged(nameof(MaxFragmentation));
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
