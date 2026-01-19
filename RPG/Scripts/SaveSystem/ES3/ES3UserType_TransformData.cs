using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("position", "rotation", "scale")]
	public class ES3UserType_TransformData : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3UserType_TransformData() : base(typeof(kfutils.TransformData)){ Instance = this; priority = 1;}


		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (kfutils.TransformData)obj;
			
			writer.WriteProperty("position", instance.position, ES3Type_Vector3.Instance);
			writer.WriteProperty("rotation", instance.rotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("scale", instance.scale, ES3Type_Vector3.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new kfutils.TransformData();
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "position":
						instance.position = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "rotation":
						instance.rotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "scale":
						instance.scale = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
			return instance;
		}
	}


	public class ES3UserType_TransformDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TransformDataArray() : base(typeof(kfutils.TransformData[]), ES3UserType_TransformData.Instance)
		{
			Instance = this;
		}
	}
}