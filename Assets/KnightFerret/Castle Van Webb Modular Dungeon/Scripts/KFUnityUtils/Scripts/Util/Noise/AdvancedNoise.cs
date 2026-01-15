using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils.noise {

    /// <summary>
    /// Produces a distorted noise map by using more noise layers to pull around the effective locations.
    /// As a terrain this produces maps that are full of ridges and rifts, and also less biased toward features
    /// following the asxes.
    ///
    /// WARNING:  This uses a fair amount of RAM and can be pretty slow.
    /// THIS IS NOT REALLY GOOD ANYWAY!!! -- DO NOT WASTE TIME USING IT!
    /// </summary>
    public class AdvancedNoise {
        private int sizex, sizez;
        private int interval, cutoff;
        SpatialHash srandom;
        private HeightNoiseMap noiseMaker;
        private float[,,] scratchpad;
        private float[,] field;


        public AdvancedNoise(SpatialHash random, int sizex, int sizez, int interval, int cutoff) {
            this.sizex = sizex;
            this.sizez = sizez;
            this.interval = interval;
            this.cutoff = cutoff;
            srandom = random;
            noiseMaker = new HeightNoiseMap(sizex, sizez, interval, cutoff);
        }


        public AdvancedNoise(SpatialHash random, int sizex, int sizez, int interval) {
            this.sizex = sizex;
            this.sizez = sizez;
            this.interval = interval;
            this.cutoff = 2;
            srandom = random;
            noiseMaker = new HeightNoiseMap(sizex, sizez, interval);
        }



        public float[,] Process(int x, int z) {
            scratchpad = new float[3, sizex * 3, sizez * 3];
            noiseMaker.SetFullScaling();
            for(int i = -1; i < 2; i++)
                for(int j = -1; j < 2; j++)
                    for(int k = 1; k < 3; k++) {
                        float[,] tmp = noiseMaker.Process(srandom, x + i, z + j, k);
                        for(int i2 = 0; i2 < sizex; i2++)
                            for(int j2 = 0; j2 < sizez; j2++) {
                                scratchpad[k, i2 + (sizex * i) + sizex, j2 + (sizez * j) + sizez] = tmp[i2, j2];
                            }
                    }
            noiseMaker.ResetDefaultScaling();
            for(int i = -1; i < 2; i++)
                for(int j = -1; j < 2; j++){
                    float[,] tmp = noiseMaker.Process(srandom, x + i, z + j, 0);
                    for(int i2 = 0; i2 < sizex; i2++)
                        for(int j2 = 0; j2 < sizez; j2++) {
                            scratchpad[0, i2 + (sizex * i) + sizex, j2 + (sizez * j) + sizez] = tmp[i2, j2];
                        }
                    }
            int sx2 = sizex + 1, sz2 = sizez + 1, xshift = sizex / 4, zshift = sizez / 4;
            field = new float[sx2, sz2];
            for(int i = 0; i < sx2; i++)
                for(int j = 0; j < sz2; j++) {
                    field[i, j] = scratchpad[0, i + sizex + (int)(scratchpad[1, i  + sizex, j + sizez] * xshift),
                                                j + sizez + (int)(scratchpad[2, i  + sizex, j + sizez] * zshift)];
                }
            return field;
        }

    }

}