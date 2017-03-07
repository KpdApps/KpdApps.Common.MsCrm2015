using System;
using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015
{
    public class BasePluginAction
    {
        public PluginState State { get; set; }

        public IOrganizationService Service => State.Service;

        public BasePluginAction(PluginState state)
        {
            State = state;
        }

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
