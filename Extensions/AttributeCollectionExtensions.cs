using System;
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
			if (properties.Contains(name))
				properties[name] = value;
			else
				properties.Add(name, value);
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
			if (properties.Contains(name))
				properties[name] = new OptionSetValue(value);
			else
				properties.Add(name, new OptionSetValue(value));
		}

		public static void SetStatusValue(this AttributeCollection properties, int value)
		{
			SetPicklistValue(properties, "statuscode", value);
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
			if (properties.Contains(name))
				properties[name] = value;
			else
				properties.Add(name, value);
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
			if (properties.Contains(name))
				properties[name] = new EntityReference(type, value);
			else
				properties.Add(name, new EntityReference(type, value));
		}

		public static int GetNumberValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return 0;

			object property = properties[name];
			return property != null ? Convert.ToInt32(property) : 0;
		}

		public static decimal GetMoneyValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return 0;

			object property = properties[name];
			Money num = property as Money;
			return num != null ? Convert.ToDecimal(num.Value) : 0;
		}

		public static double GetFloatValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return 0;

			object property = properties[name];
			return property != null ? Convert.ToDouble(property) : 0;
		}

		public static decimal GetDecimalValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return 0;

			object property = properties[name];
			return property != null ? Convert.ToDecimal(property) : 0;
		}

		public static void SetNumberValue(this AttributeCollection properties, string name, int value)
		{
			if (properties.Contains(name))
				properties[name] = value;
			else
				properties.Add(name, value);
		}

		public static void SetMoneyValue(this AttributeCollection properties, string name, decimal value)
		{
			if (properties.Contains(name))
				properties[name] = new Money(value);
			else
				properties.Add(name, new Money(value));
		}

		public static void SetDecimalValue(this AttributeCollection properties, string name, decimal value)
		{
			if (properties.Contains(name))
				properties[name] = value;
			else
				properties.Add(name, value);
		}

		public static void SetFloatValue(this AttributeCollection properties, string name, double value)
		{
			if (properties.Contains(name))
				properties[name] = value;
			else
				properties.Add(name, value);
		}

		public static bool GetBooleanValue(this AttributeCollection properties, string name)
		{
			if (!properties.Contains(name))
				return false;

			object property = properties[name];
			return property != null && Convert.ToBoolean(property);
		}

		public static void SetBooleanValue(this AttributeCollection properties, string name, bool value)
		{
			if (properties.Contains(name))
				properties[name] = value;
			else
				properties.Add(name, value);
		}

		public static T GetAliasedValue<T>(this AttributeCollection collection, string aliasedEntityName, string attributeName)
		{
			const string aliasedTemplate = "{0}.{1}";

			if (string.IsNullOrEmpty(aliasedEntityName))
			{
				throw new ArgumentNullException(nameof(aliasedEntityName));
			}

			if (string.IsNullOrEmpty(attributeName))
			{
				throw new ArgumentNullException(nameof(attributeName));
			}

			var aliasedAttrName = string.Format(aliasedTemplate, aliasedEntityName, attributeName);

			if (!collection.Contains(aliasedAttrName))
			{
				return default(T);
			}

			var value = collection[aliasedAttrName];

			//Если значение аттрибута нулевое или не типа AliasedValue - дефолтное значение 
			if (!(value is AliasedValue))
			{
				return default(T);
			}

			var aliasedValue = ((AliasedValue)value).Value;
			return (T)aliasedValue;
		}
	}
}