using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace KpdApps.Common.MsCrm2015.Extensions
{
    public sealed class SystemServiceSwitcher : IDisposable
    {
        private IOrganizationService Service { get; }

        private Guid UserCallerId { get; }

        private static readonly Dictionary<Guid, Guid> OrgSystemUserIds = new Dictionary<Guid, Guid>();

        protected Guid SystemUser
        {
            get
            {
                var orgId = ((WhoAmIResponse)Service.Execute(new WhoAmIRequest())).OrganizationId;

                if (OrgSystemUserIds.ContainsKey(orgId))
                    return OrgSystemUserIds[orgId];

                var query = new QueryExpression
                {
                    EntityName = "systemuser",
                    NoLock = true,
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("fullname", ConditionOperator.Equal, "SYSTEM")
                        }
                    }
                };

                EntityCollection users = Service.RetrieveMultiple(query);
                if (users.Entities.Count > 0)
                    OrgSystemUserIds.Add(orgId, users[0].Id);
                else throw new Exception("Пользователь SYSTEM не найден.");

                return OrgSystemUserIds[orgId];
            }
        }

        public SystemServiceSwitcher(IOrganizationService service)
        {
            Service = service;
            OrganizationServiceProxy proxy = Service as OrganizationServiceProxy;
            if (proxy != null)
            {
                UserCallerId = proxy.CallerId;
                proxy.CallerId = SystemUser;
            }
        }

        public void Dispose()
        {
            if (Service != null && ((OrganizationServiceProxy)Service).CallerId != UserCallerId)
                ((OrganizationServiceProxy)Service).CallerId = UserCallerId;
        }
    }
}