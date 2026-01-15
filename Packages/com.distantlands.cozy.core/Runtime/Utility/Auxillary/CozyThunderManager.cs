using System.Collections;
using System.Collections.Generic;
using DistantLands.Cozy.Data;
using UnityEngine;

namespace DistantLands.Cozy
{
    public class CozyThunderManager : MonoBehaviour
    {
        private float thunderTimer = 0;
        public CozyWeather weatherSphere;
        public ThunderFX thunderFX;
        // Update is called once per frame
        public void PlayEffect(float weight)
        {
            if (!Application.isPlaying)
                return;
                
            if (weight > 0.5f)
            {
                thunderTimer -= Time.deltaTime;

                if (thunderTimer <= 0)
                {
                    Strike();
                }
                if (thunderTimer > thunderFX.timeBetweenStrikes.y)
                {
                    thunderTimer = thunderFX.timeBetweenStrikes.y;
                }

            }
        }

        public void Strike()
        {

            if (!weatherSphere.cozyCamera) return;

            Camera cozyCamera = weatherSphere.cozyCamera;

            Vector3 worldPoint;

            if (Random.value > thunderFX.spawnInFrustumPercentage)
            {

                Vector3 randomPoint = new Vector3(
                    Random.Range(thunderFX.minScreenXmultiplier, thunderFX.maxScreenXmultiplier),
                    Random.Range(thunderFX.minScreenYmultiplier, thunderFX.maxScreenYmultiplier),
                    Random.Range(cozyCamera.nearClipPlane + thunderFX.minimumDistance, thunderFX.maximumDistance)
                );

                worldPoint = cozyCamera.ViewportToWorldPoint(randomPoint);
            }
            else
                worldPoint = cozyCamera.transform.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized * Random.Range(thunderFX.minimumDistance, thunderFX.maximumDistance);

            worldPoint.y = cozyCamera.transform.position.y;

            Transform i = Instantiate(thunderFX.thunderPrefab, worldPoint, Quaternion.identity, transform).transform;

            i.LookAt(cozyCamera.transform.position);

            thunderTimer = Random.Range(thunderFX.timeBetweenStrikes.x, thunderFX.timeBetweenStrikes.y);
        }

    }
}