using System;

namespace KpdApps.Common.MsCrm2015
{
    public class BasePluginAction
    {
        public PluginState State { get; set; }

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
