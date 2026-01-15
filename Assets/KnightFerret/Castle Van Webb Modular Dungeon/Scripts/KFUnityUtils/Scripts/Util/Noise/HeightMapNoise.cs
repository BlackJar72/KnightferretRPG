using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils.noise {

    /**
     * A simple 2-D gradient noise generation class.  Technically, this is misnamed 
     * as it does not have all of Ken Perlin's specific innovations and deviates 
     * from true perlin noise in various way -- but its pretty close, being based 
     * on the same underlying principles.
     * 
     * Also not, this implementation sacrifices optimization for clarity; usually 
     * a good thing in code, for the likely use cases a more optimized 
     * implementation that this would likely be desired in production code.
     *
     * This sizes used should be the same (should probably be one variable) and most be
     * a power of two; this is the number of squares in the terrain -- the size of the
     * array created will be one more to represent terrain vertices.
     *
     * @author jared
     */
    public class HeightNoiseMap {
        // These are not a "magic" number, but it does take more math to derive than I want to write here,
        // though only basic calculus and a few step of simple high school algebra.
        private const float SQRT_THREE = 1.73205080757f;
        private const float SCALE_FOR_ZERO_TO_ONE = SQRT_THREE * 0.5f;
        int xOff, zOff, sizex, sizez, interval, cutoff, currentInterval, layer;
        float[,] field;
        float divisor;
        private float scale = SCALE_FOR_ZERO_TO_ONE;
        private float offset = 0.5f;


        public HeightNoiseMap(int sizex, int sizey, int interval, int cutoff) {
            this.sizex = sizex;
            this.sizez = sizey;
            this.interval = interval;
            this.cutoff = cutoff;
            this.layer = 0;
        }

        public HeightNoiseMap(int sizex, int sizey, int interval) {
            this.sizex = sizex;
            this.sizez = sizey;
            this.interval = interval;
            this.cutoff = 2;
            this.layer = 0;
        }


        public void ResetDefaultScaling() {
            scale = SCALE_FOR_ZERO_TO_ONE;
            offset = 0.5f;
        }


        public void SetFullScaling() {
            scale = SQRT_THREE;
            offset = 0.0f;
        }


        public void SetCustomScaling(float scale, float offset) {
            this.scale = scale;
            this.offset = offset;
        }


        /**
         * Generate a noise map for map coordinates xOff,zOff.
         * 
         * @param rand
         * @param x
         * @param z
         * @return 
         */
        public float[,] Process(SpatialHash rand, int x, int z, int layer = 0)  {
            xOff = x * sizex;
            zOff = z * sizez;
            layer = layer * 2;
            field = new float[sizex + 1, sizez + 1];
            currentInterval = interval;
            divisor = 1.0f;
            while(currentInterval > cutoff) {
                ProcessOne(rand);
                divisor *= 2;
                currentInterval /= 2;
            }
            for(int i = 0; i < sizex + 1; i++)
                for(int j = 0; j < sizez + 1; j++) {
                    field[i, j] = (field[i, j] * scale) + offset;
                }
            return field;
        }


        private void ProcessOne(SpatialHash rand) {
            int nodesX = Mathf.Max(sizex / currentInterval + 2, 3);
            int nodesY = Mathf.Max(sizez / currentInterval + 2, 3);
            Vec2D[,] nodes = new Vec2D[nodesX, nodesY];
            for(int i = 0; i < nodesX; i++)
                for(int j = 0; j < nodesY; j++) {
                    nodes[i, j] = new Vec2D(rand, i + xOff / currentInterval,
                            j + zOff / currentInterval, (int)divisor, layer);
                }
            for(int i = 0; i < sizex + 1; i++)
                for(int j = 0; j < sizez + 1; j++) {
                    field[i, j] += ProcessPoint(nodes, i, j);
                }
        }


        public float ProcessPoint(Vec2D[,] nodes, int x, int y) {
            float output = 0.0f;
            
            float ci = (float)currentInterval;
            float dx = FullFade(x % currentInterval);
            float dy = FullFade(y % currentInterval);
            int    px = x / currentInterval;
            int    py = y / currentInterval;

            output += CalcLoc(nodes[px, py],
                    new Vec2D(dx, dy), ci);
            output += CalcLoc(nodes[px + 1, py],
                    new Vec2D((ci - dx), dy), ci);
            output += CalcLoc(nodes[px + 1, py + 1],
                    new Vec2D((ci - dx), (ci - dy)), ci);
            output += CalcLoc(nodes[px, py + 1],
                    new Vec2D(dx, (ci - dy)), ci);

            output /= interval;
            output /= 2.0f;
            return output;
        }

        
        private float CalcLoc(Vec2D from, Vec2D at, float ci) {
            double dx = at.x / ci;
            double dy = at.y / ci;
            double l = (1 - ((dx * dx) + (dy * dy)));
            if(l > 0) {
                return (float)(Vec2D.Dot(from, at) * l);            
            }        
            return 0.0f;
        }


        private float Fade(float val) {
            return val * val * val * (val * (val * 6 - 15) + 10);
        }


        private float FullFade(float val) {
            return Fade(val / currentInterval) * currentInterval;
        }


    }


}
