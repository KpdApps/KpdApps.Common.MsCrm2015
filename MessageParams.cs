using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace KpdApps.Common.MsCrm2015
{
    public class MessageParams
    {
        public TargetEntityType EntityType { get; private set; }
        public string EntityName { get; private set; }
        public string TargetEntityReferenceName { get; private set; }
        public string MessageName { get; private set; }
        public string[] PreImages { get; private set; }
        public string[] PostImages { get; private set; }
        public Dictionary<string, Type> InputParameters { get; private set; }
        public BasePluginAction Action { get; private set; }

        private MessageParams(BasePluginAction action, string messageName, Dictionary<string, Type> inputParameters, string[] preImages, string[] postImages)
        {
            Action = action;
            MessageName = messageName;
            PreImages = preImages != null ? preImages : new string[] { };
            PostImages = postImages != null ? postImages : new string[] { };
            InputParameters = inputParameters != null ? inputParameters : new Dictionary<string, Type>();
        }

        public static MessageParams CreateMessageParams(BasePluginAction action, string messageName)
        {
            return new MessageParams(action, messageName, null, null, null);
        }

        public static MessageParams CreateMessageParams(BasePluginAction action, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters)
        {
            return new MessageParams(action, messageName, inputParameters, preImages, postImages);
        }

        public static MessageParams CreateForTargetEntity(BasePluginAction action, string entityName, string messageName)
        {
            return CreateForTargetEntity(action, entityName, messageName, null, null, null);
        }

        public static MessageParams CreateForTargetEntity(BasePluginAction action, string entityName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters)
        {
            MessageParams messageParams = new MessageParams(action, messageName, inputParameters, preImages, postImages)
            {
                EntityType = TargetEntityType.TargetEntity,
                EntityName = entityName
            };

            if (!messageParams.InputParameters.ContainsKey(TargetTypes.Target))
                messageParams.InputParameters.Add(TargetTypes.Target, typeof(Entity));

            return messageParams;
        }

        public static MessageParams CreateForTargetEntityReference(BasePluginAction action, string entityName, string messageName)
        {
            return CreateForTargetEntityReference(action, entityName, messageName, null, null, null);
        }

        public static MessageParams CreateForTargetEntityReference(BasePluginAction action, string entityName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters)
        {
            MessageParams messageParams = new MessageParams(action, messageName, inputParameters, preImages, postImages)
            {
                EntityType = TargetEntityType.TargetEntityReference,
                EntityName = entityName
            };

            if (!messageParams.InputParameters.ContainsKey(TargetTypes.Target))
                messageParams.InputParameters.Add(TargetTypes.Target, typeof(EntityReference));

            return messageParams;
        }

        public static MessageParams CreateForTargetEntityMoniker(BasePluginAction action, string entityName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters)
        {
            MessageParams messageParams = new MessageParams(action, messageName, inputParameters, preImages, postImages)
            {
                EntityType = TargetEntityType.EntityMoniker,
                EntityName = entityName
            };

            if (!messageParams.InputParameters.ContainsKey(TargetTypes.EntityMoniker))
                messageParams.InputParameters.Add(TargetTypes.EntityMoniker, typeof(EntityReference));

            return messageParams;
        }

        public static MessageParams CreateForRelationship(BasePluginAction action, string relationshipName, string messageName)
        {
            return CreateForRelationship(action, relationshipName, messageName, null, null, null);
        }

        public static MessageParams CreateForRelationship(BasePluginAction action, string relationshipName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters)
        {
            MessageParams messageParams = new MessageParams(action, messageName, inputParameters, preImages, postImages)
            {
                EntityType = TargetEntityType.Relationship,
                EntityName = relationshipName
            };

            if (!messageParams.InputParameters.ContainsKey(TargetTypes.Relationship))
                messageParams.InputParameters.Add(TargetTypes.Relationship, typeof(Relationship));

            return messageParams;
        }
    }
}
