using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Nancy.Bootstrapper;

namespace Cimpress.Nancy.Components
{
    public class ComponentManager : IComponentManager
    {
        private readonly List<IBootstrapperExtender> _bootstrapperExtenders;
        private readonly List<IVersionModuleExtender> _moduleExtenders;

        public ComponentManager(IEnumerable<IBootstrapperExtender> bootstrapperExtenders,
            IEnumerable<IVersionModuleExtender> moduleExtenders)
        {
            _bootstrapperExtenders = bootstrapperExtenders.OrderBy(x => x.Priority).ToList();
            _moduleExtenders = moduleExtenders.OrderBy(x => x.Priority).ToList();
        }

        public List<IBootstrapperExtender> GetBootstrapperExtenders()
        {
            return _bootstrapperExtenders;
        }

        public List<IVersionModuleExtender> GetModuleExtenders()
        {
            return _moduleExtenders;
        }
    }
}
