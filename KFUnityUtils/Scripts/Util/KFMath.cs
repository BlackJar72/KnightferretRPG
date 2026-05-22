using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] 
        public static int ModRight(int a, int b) => (a & 0x7fffffff) % b;


        /// <summary>
        /// TLDR: APPLIES A "SOFT CAP."
        /// 
        /// Causes the input to start asymptoting at the number given as start, 
        /// such that the output would start + rate at n = infinity.
        /// 
        /// For value of n less than start the output will be n, with n = start 
        /// producing the same results for either formula so as to create a 
        /// continuous curve.
        /// 
        /// This was first developed to avoid sudden height cut-off on precedurally 
        /// generated mountains that might otherwise produce conspicuously unnatural 
        /// looking fast tops if exceeding a height limit.
        /// 
        /// It has since been used effectively to soft-cap armor in the health and 
        /// damage system included in the "Damage System" folder.  It was also used 
        /// to soft cap difficulty in the Cavern Of Evil "NG+" levels. It could be  
        /// useful for applying a soft cap to anything, really.
        ///
        /// n is the number being converted to an asymptopic form.
        /// start is the place where the output should start to curve.
        /// rate is the reciprical of the value it should approach minus the start.
        /// </summary>
        /// <param name="n">The value being asymptotically soft capped.</param>
        /// <param name="start">And offset represent the point where the soft cap begins</param>
        /// <param name="rate">How far past start the result would be at n = infinity</param>
        /// <returns>The soft capped value</returns>
        [Pure] public static float Asymptote(float n, float start, float rate) {
            if(n > start) {
                float output = (n - start) / rate;
                output = 1 - (1 / (output + 1));
                output = (output * rate) + start;
                return output;
            }
            return n;
        }


        /// <summary>
        /// TLDR: APPLIES A "SOFT CAP."
        /// 
        /// Causes the input to start asymptoting at the number given as start, 
        /// such that the output would start + rate at n = infinity.
        /// 
        /// For value of n less than start the output will be n, with n = start 
        /// producing the same results for either formula so as to create a 
        /// continuous curve.
        /// 
        /// This was first developed to avoid sudden height cut-off on precedurally 
        /// generated mountains that might otherwise produce conspicuously unnatural 
        /// looking fast tops if exceeding a height limit.
        /// 
        /// It has since been used effectively to soft-cap armor in the health and 
        /// damage system included in the "Damage System" folder.  It was also used 
        /// to soft cap difficulty in the Cavern Of Evil "NG+" levels. It could be  
        /// useful for applying a soft cap to anything, really. 
        ///
        /// n is the number being converted to an asymptopic form.
        /// start is the place where the output should start to curve.
        /// rate is the reciprical of the value it should approach minus the start.
        ///
        /// <param name="n">The value being asymptotically soft capped.</param>
        /// <param name="start">And offset represent the point where the soft cap begins</param>
        /// <param name="rate">How far past start the result would be at n = infinity</param>
        /// <returns>The soft capped value</returns>
        [Pure] public static double Asymptote(double n, double start, double rate) {
            if(n > start) {
                double output = (n - start) / rate;
                output = 1 - (1 / (output + 1));
                output = (output * rate) + start;
                return output;
            }
            return n;
        }



        /// <summary>
        /// An alternate assymtote function, I probably won't be using it in typical code, but I want it 
        /// documented for future porting, perhaps to HLSL or GLSL be used in shaders.  I wonder if something
        /// like this could be used to improved parallax displacement shaders.
        /// 
        /// Base on the function
        /// f(x) = x / (1 + abs(x))
        /// 
        /// https://gist.github.com/BlackJar72/1ceff3a726d18a986ba47262a5585905
        ///  
        /// </summary>
        /// <param name="n"></param>
        /// <param name="start"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        [Pure] public static float FastSigmoid(float n, float start, float rate) {
            if(n > start) {
                // Transforms the scale, hopefully; I need to thrink this part through and test it
                float output = (n - start) / rate;
                // Creates and signmoid output, representing the function:   f(x) = x / (1 + abs(x))  
                // 0 => 0, 1 => 0.5, -1 => -0.5, and with 1 and -1 approached as positive and negative infinity respectively
                output = output / (1 + Math.Abs(output));
                // Reverses the previous transform to the original scale and start
                output = (output * rate) + start;
                return output;
            }
            return n;
        }



        /// <summary>
        /// An alternate assymtote function, I probably won't be using it in typical code, but I want it 
        /// documented for future porting, perhaps to HLSL or GLSL be used in shaders.  I wonder if something
        /// like this could be used to improved parallax displacement shaders.
        /// 
        /// Base on the function
        /// f(x) = x / (1 + abs(x))
        /// 
        /// https://gist.github.com/BlackJar72/1ceff3a726d18a986ba47262a5585905
        ///  
        /// </summary>
        /// <param name="n"></param>
        /// <param name="start"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        [Pure] public static double FastSigmoid(double n, double start, double rate) {
            if(n > start) {
                // Transforms the scale, hopefully; I need to thrink this part through and test it
                double output = (n - start) / rate;
                // Creates and signmoid output, representing the function:   f(x) = x / (1 + abs(x))  
                // 0 => 0, 1 => 0.5, -1 => -0.5, and with 1 and -1 approached as positive and negative infinity respectively
                output = output / (1 + Math.Abs(output));
                // Reverses the previous transform to the original scale and start
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
        /// This will convert a string to a ulong for use as a seed for random number generation.
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
        /// This will convert a string to an int for use as a seed for random number generation.
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] 
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            float dx = x1 - x2;
            float dy = y1 - y2;
            return Mathf.Sqrt((dx * dx) + (dy * dy));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] 
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] 
        public static float SquareDistance(float x1, float y1, float x2, float y2)
        {
            float dx = x1 - x2;
            float dy = y1 - y2;
            return (dx * dx) + (dy * dy);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] 
        public static double SquareDistance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return (dx * dx) + (dy * dy);
        }

    }

}
