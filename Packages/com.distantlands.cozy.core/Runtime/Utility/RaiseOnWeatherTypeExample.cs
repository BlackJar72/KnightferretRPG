using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy
{
    public class RaiseOnWeatherTypeExample : MonoBehaviour
    {

        public EventFX weatherType;

        void OnEnable()
        {

            weatherType.onCall += OnStart;
            weatherType.onCall += OnStart;

        }
        void OnDisable()
        {

            weatherType.onCall -= OnStart;
            weatherType.onEnd -= OnEnd;

        }

        public void OnStart()
        {

            //Place your code for when the Event FX profile is started here.

        }

        public void OnEnd()
        {

            //Place your code for when the Event FX profile is ended here.

        }
    }
}