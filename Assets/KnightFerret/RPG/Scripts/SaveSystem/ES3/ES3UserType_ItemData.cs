using System;
using kfutils.rpg;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("prototype", "transformData", "metadata", "physics")]
	public class ES3UserType_ItemData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ItemData() : base(typeof(kfutils.rpg.ItemData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (kfutils.rpg.ItemData)obj;
			
			writer.WriteProperty("id", instance.ID);			
			writer.WriteProperty("prototype", instance.Prototype.ID);
			writer.WriteProperty("transformData", instance.transformData, ES3UserType_TransformData.Instance);
			writer.WritePrivateField("metadata", instance);
			writer.WriteProperty("physics", instance.physics, ES3Type_bool.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			kfutils.rpg.ItemPrototype prototype = null;
			string id = null;
			kfutils.TransformData transformData = null;
			kfutils.rpg.ItemMetadata metadata = null;
			bool physics = false;
			foreach (string propertyName in reader.Properties)
			{
				switch (propertyName)
				{
					case "id":
						id = reader.Read<string>();
						break;
					case "prototype":
						prototype = ItemManagement.GetPrototype(reader.Read<string>());
						break;
					case "transformData":
						transformData = reader.Read<kfutils.TransformData>(ES3UserType_TransformData.Instance);
						break;
					case "metadata":
						metadata = reader.Read<ItemMetadata>();
						break;
					case "physics":
						physics = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
            ItemData instance = new(id, prototype, transformData, metadata) {
                physics = physics
            };
        }

		protected ItemData ReadObjectData(ES3Reader reader) {
			kfutils.rpg.ItemPrototype prototype = null;
			string id = null;
			kfutils.TransformData transformData = null;
			kfutils.rpg.ItemMetadata metadata = null;
			bool physics = false;
			foreach (string propertyName in reader.Properties)
			{
				switch (propertyName)
				{
					case "id":
						id = reader.Read<string>();
						break;
					case "prototype":
						prototype = ItemManagement.GetPrototype(reader.Read<string>());
						break;
					case "transformData":
						transformData = reader.Read<kfutils.TransformData>(ES3UserType_TransformData.Instance);
						break;
					case "metadata":
						metadata = reader.Read<ItemMetadata>();
						break;
					case "physics":
						physics = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
			ItemData instance = new(id, prototype, transformData, metadata)
			{
				physics = physics
			};
			return instance;
        }

		protected override object ReadObject<T>(ES3Reader reader) {
			var instance = ReadObjectData(reader);
			return instance;
		}
	}


	public class ES3UserType_ItemDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ItemDataArray() : base(typeof(kfutils.rpg.ItemData[]), ES3UserType_ItemData.Instance)
		{
			Instance = this;
		}
	}
}