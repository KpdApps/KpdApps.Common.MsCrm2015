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

        public IPluginExecutionContext Context => State.Context;

        protected virtual MessageParams[] GetMessageParams()
        {
            return new MessageParams[] { };
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            State = CreatePluginState(serviceProvider);

            try
            {
                MessageParams[] messageParams = GetMessageParams();
                foreach (var messageParam in messageParams)
                {
                    if (!Validate(messageParam))
                        continue;

                    messageParam.Action.Execute();
                }
            }
            catch (Exception ex)
            {
                State?.TracingService.TraceError(ex);
                throw;
            }
        }

        private bool Validate(MessageParams messageParam)
        {
            if (string.Compare(messageParam.MessageName, Context.MessageName, true) != 0)
                return false;

            foreach (var preImage in messageParam.PreImages)
            {
                if (!Context.PreEntityImages.ContainsKey(preImage))
                    throw new Exception($"PRE IMAGE \"{preImage}\" not found.");
            }

            foreach (var postImage in messageParam.PostImages)
            {
                if (!Context.PostEntityImages.ContainsKey(postImage))
                    throw new Exception($"POST IMAGE \"{postImage}\" not found.");
            }

            foreach (var inputParameter in messageParam.InputParameters)
            {
                if (!Context.InputParameters.ContainsKey(inputParameter.Key))
                    throw new Exception($"InputParameter {inputParameter.Key} not found.");

                if (Context.InputParameters[inputParameter.Key].GetType() != inputParameter.Value)
                    throw new Exception($"InputParameter ({inputParameter.Key}, {Context.InputParameters[inputParameter.Key].GetType().Name} does not match  {inputParameter.Value.Name}.");
            }

            if (!string.IsNullOrEmpty(messageParam.EntityName))
            {
                switch (messageParam.EntityType)
                {
                    case TargetEntityType.Relationship:
                        if (string.Compare(((Relationship)Context.InputParameters[TargetTypes.Relationship]).SchemaName, messageParam.EntityName, true) != 0)
                            return false;
                        break;
                    case TargetEntityType.TargetEntity:
                        if (string.Compare(((Entity)Context.InputParameters[TargetTypes.Target]).LogicalName, messageParam.EntityName, true) != 0)
                            return false;
                        break;
                    case TargetEntityType.EntityMoniker:
                        if (string.Compare(((EntityReference)Context.InputParameters[TargetTypes.EntityMoniker]).LogicalName, messageParam.EntityName, true) != 0)
                            return false;
                        break;
                    case TargetEntityType.TargetEntityReference:
                        if (string.Compare(((EntityReference)Context.InputParameters[TargetTypes.Target]).LogicalName, messageParam.EntityName, true) != 0)
                            return false;
                        break;
                }
            }

            return true;
        }

        public abstract void ExecuteInternal(PluginState state);

        public virtual PluginState CreatePluginState(IServiceProvider serviceProvider)
        {
            return new PluginState(serviceProvider);
        }
    }
}
