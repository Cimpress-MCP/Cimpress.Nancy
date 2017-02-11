using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cimpress.Nancy.Components
{
    public interface IComponentManager
    {
        List<IBootstrapperExtender> GetBootstrapperExtenders();
        List<IVersionModuleExtender> GetModuleExtenders();
    }
}
