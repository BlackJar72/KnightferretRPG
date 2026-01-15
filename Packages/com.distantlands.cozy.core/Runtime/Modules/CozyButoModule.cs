//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using UnityEngine.Rendering;
#if BUTO
using OccaSoftware.Buto.Runtime;
#endif

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyButoModule : CozyModule
    {

#if BUTO

        [SerializeField]
        [CozySearchable("Buto", "profile", "volumetric fog")]
        private ButoVolumetricFog fog;
        [SerializeField]
        private VolumeProfile volumeProfile;
        [CozySearchable]
        [Range(0, 2)] public float fogBrightnessMultiplier;
        [CozySearchable]
        [Range(0, 2)] public float fogDensityMultiplier;

        void Awake()
        {
            TryFindFog();
        }

        void Update()
        {

            if (weatherSphere == null)
                base.InitializeModule();

            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
                return;

            if (fog)
            {

                fog.colorInfluence.Override(1);
                fog.litColor.Override(weatherSphere.fogColor5 * fogBrightnessMultiplier);
                fog.shadowedColor.Override(weatherSphere.fogColor5 * 0.5f * fogBrightnessMultiplier);
                fog.fogDensity.Override(fogDensityMultiplier * 10 * weatherSphere.fogDensity);

            }
            else
            {
                TryFindFog();
            }

        }

        void TryFindFog()
        {

            if (volumeProfile)
            {

                foreach (VolumeComponent component in volumeProfile.components)
                {

                    if (component is ButoVolumetricFog)
                    {
                        fog = (ButoVolumetricFog)component;
                        return;
                    }
                }
            }
            else
            {
                foreach (Volume vol in FindObjectsByType<Volume>(FindObjectsSortMode.None))
                {
                    foreach (VolumeComponent component in vol.profile.components)
                    {

                        if (component is ButoVolumetricFog)
                        {
                            fog = (ButoVolumetricFog)component;
                            volumeProfile = vol.profile;
                            return;
                        }
                    }
                }
            }

            Debug.Log("Could not find instance of Buto in the scene!");
        }
#endif

    }

}