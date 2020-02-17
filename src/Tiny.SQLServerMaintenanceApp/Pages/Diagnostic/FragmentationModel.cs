using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tiny.SQLServerMaintenanceApp
{
    public class FragmentationModel : ObservableObject
    {
        private readonly ObservableCollection<Index> _indexes;
        public FragmentationModel(Fragmentation statistiques)
        {
            SchemaName = statistiques.SchemaName;
            TableName = statistiques.TableName;
            _indexes = new ObservableCollection<Index>();

            foreach (var index in statistiques.Indexes)
            {
                _indexes.Add(new Index(index.IndexName, index.FragmentationInPercent));
            }
        }

        public string SchemaName { get; private set; }
        public string TableName { get; private set; }
        public IEnumerable<Index> Indexes { get { return _indexes; } }

        public void Add(Index index)
        {
            _indexes.Add(index);
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
}
