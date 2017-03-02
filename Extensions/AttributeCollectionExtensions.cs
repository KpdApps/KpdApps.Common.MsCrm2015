﻿using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015.Extensions
{
	public static class AttributeCollectionExtensions
	{
		public static object GetValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return null;

			object crmValue = properties[name];
			if (crmValue == null)
				return null;

			Type fieldType = crmValue.GetType();
			Type[] endTypes = { typeof(DateTime), typeof(int), typeof(double) };

			EntityReference reference = crmValue as EntityReference;
			if (reference != null)
				return reference.Id;

			if (fieldType == typeof(OptionSetValue))
			{
				OptionSetValue field = (OptionSetValue)crmValue;
				return field.Value;
			}

			if (fieldType == typeof(Money))
			{
				Money field = (Money)crmValue;
				return field.Value;
			}

			if (endTypes.Contains(fieldType))
				return crmValue;

			if (fieldType == typeof(AliasedValue))
				return ((AliasedValue)crmValue).Value;

			return crmValue;
		}

		public static string GetStringValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return null;

			object property = properties[name];
			return property?.ToString();
		}

		public static void SetStringValue(this AttributeCollection properties, string name, string value)
		{
			if (!properties.Contains(name))
				properties.Add(name, value);
			else
				properties[name] = value;
		}

		public static int GetStatusValue(this AttributeCollection properties)
		{
			return GetPicklistValue(properties, "statuscode");
		}

		public static int GetStateValue(this AttributeCollection properties)
		{
			return GetPicklistValue(properties, "statecode");
		}

		public static int GetPicklistValue(this AttributeCollection properties, string name)
		{
			return GetPicklistValue(properties, name, -1);
		}

		public static int GetPicklistValue(this AttributeCollection properties, string name, int defaultValue)
		{
			if (!properties.Contains(name))
				return defaultValue;

			OptionSetValue property = properties[name] as OptionSetValue;
			return property?.Value ?? defaultValue;
		}

		public static void SetPicklistValue(this AttributeCollection properties, string name, int value)
		{
			if (!properties.Contains(name))
				properties.Add(name, new OptionSetValue(value));
			else
				properties[name] = new OptionSetValue(value);
		}

		public static void SetStatusValue(this AttributeCollection properties, string name, int value)
		{
			SetPicklistValue(properties, name, value);
		}

		public static DateTime GetDateTimeValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return DateTime.MinValue;

			object property = properties[name];
			if (property != null)
				return Convert.ToDateTime(property);
			return DateTime.MinValue;
		}

		public static void SetDateTimeValue(this AttributeCollection properties, string name, DateTime value)
		{
			if (!properties.Contains(name))
			{
				properties.Add(name, value);
			}
			else
			{
				properties[name] = value;
			}
		}

		public static EntityReference GetLookup(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name)) return null;

			var @ref = properties[name];
			if (@ref != null)
				return (EntityReference)properties[name];

			return null;
		}

		public static Guid GetLookupValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return Guid.Empty;

			object property = properties[name];
			EntityReference @ref = property as EntityReference;
			if (@ref != null)
				return @ref.Id;

			return Guid.Empty;
		}

		public static void SetLookupValue(this AttributeCollection properties, string name, string type, Guid value)
		{
			if (!properties.Contains(name))
			{
				properties.Add(name, new EntityReference(type, value));
			}
			else
			{
				properties[name] = new EntityReference(type, value);
			}
		}
	}
}