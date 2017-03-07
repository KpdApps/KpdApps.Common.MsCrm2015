using System;
using KpdApps.Common.MsCrm2015.Extensions;
using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015
{
    public abstract class BasePlugin : IPlugin
    {
        public readonly string UnsecureConfiguration;

        public readonly string SecureConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unsecureConfiguration"></param>
        /// <param name="secureConfiguration"></param>
        public BasePlugin(string unsecureConfiguration, string secureConfiguration)
        {
            UnsecureConfiguration = unsecureConfiguration;
            SecureConfiguration = secureConfiguration;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BasePlugin()
        {

        }

        public PluginState State { get; private set; }

        public void Execute(IServiceProvider serviceProvider)
        {
            State = CreatePluginState(serviceProvider);

            try
            {
                ExecuteInternal(State);
            }
            catch (Exception ex)
            {
                State?.TracingService.TraceError(ex);
                throw;
            }
        }

        public abstract void ExecuteInternal(PluginState state);

        public virtual PluginState CreatePluginState(IServiceProvider serviceProvider)
        {
            return new PluginState(serviceProvider);
        }
    }
}
