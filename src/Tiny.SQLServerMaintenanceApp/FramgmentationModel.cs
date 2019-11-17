using GalaSoft.MvvmLight;

namespace Tiny.SQLServerMaintenanceApp
{
    public class FramgmentationModel : ObservableObject
    {
        public FramgmentationModel(Statistiques statistiques)
        {
            FragmentationInPercent = statistiques.FragmentationInPercent;
            SchemaName = statistiques.SchemaName;
            TableName = statistiques.TableName;
            IndexName = statistiques.IndexName;
        }

        public double FragmentationInPercent { get; private set; }
        public string SchemaName { get; private set; }
        public string TableName { get; private set; }
        public string IndexName { get; private set; }
    }
}
