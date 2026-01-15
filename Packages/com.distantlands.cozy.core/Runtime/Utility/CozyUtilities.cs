using System.Collections;
using DistantLands.Cozy.Data;
using UnityEngine;

namespace DistantLands.Cozy
{
    public class CozyUtilities
    {
        public static float Remap(float sourceStart, float sourceEnd, float destinationStart, float destinationEnd, float value)
        {
            var ratio = Mathf.InverseLerp(sourceStart, sourceEnd, value);
            return Mathf.Lerp(destinationStart, destinationEnd, ratio);
        }

        public static T GetOverriableDefault<T>()
        {
            return default;
        }
        public static Color GetOverriableDefault()
        {
            return Color.clear;
        }

    }

    [System.Serializable]
    public class WeatherRelation
    {
        [Range(0, 1)] public float weight; public WeatherProfile profile; public bool transitioning = true;

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
            transitioning = false;

        }

    }

    [System.Serializable]
    public struct Overridable<T>
    {  
        public T value;
        public bool overrideValue;
        public static implicit operator bool(Overridable<T> data)
        {
            return data.overrideValue;
        }
        public Overridable(T _value, bool _overrideValue)
        {
            overrideValue = _overrideValue;
            value = _value;
        }
        public static implicit operator T(Overridable<T> data)
        {
            return data.overrideValue ? data.value : CozyUtilities.GetOverriableDefault<T>();
        }
        public static implicit operator Overridable<T>(T value)
        {
            return new Overridable<T>(value, true);
        }
    }

}