namespace Services.Aid.Settings
{
    public interface IDatabaseSettings
    {
        public string HumaneAidCollectionName { get; set; }
        public string BasisAidCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
