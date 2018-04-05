using System;
using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015
{
    public class BasePluginAction : IPlugin
    {
        public PluginState State { get; set; }

        public IOrganizationService Service => State.Service;

        public IOrganizationService AdminService => State.AdminService;

        private int _order;
        public int Order => _order;

        public BasePluginAction(PluginState state, int order)
        {
            State = state;
            _order = order;
        }

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            State = new PluginState(serviceProvider);
            Execute();
        }
    }
}
