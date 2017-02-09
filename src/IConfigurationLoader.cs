namespace Cimpress.Nancy
{
    public interface IConfigurationLoader
    {
        T LoadConfiguration<T>(T configuration) where T : IConfiguration;
        string GetVersion();
    }
}