using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y", "z")]
	public class ES3UserType_Vector3Data : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3UserType_Vector3Data() : base(typeof(kfutils.Vector3Data)){ Instance = this; priority = 1;}


		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (kfutils.Vector3Data)obj;
			
			writer.WriteProperty("x", instance.x, ES3Type_float.Instance);
			writer.WriteProperty("y", instance.y, ES3Type_float.Instance);
			writer.WriteProperty("z", instance.z, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new kfutils.Vector3Data();
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "x":
						instance.x = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "y":
						instance.y = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "z":
						instance.z = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
			return instance;
		}
	}


	public class ES3UserType_Vector3DataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Vector3DataArray() : base(typeof(kfutils.Vector3Data[]), ES3UserType_Vector3Data.Instance)
		{
			Instance = this;
		}
	}
}