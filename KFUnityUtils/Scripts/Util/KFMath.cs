using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils {

    public static class KFMath {

        /// <summary>
        /// This is will produce an always positive modulus,
        /// that is, a remainder from the next lower number
        /// even when negative.  Many situations require this,
        /// such as when locating a value in a 2D grid stored
        /// as a 1D array.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ModRight(int a, int b) {
            return (a & 0x7fffffff) % b;
        }


        /// <summary>
        /// A convenience method, but one probably better coded locally in
        /// most situations for efficiency (at least in intended uses).  In
        /// some ways this is a reminder, but could be handy in non-performance
        /// critical code.
        ///
        /// n is the number being converted to an asymptopic form.
        /// start is the place where the output should start to curve.
        /// rate is the reciprical of the value it should approach minus the start.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="start"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static float Asymptote(float n, float start, float rate) {
            if(n > start) {
                float output = (n - start) / rate;
                output = 1 - (1 / (output + 1));
                output = (output * rate) + start;
                return output;
            }
            return n;
        }


        /// <summary>
        /// A convenience method, but one probably better coded locally in
        /// most situations for efficiency (at least in intended uses).  In
        /// some ways this is a reminder, but could be handy in non-performance
        /// critical code.
        ///
        /// n is the number being converted to an asymptopic form.
        /// start is the place where the output should start to curve.
        /// rate is the reciprical of the value it should approach minus the start.
        ///
        /// </summary>
        /// <param name="n"></param>
        /// <param name="start"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static double Asymptote(double n, double start, double rate) {
            if(n > start) {
                double output = (n - start) / rate;
                output = 1 - (1 / (output + 1));
                output = (output * rate) + start;
                return output;
            }
            return n;
        }


        /// <summary>
        /// This will convert a string to a long for use as a seed for random number generation.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long GetLongSeed(this string str) {
            string strSeed = str.Trim();
            long output;
            try
            {
                output = long.Parse(strSeed);
            }
            catch (System.FormatException)
            {
                output = strSeed.GetHashCode();
                output |= output << 32;
            }
            return output;
        }


        /// <summary>
        /// This will convert a string to a long for use as a seed for random number generation.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ulong GetULongSeed(this string str) {
            string strSeed = str.Trim();
            ulong output = 0;
            try
            {
                output = ulong.Parse(strSeed);
            }
            catch (System.FormatException)
            {
                output |= (ulong)((uint)strSeed.GetHashCode());
                output |= output << 32;
            }
            return output;
        }


        /// <summary>
        /// This will convert a string to a long for use as a seed for random number generation.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetIntSeed(this string str) {
            string strSeed = str.Trim();
            int output;
            try
            {
                output = int.Parse(strSeed);
            }
            catch (System.FormatException)
            {
                output = strSeed.GetHashCode();
            }
            return output;
        }

    }

}
