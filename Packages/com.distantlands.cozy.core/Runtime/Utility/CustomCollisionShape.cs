using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CustomCollisionShape : MonoBehaviour
    {
        public MeshCollider trigger;
        public Color displayColor = new Color(1, 1, 1, 1);
        public List<Vector3> bounds = new List<Vector3>() { new Vector3(-5, 0, 5), new Vector3(5, 0, 5), new Vector3(5, 0, -5), new Vector3(-5, 0, -5) };
        public float height = 10;

        void OnEnable()
        {
            if (!trigger)
                CheckTrigger();
        }

        void OnDisable()
        {
            if (trigger.gameObject.activeInHierarchy)
                DestroyImmediate(trigger);
        }

        public void CheckTrigger()
        {
            trigger = gameObject.AddComponent<MeshCollider>();
            trigger.sharedMesh = BuildZoneCollider();
            trigger.convex = true;
            trigger.isTrigger = true;

        }
        public Mesh BuildZoneCollider()
        {

            Mesh mesh = new Mesh();
            mesh.name = $"{name} Custom Trigger Mesh";

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            foreach (Vector3 i in bounds)
            {

                verts.Add(i);
                verts.Add(new Vector3(i.x, height, i.z));

            }

            for (int i = 0; i < bounds.Count; i++)
            {

                if (i == 0)
                {
                    tris.Add(0);
                    tris.Add(verts.Count - 1);
                    tris.Add(verts.Count - 2);

                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(verts.Count - 1);
                }
                else
                {
                    int start = i * 2;

                    tris.Add(start);
                    tris.Add(start - 1);
                    tris.Add(start - 2);

                    tris.Add(start);
                    tris.Add(start + 1);
                    tris.Add(start - 1);

                }
            }

            for (int i = 0; i < verts.Count - 4; i += 2)
            {

                tris.Add(0);
                tris.Add(i + 2);
                tris.Add(i + 4);

                tris.Add(1);
                tris.Add(i + 3);
                tris.Add(i + 5);

            }


            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0, true);
            mesh.RecalculateNormals();

            return mesh;


        }

        private void OnDrawGizmos()
        {

            if (!trigger)
                return;

            if (bounds.Count >= 3)
            {
                for (int i = 0; i < bounds.Count; i++)
                {

                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.3f);
                    Gizmos.DrawSphere(TransformToLocalSpace(bounds[i]), 0.2f);

                    Vector3 point = Vector3.zero;
                    if (i == 0)
                        point = bounds.Last();
                    else
                        point = bounds[i - 1];

                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 1);
                    Gizmos.DrawLine(TransformToLocalSpace(bounds[i]), TransformToLocalSpace(point));

                }

                for (int i = 0; i < bounds.Count; i++)
                {

                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.5f);
                    Gizmos.DrawSphere(TransformToLocalSpace(bounds[i]) + Vector3.up * height, 0.2f);

                    Vector3 point = Vector3.zero;
                    if (i == 0)
                        point = bounds.Last();
                    else
                        point = bounds[i - 1];

                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 1);
                    Gizmos.DrawLine(TransformToLocalSpace(bounds[i]) + Vector3.up * height, TransformToLocalSpace(point) + Vector3.up * height);
                    Gizmos.DrawLine(TransformToLocalSpace(bounds[i]), TransformToLocalSpace(bounds[i]) + Vector3.up * height);
                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.3f);
                    Gizmos.DrawLine((TransformToLocalSpace(bounds[i]) + TransformToLocalSpace(point)) / 2,
                        (TransformToLocalSpace(bounds[i]) + TransformToLocalSpace(point)) / 2 + Vector3.up * height);


                }

                Gizmos.DrawMesh(trigger.sharedMesh, -1, transform.position, Quaternion.identity, Vector3.one);

            }

        }
        private Vector3 TransformToLocalSpace(Vector3 pos)
        {
            Vector3 i = pos.x * transform.right + pos.y * transform.up + pos.z * transform.forward;
            i += transform.position;
            return i;

        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomCollisionShape))]
    public class E_CustomCollisionShape : Editor
    {

        public CustomCollisionShape shape;

        public void OnEnable()
        {

            shape = (CustomCollisionShape)target;


        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
            if (EditorGUI.EndChangeCheck())
                if (shape.trigger)
                    shape.trigger.sharedMesh = shape.BuildZoneCollider();
                else
                    shape.CheckTrigger();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayColor"));
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}