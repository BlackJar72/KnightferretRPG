using System;
using UnityEngine;

namespace kfutils.rpg {


    public static class Options
    {
        private static float lookSensitivity = 1.0f;
        private static float lookDeadzone = 3.0f;
        private static int lookSmoothing = 3;
        private static Vector2[] lookArray = new Vector2[lookSmoothing + 1]; 

        private static float moveSensitivity = 1.0f;
        private static float moveDeadzone = 0.0f;
        private static int moveSmoothing = 3;
        private static Vector2[] moveArray = new Vector2[moveSmoothing + 1];


        // Look methods; setting values with methods to avoid them bein accidentall set via properties
        #region Look Option Handling
        public static void SetLookSensitivity(int value) => lookSensitivity = value;
        public static void SetLookDeadZone(int value) => lookDeadzone = value;


        public static void SetLookSmoothing(int value)
        {
            lookSmoothing = value;
            lookArray = new Vector2[lookSmoothing + 1];
        }


        public static Vector3 ApplyLookDeadzone(Vector2 input)
        {
            input.x = Math.Max(Mathf.Abs(input.x) - lookDeadzone, 0.0f) * Mathf.Sign(input.x);
            input.y = Math.Max(Mathf.Abs(input.y) - lookDeadzone, 0.0f) * Mathf.Sign(input.y);
            return input;
        }


        public static Vector2 SmoothLook(Vector2 input)
        {
            Vector2 sum = input;
            for(int i = 1; i < lookSmoothing; i++)
            {
                sum += lookArray[i];
                lookArray[i - 1] = lookArray[i];
            }
            lookArray[lookSmoothing - 1] = input;
            lookArray[lookSmoothing] = sum / lookSmoothing;
            return lookArray[lookSmoothing];
        }


        public static Vector2 ProcessLook(Vector2 input) =>
            SmoothLook(ApplyLookDeadzone(input) * lookSensitivity);

        #endregion Look Option Handling


        // Move methods; setting values with methods to avoid them bein accidentall set via properties
        #region Move Option Handling
        public static void SetMoveSensitivity(int value) => moveSensitivity = value;
        public static void SetMoveDeadZone(int value) => moveDeadzone = value;


        public static void SetMoveSmoothing(int value)
        {
            moveSmoothing = value;
            moveArray = new Vector2[moveSmoothing + 1];
        }


        public static Vector3 ApplyMoveDeadzone(Vector2 input)
        {
            input.x = Math.Max(Mathf.Abs(input.x) - moveDeadzone, 0.0f) * Mathf.Sign(input.x);
            input.y = Math.Max(Mathf.Abs(input.y) - moveDeadzone, 0.0f) * Mathf.Sign(input.y);
            return input;
        }


        public static Vector2 SmoothMove(Vector2 input)
        {
            Vector2 sum = input;
            for(int i = 1; i < moveSmoothing; i++)
            {
                sum += moveArray[i];
                moveArray[i - 1] = moveArray[i];
            }
            moveArray[moveSmoothing - 1] = input;
            moveArray[moveSmoothing] = sum / moveSmoothing;
            return moveArray[moveSmoothing];
        }


        public static Vector2 ProcessMove(Vector2 input) =>
            SmoothMove(ApplyMoveDeadzone(input) * moveSensitivity);

        #endregion Look Option Handling





    }


}
