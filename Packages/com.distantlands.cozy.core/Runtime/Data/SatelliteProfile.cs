//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Satellite Profile", order = 361)]
    public class SatelliteProfile : ScriptableObject
    {

        public GameObject satelliteReference;
        public Transform orbitRef;
        public Transform moonRef;
        public Light lightRef;
        public float size = 1;
        [Range(0, 1)]
        public float distance = 1;
        public bool autoScaleByDistance = true;
        public float orbitOffset;
        public Vector3 initialRotation;
        public float satelliteRotateSpeed;
        public bool linkToDay;
        public int rotationPeriod = 28;
        public int rotationPeriodOffset;
        public Vector3 satelliteRotateAxis;
        public float satelliteDirection;
        public float satelliteRotation;
        public float satellitePitch;
        public float declination;
        public int declinationPeriod;
        public bool changedLastFrame;
        public bool open;

    }
    
}