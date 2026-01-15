using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozySetMoonDirection : MonoBehaviour
    {

        CozyWeather weatherSphere;

        // Update is called once per frame
        void Update()
        {

            if (weatherSphere == null)
                weatherSphere = CozyWeather.instance;

            weatherSphere.moonDirection = -transform.forward;
            Shader.SetGlobalVector("CZY_MoonDirection", -transform.forward);

        }
    }
}