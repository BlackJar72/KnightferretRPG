//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using DistantLands.Cozy.Data;
using System.Collections.Generic;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyWeatherModule : CozyBiomeModuleBase<CozyWeatherModule>, ICozyEcosystem
    {
        public float cumulus;
        public float cirrus;
        public float altocumulus;
        public float cirrostratus;
        public float chemtrails;
        public float nimbus;
        public float nimbusHeight;
        public float nimbusVariation;
        public float borderHeight;
        public float borderEffect;
        public float borderVariation;
        public float fogDensity;

        public float filterSaturation;
        public float filterValue;
        public Color filterColor = Color.white;
        public Color sunFilter = Color.white;
        public Color cloudFilter = Color.white;

        [CozySearchable(true)]
        public CozyEcosystem ecosystem;

        public CozyEcosystem Ecosystem { get => ecosystem; set => ecosystem = value; }
        public CozySystem LocalSystem { get => system; }

        [WeatherRelation]
        [CozySearchable]
        public List<WeatherRelation> currentWeatherProfiles = new List<WeatherRelation>();
        public FilterFX defaultFilter;
        public CloudFX defaultClouds;

        private WeatherProfile strongestWeather;

        public void Awake()
        {
            if (!enabled)
                return;

            RunChecks();

            ecosystem.SetupEcosystem();
            ResetFilter();
            ResetClouds();
            strongestWeather = currentWeatherProfiles.Find(x => x.weight > 0.5f).profile;

        }

        public override void InitializeModule()
        {
            isBiomeModule = GetComponent<CozyBiome>();

            if (isBiomeModule)
            {
                AddBiome();
                return;
            }
            base.InitializeModule();
            weatherSphere.weatherModule = this;
            AddBiome();

        }

        private void RunChecks()
        {
            defaultClouds = (CloudFX)Resources.Load("Default Profiles/Default Clouds");
            defaultFilter = (FilterFX)Resources.Load("Default Profiles/Default Filter");

            ecosystem ??= new CozyEcosystem();

            if (system == weatherSphere)
                weatherSphere.weatherModule = this;
            ecosystem.weatherSphere = weatherSphere;
            ecosystem.system = system;
        }

        public void Start()
        {
            foreach (WeatherProfile profile in ecosystem.forecastProfile.profilesToForecast)
            {
                foreach (FXProfile fx in profile.FX)
                    fx?.InitializeEffect(weatherSphere);
            }
        }

        public override void UpdateWeatherWeights()
        {
            ecosystem.UpdateEcosystem();
            ManageGlobalEcosystem();
            UpdateWeatherByWeight();

            WeatherProfile strongestWeatherThisFrame = currentWeatherProfiles.Find(x => x.weight > 0.5f).profile;

            if (strongestWeatherThisFrame != strongestWeather)
            {
                strongestWeather = strongestWeatherThisFrame;
                weatherSphere.events.RaiseOnWeatherChange();
            }

        }

        public override void UpdateFXWeights()
        {
            foreach (WeatherRelation weather in currentWeatherProfiles)
            {
                weather.profile.SetWeatherWeight(weather.weight);
            }
        }

        public override void FrameReset()
        {
            ResetClouds();
            ResetFilter();
        }

        /// <summary>
        /// Calculates the weather color filter based on the currently active Filter FX profiles.
        /// </summary> 
        void ResetFilter()
        {
            if (ecosystem == null)
                return;


            filterSaturation = defaultFilter.filterSaturation;
            filterValue = defaultFilter.filterValue;
            filterColor = defaultFilter.filterColor;
            sunFilter = defaultFilter.sunFilter;
            cloudFilter = defaultFilter.cloudFilter;

        }

        /// <summary>
        /// Calculates the clouds based on the currently active Cloud FX profiles.
        /// </summary> 
        void ResetClouds()
        {
            if (ecosystem == null)
                return;

            cumulus = defaultClouds.cumulusCoverage;
            cirrus = defaultClouds.cirrusCoverage;
            altocumulus = defaultClouds.altocumulusCoverage;
            cirrostratus = defaultClouds.cirrostratusCoverage;
            chemtrails = defaultClouds.chemtrailCoverage;
            nimbus = defaultClouds.nimbusCoverage;
            nimbusHeight = defaultClouds.nimbusHeightEffect;
            nimbusVariation = defaultClouds.nimbusVariation;
            borderHeight = defaultClouds.borderHeight;
            borderEffect = defaultClouds.borderEffect;
            borderVariation = defaultClouds.borderVariation;
            fogDensity = defaultClouds.fogDensity;

        }

        /// <summary>
        /// Send all weather information to the main COZY Weather Sphere for rendering.
        ///</summary>
        public override void PropogateVariables()
        {
            weatherSphere.cumulus = cumulus;
            weatherSphere.cirrus = cirrus;
            weatherSphere.altocumulus = altocumulus;
            weatherSphere.cirrostratus = cirrostratus;
            weatherSphere.chemtrails = chemtrails;
            weatherSphere.nimbus = nimbus;
            weatherSphere.nimbusHeightEffect = nimbusHeight;
            weatherSphere.nimbusVariation = nimbusVariation;
            weatherSphere.borderHeight = borderHeight;
            weatherSphere.borderEffect = borderEffect;
            weatherSphere.borderVariation = borderVariation;
            weatherSphere.fogDensity = fogDensity;

            weatherSphere.filterSaturation = filterSaturation;
            weatherSphere.filterValue = filterValue;
            weatherSphere.filterColor = filterColor;
            weatherSphere.sunFilter = sunFilter;
            weatherSphere.cloudFilter = cloudFilter;
        }

        void ManageGlobalEcosystem()
        {
            if (system == null) RunChecks();
            currentWeatherProfiles.Clear();

            if (weight > 0)
                foreach (WeatherRelation weatherRelation in ecosystem.weightedWeatherProfiles)
                {
                    if (weatherRelation.weight == 0)
                    {
                        weatherRelation.profile.SetWeatherWeight(0);
                        continue;
                    }

                    if (currentWeatherProfiles.Find(x => x.profile == weatherRelation.profile) != null)
                    {
                        currentWeatherProfiles.Find(x => x.profile == weatherRelation.profile).weight += weatherRelation.weight * weight;
                        continue;
                    }

                    WeatherRelation l = new WeatherRelation
                    {
                        profile = weatherRelation.profile,
                        weight = weatherRelation.weight * weight
                    };
                    currentWeatherProfiles.Add(l);

                }

            foreach (CozyWeatherModule biome in biomes)
            {
                if (biome == null) continue;

                CozyEcosystem localEcosystem = biome.Ecosystem;

                if (biome.weight > 0)
                {
                    foreach (WeatherRelation weatherRelation in localEcosystem.weightedWeatherProfiles)
                    {
                        if (weatherRelation.weight == 0)
                        {
                            if (weatherRelation.profile)
                                weatherRelation.profile.SetWeatherWeight(0);
                            continue;
                        }

                        if (currentWeatherProfiles.Find(x => x.profile == weatherRelation.profile) != null)
                        {
                            currentWeatherProfiles.Find(x => x.profile == weatherRelation.profile).weight += weatherRelation.weight * biome.weight;
                            continue;
                        }

                        WeatherRelation l = new WeatherRelation();
                        l.profile = weatherRelation.profile;
                        l.weight = weatherRelation.weight * biome.weight;
                        currentWeatherProfiles.Add(l);

                    }
                }
                else
                {
                    foreach (WeatherRelation i in localEcosystem.weightedWeatherProfiles)
                    {
                        i.profile.SetWeatherWeight(0);
                    }
                }
            }
        }

        void UpdateWeatherByWeight()
        {
            ComputeBiomeWeights();

            float weatherWeightAcrossSystems = 0;

            foreach (WeatherRelation i in currentWeatherProfiles) weatherWeightAcrossSystems += i.weight;

            if (weatherWeightAcrossSystems == 0)
                weatherWeightAcrossSystems = 1;

            foreach (WeatherRelation i in currentWeatherProfiles)
            {
                i.weight /= weatherWeightAcrossSystems;
                // i.profile.SetWeatherWeight(i.weight);

            }
        }

    }


}