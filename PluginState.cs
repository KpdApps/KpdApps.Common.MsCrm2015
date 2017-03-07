using System;
using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015
{
    public class PluginState
    {
        public virtual IServiceProvider Provider { get; private set; }

        private IPluginExecutionContext _context;
        public virtual IPluginExecutionContext Context => _context ?? (_context = (IPluginExecutionContext)Provider.GetService(typeof(IPluginExecutionContext)));

        private IOrganizationService _service;
        public virtual IOrganizationService Service
        {
            get
            {
                if (_service != null)
                    return _service;

                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)Provider.GetService(typeof(IOrganizationServiceFactory));
                _service = factory.CreateOrganizationService(this.Context.UserId);
                return _service;
            }
        }

        private ITracingService _tracingService;
        public virtual ITracingService TracingService
        {
            get
            {
                if (_tracingService != null)
                    return _tracingService;

                _tracingService = (ITracingService)Provider.GetService(typeof(ITracingService));
                return _tracingService;
            }
        }

        public PluginState(IServiceProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            Provider = provider;
        }

        public T GetTarget<T>() where T : class
        {
            if (Context.InputParameters.Contains("Target"))
            {
                return (T)Context.InputParameters["Target"];
            }

            return default(T); ;
        }
    }
}
