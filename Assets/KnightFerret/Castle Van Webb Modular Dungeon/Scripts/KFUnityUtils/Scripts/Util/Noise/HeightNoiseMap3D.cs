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
    public class HeightNoiseMap3D {
        // These are not a "magic" number, but it does take more math to derive than I want to write here,
        // though only basic calculus and a few step of simple high school algebra.
        int xOff, yOff, zOff, size, interval, cutoff, currentInterval, layer, scalex = 4, scaley = 16, scalez = 4;
        float[,,] field;
        float divisor;


        public HeightNoiseMap3D(int size, int interval, int cutoff) {
            this.size = size;
            this.interval = interval;
            this.cutoff = cutoff;
            this.layer = 0;
        }

        public HeightNoiseMap3D(int size, int interval, int sx, int sy, int sz) {
            this.size = size;
            this.interval = interval;
            this.cutoff = 2;
            this.layer = 0;
            SetCoordScale(sx, sy, sz);
        }


        public void SetCoordScale(int x, int y, int z) {
            scalex = x;
            scaley = y;
            scalez = z;
        }



        /**
         * Generate a noise map for map coordinates xOff,zOff.
         * 
         * @param rand
         * @param x
         * @param z
         * @return 
         */
        public void Process(SpatialHash rand, int x, int y, int z = 0, int layer = 0)  {
            xOff = x * size;
            yOff = y * size;
            zOff = z * size;
            layer = layer * 3;
            field = new float[size + 1, size + 1, size + 1];
            currentInterval = interval;
            divisor = 1.0f;
            while(currentInterval > cutoff) {
                ProcessLayer(rand);
                divisor *=2;
                currentInterval /= 2;
            }
            for(int i = 0; i < size + 1; i++)
                for(int j = 0; j < size + 1; j++)
                    for(int k = 0; k < size + 1; k++) {
                    field[i, j, k] = field[i, j, k];
                }
        }

        private void ProcessLayer(SpatialHash rand) {
            int nodesX = Mathf.Max(size / currentInterval + 2, 3);
            int nodesY = Mathf.Max(size / currentInterval + 2, 3);
            int nodesZ = Mathf.Max(size / currentInterval + 2, 3);
            Vec3D[,,] nodes = new Vec3D[nodesX, nodesY, nodesZ];
            for(int i = 0; i < nodesX; i++)
                for(int j = 0; j < nodesY; j++)
                    for(int k = 0; k < nodesZ; k++) {
                        nodes[i, j, k] = new Vec3D(rand, i + xOff / currentInterval,
                                j + yOff / currentInterval, k + zOff / currentInterval, layer);
                    }
            for(int i = 0; i < size + 1; i++)
                for(int j = 0; j < size + 1; j++)
                    for(int k = 0; k < size + 1; k++) {
                        field[i, j, k] += ProcessPoint(nodes, i, j, k);
                    }
        }


        public float ProcessPoint(Vec3D[,,] nodes, int x, int y, int z) {
            float output = 0.0f;
            
            float ci = (float)currentInterval;

            float dx = FullFade(x % currentInterval);
            float dy = FullFade(y % currentInterval);
            float dz = FullFade(z % currentInterval);

            int    px = x / currentInterval;
            int    py = y / currentInterval;
            int    pz = z / currentInterval;

            output += CalcLoc(nodes[px, py, pz],
                    new Vec3D(dx, dy, dz), ci);
            output += CalcLoc(nodes[px + 1, py, pz],
                    new Vec3D((ci - dx), dy, dz), ci);
            output += CalcLoc(nodes[px + 1, py + 1, pz],
                    new Vec3D((ci - dx), (ci - dy), dz), ci);
            output += CalcLoc(nodes[px, py + 1, pz],
                    new Vec3D(dx, (ci - dy), dz), ci);

            output += CalcLoc(nodes[px, py, pz + 1],
                    new Vec3D(dx, dy, (ci - dz)), ci);
            output += CalcLoc(nodes[px + 1, py, pz + 1],
                    new Vec3D((ci - dx), dy, (ci - dz)), ci);
            output += CalcLoc(nodes[px + 1, py + 1, pz + 1],
                    new Vec3D((ci - dx), (ci - dy), (ci - dz)), ci);
            output += CalcLoc(nodes[px, py + 1, pz + 1],
                    new Vec3D(dx, (ci - dy), (ci - dz)), ci);

            output /= interval;
            output /= 2.0f;
            return output;
        }

        
        private float CalcLoc(Vec3D from, Vec3D at, float ci) {
            double dx = at.x / ci;
            double dy = at.y / ci;
            double dz = at.z / ci;
            double l = (1 - ((dx * dx) + (dy * dy) + (dz * dz)));
            if(l > 0) {
                return (float)(Vec3D.Dot(from, at) * l);
            }        
            return 0.0f;
        }


        private float Fade(float val) {
            return val * val * val * (val * (val * 6 - 15) + 10);
        }


        private float FullFade(float val) {
            return Fade(val / currentInterval) * currentInterval;
        }


        private float FullFade(float val, float interval) {
            return Fade(val / interval) * interval;
        }


        public float GetValue(int x, int y, int z) {
            int px = x / scalex;
            int py = y / scaley;
            int pz = z / scalez;

            float dx = FullFade(x % scalex, scalex);
            float dy = FullFade(y % scaley, scaley);
            float dz = FullFade(z % scalez, scalez);

            float s1 = (field[px, py, pz] * (1 - dz)) + (field[px, py, pz + 1] * dz);
            float s2 = (field[px + 1, py, pz] * (1 - dz)) + (field[px + 1, py, pz + 1] * dz);
            float s3 = (field[px + 1, py + 1, pz] * (1 - dz)) + (field[px + 1, py + 1, pz + 1] * dz);
            float s4 = (field[px, py + 1, pz] * (1 - dz)) + (field[px, py + 1, pz + 1] * dz);

            float f1 = (s1 * (1 - dx)) + (s2 * dx);
            float f2 = (s4 * (1 - dx)) + (s3 * dx);

            return (f1 * (1 - dy)) + (f2 * dy);
        }


    }


}
