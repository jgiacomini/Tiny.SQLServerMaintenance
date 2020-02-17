namespace Tiny.SQLServerMaintenanceApp
{
    public class IndexModel
    {
        public IndexModel(string indexName, double fragmentationInPercent)
        {
            IndexName = indexName;
            FragmentationInPercent = fragmentationInPercent;
        }

        public string IndexName { get; private set; }
        public double FragmentationInPercent { get; private set; }
    }
}
