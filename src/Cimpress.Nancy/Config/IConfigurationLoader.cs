using System.Threading.Tasks;

namespace Cimpress.Nancy.Config
{
    public interface IConfigurationLoader
    {
        Task<T> LoadConfiguration<T>() where T : IConfiguration;
        string GetVersion();
    }
}