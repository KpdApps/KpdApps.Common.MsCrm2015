using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace KpdApps.Common.MsCrm2015.Extensions
{
	/// <summary>
	/// Extensions for EntityReference.
	/// </summary>
	public static class EntityReferenceExtensions
	{
		/// <summary>
		/// Convert EntityReference to ActivityParty.
		/// </summary>
		/// <param name="reference"><see cref="EntityReference"/>.</param>
		/// <returns>EntityCollection with ActivityParty.</returns>
		public static EntityCollection ToActivityParty(this EntityReference reference)
		{
			Entity activityParty = new Entity
			{
				LogicalName = "activityparty",
				Attributes = { ["partyid"] = reference }
			};
			EntityCollection activityCollection = new EntityCollection();
			activityCollection.Entities.Add(activityParty);
			return activityCollection;
		}

		/// <summary>
		/// Get Entity by EntityReference.
		/// </summary>
		/// <param name="entityReference"></param>
		/// <param name="organizationService"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		public static Entity ToEntity(this EntityReference entityReference, IOrganizationService organizationService, string[] columns = null)
		{
			return organizationService.Retrieve(
					entityReference.LogicalName
					, entityReference.Id
					, columns == null ? new ColumnSet(true) : new ColumnSet(columns));
		}
	}
}
