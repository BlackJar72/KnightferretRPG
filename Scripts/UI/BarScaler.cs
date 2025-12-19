using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg.ui {
    public class BarScaler : MonoBehaviour
    {
        public Image image;


        public void SetBar(float value)
        {
            try {
            image.rectTransform.localScale = 
                new Vector3(Mathf.Clamp(value, 0, 1), 
                    gameObject.transform.localScale.y, 
                    gameObject.transform.localScale.z);
            }
            catch (Exception e)
            {                
                Debug.Log("image = " + image);
                Debug.Log("gameObject = " + gameObject);
                Debug.LogException(e);
            }
        }


        public void SetBar(int current, int max)
        {
            float value = (float)current / (float)max;
            image.rectTransform.localScale =
                new Vector3(Mathf.Clamp(value, 0, 1),
                    gameObject.transform.localScale.y,
                    gameObject.transform.localScale.z);
        }

        public void SetBarInverse(float value)
        {
            image.rectTransform.localScale =
                new Vector3(Mathf.Clamp(1.0f - value, 0, 1),
                    gameObject.transform.localScale.y,
                    gameObject.transform.localScale.z);
        }


        public void SetBarInverse(int current, int max)
        {
            float value = (float)(max - current) / (float)max;
            image.rectTransform.localScale =
                new Vector3(Mathf.Clamp(value, 0, 1),
                    gameObject.transform.localScale.y,
                    gameObject.transform.localScale.z);
        }

        
    }

}
