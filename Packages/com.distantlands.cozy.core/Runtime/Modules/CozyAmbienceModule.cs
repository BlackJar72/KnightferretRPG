//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using DistantLands.Cozy.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DistantLands.Cozy
{

    [ExecuteAlways]
    public class CozyAmbienceModule : CozyBiomeModuleBase<CozyAmbienceModule>
    {

        [CozySearchable("Ambiences", "Ambience profiles", "profiles")]
        public AmbienceProfile[] ambienceProfiles = new AmbienceProfile[0];

        [System.Serializable]
        public class WeightedAmbience
        {
            public AmbienceProfile ambienceProfile;
            [Range(0, 1)]
            public float weight;
            public bool transitioning;
            public IEnumerator Transition(float value, float time)
            {
                transitioning = true;
                float t = 0;
                float start = weight;

                while (t < time)
                {

                    float div = (t / time);
                    yield return new WaitForEndOfFrame();

                    weight = Mathf.Lerp(start, value, div);
                    t += Time.deltaTime;

                }

                weight = value;
                ambienceProfile.SetWeight(weight);
                transitioning = false;

            }
        }

        public List<WeightedAmbience> weightedAmbience = new List<WeightedAmbience>();

        [CozySearchable("Ambience", "Ambience profile", "profile")]
        public AmbienceProfile currentAmbienceProfile;
        public AmbienceProfile ambienceChangeCheck;
        public float timeToChangeProfiles = 7;
        public float ambienceTimer;

        void Start()
        {
            if (!enabled)
                return;

            if (isBiomeModule)
                return;

            if (ambienceProfiles.Length == 0)
            {
                FindAllAmbiences();
            }

            foreach (AmbienceProfile profile in ambienceProfiles)
            {
                foreach (FXProfile fx in profile.FX)
                    fx?.InitializeEffect(weatherSphere);
            }

            if (Application.isPlaying)
            {
                SetNextAmbience();
                weightedAmbience = new List<WeightedAmbience>() { new WeightedAmbience() { weight = 1, ambienceProfile = currentAmbienceProfile } };
            }

        }

        public void FindAllAmbiences()
        {

            List<AmbienceProfile> profiles = new List<AmbienceProfile>();

            foreach (AmbienceProfile i in EditorUtilities.GetAllInstances<AmbienceProfile>())
                if (i.name != "Default Ambience")
                    profiles.Add(i);

            foreach (AmbienceProfile profile in ambienceProfiles)
            {
                foreach (FXProfile fx in profile.FX)
                    fx?.InitializeEffect(weatherSphere);
            }

            ambienceProfiles = profiles.ToArray();

        }

        // Update is called once per frame
        public override void UpdateWeatherWeights()
        {
            if (Application.isPlaying)
            {
                if (ambienceChangeCheck != currentAmbienceProfile)
                {
                    SetAmbience(currentAmbienceProfile);
                }

                if (weatherSphere.timeModule)
                    ambienceTimer -= Time.deltaTime * weatherSphere.timeModule.modifiedTimeSpeed;
                else
                    ambienceTimer -= Time.deltaTime * 0.005f;

                if (ambienceTimer <= 0)
                {
                    SetNextAmbience();
                }

                foreach (WeightedAmbience i in weightedAmbience)
                {
                    i.weight = i.weight * weight;
                }

                weightedAmbience.RemoveAll(x => x.weight == 0 && x.transitioning == false);

            }

            ComputeBiomeWeights();
            // ManageBiomeWeights();
        }

        public override void UpdateFXWeights()
        {
            foreach (WeightedAmbience weather in weightedAmbience)
            {
                if (weather != null && weather.ambienceProfile)
                    weather.ambienceProfile.SetWeight(weather.weight);
            }
        }
        public override void UpdateBiomeModule()
        {

            currentAmbienceProfile.SetWeight(weight);

        }

        public void SetNextAmbience()
        {

            currentAmbienceProfile = WeightedRandom(ambienceProfiles.ToArray());

        }

        public void SetAmbience(AmbienceProfile profile)
        {

            currentAmbienceProfile = profile;
            ambienceChangeCheck = currentAmbienceProfile;

            if (weightedAmbience.Find(x => x.ambienceProfile == profile) == null)
                weightedAmbience.Add(new WeightedAmbience() { weight = 0, ambienceProfile = profile, transitioning = true });

            foreach (WeightedAmbience j in weightedAmbience)
            {

                if (j.ambienceProfile == profile)
                    StartCoroutine(j.Transition(1, timeToChangeProfiles));
                else
                    StartCoroutine(j.Transition(0, timeToChangeProfiles));

            }

            ambienceTimer += Random.Range(currentAmbienceProfile.minTime, currentAmbienceProfile.maxTime);
        }

        public void SetAmbience(AmbienceProfile profile, float timeToChange)
        {

            currentAmbienceProfile = profile;
            ambienceChangeCheck = currentAmbienceProfile;

            if (weightedAmbience.Find(x => x.ambienceProfile == profile) == null)
                weightedAmbience.Add(new WeightedAmbience() { weight = 0, ambienceProfile = profile, transitioning = true });

            foreach (WeightedAmbience j in weightedAmbience)
            {

                if (j.ambienceProfile == profile)
                    StartCoroutine(j.Transition(1, timeToChange));
                else
                    StartCoroutine(j.Transition(0, timeToChange));

            }

            ambienceTimer += Random.Range(currentAmbienceProfile.minTime, currentAmbienceProfile.maxTime);
        }

        public void SkipTime(float timeToSkip) => ambienceTimer -= timeToSkip;

        public AmbienceProfile WeightedRandom(AmbienceProfile[] profiles)
        {
            AmbienceProfile i = null;
            List<float> floats = new List<float>();
            float totalChance = 0;

            foreach (AmbienceProfile k in profiles)
            {
                float chance;

                if (weatherSphere.weatherModule)
                    if (k.dontPlayDuring.Contains(weatherSphere.weatherModule.ecosystem.currentWeather))
                        chance = 0;
                    else
                        chance = k.GetChance(weatherSphere);
                else
                    chance = k.GetChance(weatherSphere);

                floats.Add(chance);
                totalChance += chance;
            }

            if (totalChance == 0)
            {
                i = (AmbienceProfile)Resources.Load("Default Ambience");
                Debug.LogWarning("Could not find a suitable ambience given the current selected profiles and chance effectors. Defaulting to an empty ambience.");
                return i;
            }

            float selection = Random.Range(0, totalChance);

            int m = 0;
            float l = 0;

            while (l <= selection)
            {
                if (selection >= l && selection < l + floats[m])
                {
                    i = profiles[m];
                    break;
                }
                l += floats[m];
                m++;

            }

            if (!i)
            {
                i = profiles[0];
            }

            return i;
        }

        public float GetTimeTillNextAmbience() => ambienceTimer;

    }

}