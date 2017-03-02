using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015.Extensions
{
	public static class PluginExecutionContextExtensions
	{
		/// <summary>
		/// Return "Target" from ImputParameters as <see cref="Entity"/>.
		/// If Target not found will return null.
		/// </summary>
		/// <param name="pluginContext"><see cref="IPluginExecutionContext"/>.</param>
		/// <returns><see cref="Entity"/>.</returns>
		public static Entity GetEntity(this IPluginExecutionContext pluginContext)
		{
			Entity result = null;

			// Check if the InputParameters property bag contains a target
			// of the current operation and that target is of type DynamicEntity.
			if (pluginContext.InputParameters.Contains("Target") &&
				pluginContext.InputParameters["Target"] is Entity)
			{
				// Obtain the target business entity from the input parameters.
				result = (Entity)pluginContext.InputParameters["Target"];
			}

			return result;
		}
	}
}
