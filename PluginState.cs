using System;
using Microsoft.Xrm.Sdk;
using KpdApps.Common.MsCrm2015.Extensions;

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
        private IOrganizationService _adminService;
        public virtual IOrganizationService AdminService
        {
            get
            {
                if (_adminService != null)
                    return _adminService;

                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)Provider.GetService(typeof(IOrganizationServiceFactory));
                _adminService = factory.CreateOrganizationService(null);
                return _adminService;
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

            return default(T);
        }

        public Entity TryGetPostImage(string name)
        {
            if (Context.PostEntityImages.Contains(name))
                return Context.PostEntityImages[name];

            return null;
        }

        public Entity GetPostImage(string name)
        {
            var image = TryGetPostImage(name);
            if (image == null)
                throw new ApplicationException($"Post-image with name \"{name}\" not found.");

            return image;
        }

        public Entity TryGetPreImage(string name)
        {
            if (Context.PreEntityImages.Contains(name))
                return Context.PreEntityImages[name];

            return null;
        }

        public Entity GetPreImage(string name)
        {
            var image = TryGetPreImage(name);
            if (image == null)
                throw new ApplicationException($"Pre-image with name \"{name}\" not found.");

            return image;
        }

        public EntityReference GetEntityMoniker()
        {
            if (Context.InputParameters.Contains("EntityMoniker"))
            {
                return (EntityReference)Context.InputParameters["EntityMoniker"];
            }

            throw new ApplicationException("EntityMoniker not found.");
        }

        public Guid Log(string message)
        {
            string logicalName = string.Empty;
            Guid id = Guid.Empty;

            if (Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity)
            {
                Entity target = (Entity)Context.InputParameters["Target"];
                logicalName = target.LogicalName;
                id = target.Id;
            }
            else if (Context.InputParameters.Contains("EntityMoniker") && Context.InputParameters["EntityMoniker"] is EntityReference)
            {
                EntityReference moniker = (EntityReference)Context.InputParameters["EntityMoniker"];
                logicalName = moniker.LogicalName;
                id = moniker.Id;
            }

            Entity logMessage = new Entity(Schema.LogMessage.LogicalName);

            if (!string.IsNullOrEmpty(logicalName))
            {
                logMessage.Attributes.SetStringValue(Schema.LogMessage.ObjectName, logicalName);
            }

            if (id != Guid.Empty)
            {
                logMessage.Attributes.SetStringValue(Schema.LogMessage.ObjectId, id.ToString());
            }

            logMessage.Attributes.SetStringValue(Schema.LogMessage.Message, NormalizeLength(message, 1000));
            logMessage.Attributes.SetStringValue(Schema.LogMessage.ShortMessage, NormalizeLength(message, 250));

            return AdminService.Create(logMessage);
        }

        public Guid Log(Exception exception)
        {
            string logicalName = string.Empty;
            Guid id = Guid.Empty;

            if (Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity)
            {
                Entity target = (Entity)Context.InputParameters["Target"];
                logicalName = target.LogicalName;
                id = target.Id;
            }
            else if (Context.InputParameters.Contains("EntityMoniker") && Context.InputParameters["EntityMoniker"] is EntityReference)
            {
                EntityReference moniker = (EntityReference)Context.InputParameters["EntityMoniker"];
                logicalName = moniker.LogicalName;
                id = moniker.Id;
            }

            Entity logMessage = new Entity(Schema.LogMessage.LogicalName);

            if (!string.IsNullOrEmpty(logicalName))
            {
                logMessage.Attributes.SetStringValue(Schema.LogMessage.ObjectName, logicalName);
            }

            if (id != Guid.Empty)
            {
                logMessage.Attributes.SetStringValue(Schema.LogMessage.ObjectId, id.ToString());
            }


            if (exception.InnerException != null)
            {
                Guid innerExceptionId = Log(exception.InnerException);

            }

            logMessage.Attributes.SetStringValue(Schema.LogMessage.StackTrace, NormalizeLength(exception.StackTrace, 5000));
            logMessage.Attributes.SetStringValue(Schema.LogMessage.Message, NormalizeLength(exception.Message, 1000));
            logMessage.Attributes.SetStringValue(Schema.LogMessage.ShortMessage, NormalizeLength(exception.Message, 250));

            return AdminService.Create(logMessage);
        }

        private string NormalizeLength(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
