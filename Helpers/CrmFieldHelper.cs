using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using KpdApps.Common.MsCrm2015.Extensions;

namespace KpdApps.Common.MsCrm2015.Helpers
{
    public static class CrmFieldHelper
    {
        public static object GetFieldValueByFieldAddress(Entity targetEntity, string fieldAddress, IOrganizationService service)
        {
            if (string.IsNullOrEmpty(fieldAddress))
            {
                return null;
            }

            string[] fieldsArray = fieldAddress.Split('.');
            string fieldName = string.Empty;
            EntityReference entityRef = targetEntity.ToEntityReference();
            Entity ent = targetEntity;
            object result = null;

            for (var i = 0; i < fieldsArray.Length; i++)
            {
                fieldName = fieldsArray[i];

                int nextIndex = i + 1;

                if (nextIndex < fieldsArray.Length)
                {
                    entityRef = ent.Attributes.GetLookup(fieldName);
                    ent = service.Retrieve(entityRef.LogicalName, entityRef.Id, new ColumnSet(fieldsArray[nextIndex]));
                }
                else
                {
                    result = ent.Attributes.GetValue(fieldName);
                }
            }
                
            return result;
        }
    }
}