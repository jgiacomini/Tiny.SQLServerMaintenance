namespace Tiny.SQLServerMaintenanceApp
{
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
