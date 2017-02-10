using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;

namespace Cimpress.Nancy.Components
{
    public class ComponentRegistrations : Registrations
    {
        public ComponentRegistrations(ITypeCatalog typeCatalog) : base(typeCatalog)
        {
            RegisterAll<IBootstrapperExtender>();
            RegisterAll<IVersionModuleExtender>();
            RegisterWithDefault<IComponentManager>(typeof(ComponentManager));
            RegisterWithDefault<INancyLogger>(typeof(DefaultNancyLogger));
        }
    }
}
