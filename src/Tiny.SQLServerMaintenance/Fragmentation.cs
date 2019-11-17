namespace Tiny.SQLServerMaintenanceApp
{
    public class Fragmentation
    {
        public double FragmentationInPercent { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
    }
}
