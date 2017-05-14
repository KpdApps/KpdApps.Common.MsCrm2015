using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Xrm.Sdk;

namespace KpdApps.Common.MsCrm2015.Extensions
{
	/// <summary>
	/// Extensions for Entity.
	/// </summary>
	public static class EntityExtensions
	{
		/// <summary>
		/// Copy all attributes (except primary key) to new Entity.
		/// </summary>
		public static Entity Clone(this Entity entity)
		{
			Entity result = new Entity(entity.LogicalName);

			//Поле не подлежащие клонированию
			StringCollection skippedFields = new StringCollection { entity.LogicalName + "id" };
			if (entity.Contains("activitytypecode"))
				skippedFields.Add("activityid");

			foreach (var attr in entity.Attributes)
			{
				if (skippedFields.Contains(attr.Key))
					continue;

				object val = CloneAttributeValue(attr.Value);
				result.Attributes.Add(attr.Key, val);
			}

			return result;
		}

		/// <summary>
		/// Copy selected columns to new Entity.
		/// </summary>
		/// <param name="entity">Original Entity.</param>
		/// <param name="fields">Columns to copy.</param>
		public static Entity Clone(this Entity entity, params string[] fields)
		{
			if (fields == null)
				throw new ArgumentNullException(nameof(fields));

			Entity result = new Entity(entity.LogicalName);
			foreach (string field in fields)
			{
				if (!entity.Contains(field))
					continue;

				object val = CloneAttributeValue(entity[field]);
				result.Attributes.Add(field, val);
			}

			return result;
		}

		public static void SetFieldsByMapping(this Entity target, Entity sourceEntity, Dictionary<string, string> mapping)
		{
			foreach (var sourceAttrName in mapping.Keys)
			{
				object value = null;
				if (sourceEntity.Attributes.ContainsNotNull(sourceAttrName))
				{
					value = sourceEntity.Attributes[sourceAttrName];
				}

				var targetName = mapping[sourceAttrName];
				if (target.Attributes.Contains(targetName))
				{
					target.Attributes[targetName] = value;
				}
				else
				{
					target.Attributes.Add(targetName, value);
				}
			}
		}

		/// <summary>
		/// Check attribute by regular Contains and Value not null.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		public static bool ContainsNotNull(this Entity entity, string attributeName)
		{
			return entity.Attributes.ContainsNotNull(attributeName);
		}

		/// <summary>
		/// Copy attribute value.
		/// </summary>
		/// <param name="value"><see cref="OptionSetValue"/>, <see cref="Money"/>, <see cref="EntityReference"/>, <see cref="EntityCollection"/></param>
		/// <returns></returns>
		private static object CloneAttributeValue(object value)
		{
			object val = value;
			if (val is OptionSetValue)
			{
				var opt = (OptionSetValue)val;
				val = new OptionSetValue(opt.Value);
			}
			else if (val is Money)
			{
				var m = (Money)val;
				val = new Money(m.Value);
			}
			else if (val is EntityReference)
			{
				var temp = (EntityReference)val;
				val = new EntityReference(temp.LogicalName, temp.Id);
			}
			else if (val is EntityCollection)
			{
				var coll = (EntityCollection)val;
				val = new EntityCollection() { EntityName = coll.EntityName };
				foreach (Entity e in coll.Entities)
				{
					var clone = e.Clone();
					if (clone.LogicalName == "activityparty")
					{
						if (clone.Attributes.Contains("activityid"))
						{
							clone.Attributes.Remove("activityid");
						}

						if (clone.Attributes.Contains("activitypartyid"))
						{
							clone.Attributes.Remove("activitypartyid");
						}
					}
					((EntityCollection)val).Entities.Add(clone);
				}
			}
			return val;
		}

		/// <summary>
		/// Serialize Entity by DataContractSerializer.
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <returns>Serialized string.</returns>
		public static string Serialize(this Entity entity)
		{
			var serializer = new DataContractSerializer(typeof(Entity), null, int.MaxValue, false, false, null, new KnownTypesResolver());
			using (var sWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				var writer = new XmlTextWriter(sWriter);
				serializer.WriteObject(writer, entity);
				return sWriter.ToString();
			}
		}

		/// <summary>
		/// Deserialize Entity by DataContractSerializer.
		/// </summary>
		/// <param name="entityXml">Serialized string.</param>
		/// <returns>Entity.</returns>
		public static Entity Deserialize(string entityXml)
		{
			var reader = new XmlTextReader(new StringReader(entityXml));
			var serializer = new DataContractSerializer(typeof(Entity), null, int.MaxValue, false, false, null, new KnownTypesResolver());
			return (Entity)serializer.ReadObject(reader);
		}

		/// <summary>
		/// Create empty Entity (which inherit only parent logical name and identification field)
		/// </summary>
		public static Entity CreateEmpty(this Entity entity)
		{
			var result = new Entity(entity.LogicalName) { Id = entity.Id };

			var keyName = entity.LogicalName + "id";
			if (entity.LogicalName.Contains(keyName))
				result.Attributes.Add(keyName, entity.Id);

			return result;
		}
	}
}
