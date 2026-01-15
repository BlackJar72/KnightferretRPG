using System.Collections;
using System.Collections.Generic;
using kfutils.rpg;
using UnityEngine;

namespace cvwdemo {

    public class Quit : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                Application.Quit();
            }
        }
    }

}
