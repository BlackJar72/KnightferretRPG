using System;
using Unity.VisualScripting;
using UnityEngine;


namespace kfutils
{


    [System.Serializable]
    public class TransformData
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public Quaternion rotation;
        [SerializeField] public Vector3 scale = Vector3.one;


        [System.Serializable]
        public struct Data {
            [SerializeField] public Vector3Data position;
            [SerializeField] public Quaternion rotation;
            [SerializeField] public Vector3Data scale;
            public Data(TransformData data) {
                position = new Vector3Data(data.position);
                rotation = data.rotation;
                scale    = new Vector3Data(data.scale);
            }
            public TransformData GetTransformData => new TransformData(this);
            public void SetTransformData(TransformData transform) {
                transform.position = position.Vector;
                transform.rotation = rotation;
                transform.scale    = scale.Vector;
            }
        }

        public TransformData() {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

        public TransformData(Transform transform)
        {
            position = transform.localPosition;
            rotation = transform.localRotation;
            scale = transform.localScale;          
        }

        public TransformData(TransformData.Data transform)
        {
            position = transform.position.Vector;
            rotation = transform.rotation;
            scale = transform.scale.Vector;          
        }

        public TransformData(Vector3 pos, Quaternion rot, Vector3 scale) {
            position = pos;
            rotation = rot;
            this.scale = scale;
        }

        public TransformData(Vector3 pos, Quaternion rot) {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }


        public void SetData(ref Transform transform)
        {
            transform.SetLocalPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }


        public void AddData(ref Transform transform)
        {
            transform.localPosition += position;
            transform.localRotation *= rotation;
        }


        public string ToJSON() {
            return JsonUtility.ToJson(new TransformData.Data(this));
        }


        public void SetFromToJSON(string json) {
            JsonUtility.FromJson<TransformData.Data>(json).SetTransformData(this);
        }


        public static TransformData FromJSON(string json) {
            return new TransformData(JsonUtility.FromJson<TransformData.Data>(json));
        }
        

    }



    [System.Serializable]
    public struct Vector3Data {
        public float x, y, z;

        public readonly Vector3 Vector { get => new(x, y, z); }

        public Vector3Data(Vector3 vector) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public void SetData(ref Vector3 vector) {
            vector.x = x;
            vector.y = y;
            vector.z = z;
        }

        public void AddData(ref Vector3 vector) {
            vector.x += x;
            vector.y += y;
            vector.z += z;
        }
    }


    public static class TransformExtensions {


        public static void SetData(this Transform transform, TransformData data) {
            transform.SetLocalPositionAndRotation(data.position, data.rotation);
            transform.localScale = data.scale; 
        }

        
        public static void SetDataGlobal(this Transform transform, TransformData data) {
            transform.SetPositionAndRotation(data.position, data.rotation);
            transform.localScale = data.scale;
        }


        public static void SetLoccation(this Transform transform, TransformData data) => 
                    transform.SetLocalPositionAndRotation(data.position, data.rotation);

        
        public static void SetLocationsGlobal(this Transform transform, TransformData data) =>
                    transform.SetPositionAndRotation(data.position, data.rotation);

        
        public static TransformData GetGlobalData(this Transform transform) => 
                    new TransformData(transform.position, transform.rotation, transform.localScale);

        
        public static TransformData GetLocalData(this Transform transform) => 
                    new TransformData(transform.localPosition, transform.localRotation, transform.localScale);
            

    }

}