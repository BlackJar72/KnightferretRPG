using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg.ui {
    public class AdvancedBarScaler : MonoBehaviour
    {
        public Image image;
        public Gradient gradient;


        public void SetBar(float value)
        {
            value = Mathf.Clamp01(value);
            image.rectTransform.localScale = 
                new Vector3(gameObject.transform.localScale.x,
                            value, gameObject.transform.localScale.z);
                image.color = gradient.Evaluate(value);
        }


        public void SetBar(int current, int max)
        {
            float value = Mathf.Clamp01((float)current / (float)max);
            image.rectTransform.localScale =
                new Vector3(gameObject.transform.localScale.x,
                            value, gameObject.transform.localScale.z);
            image.color = gradient.Evaluate(value);
        }

        public void SetBarInverse(float value)
        {
            value = Mathf.Clamp01(1.0f - value);
            image.rectTransform.localScale =
                new Vector3(gameObject.transform.localScale.x,
                            value, gameObject.transform.localScale.z);
            image.color = gradient.Evaluate(value);

        }


        public void SetBarInverse(int current, int max)
        {
            float value = Mathf.Clamp01((float)(max - current) / (float)max);
            image.rectTransform.localScale =
                new Vector3(gameObject.transform.localScale.x, 
                            value, gameObject.transform.localScale.z);
            image.color = gradient.Evaluate(value);        
        }


        public void Activate()
        {
            gameObject.SetActive(true);
        }


        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

    }

}
