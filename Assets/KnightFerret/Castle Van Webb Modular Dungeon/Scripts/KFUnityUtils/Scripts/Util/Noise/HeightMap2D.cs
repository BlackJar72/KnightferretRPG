using UnityEngine;


namespace kfutils.noise
{

    public class HeightMap2D
    {
        int sizex, sizez, interval, cutoff, currentInterval;
        float[,] field;
        float scale;

        public float[,] Field => field;


        public HeightMap2D(int sizex, int sizey, int interval, int cutoff, float scale)
        {
            this.sizex = sizex;
            this.sizez = sizey;
            this.interval = interval;
            this.cutoff = cutoff;
            this.scale = scale;
        }

        public HeightMap2D(int sizex, int sizey, int interval, float scale)
        {
            this.sizex = sizex;
            this.sizez = sizey;
            this.interval = interval;
            this.cutoff = 2;
            this.scale = scale;
        }


        /**
         * Generate a noise map for map coordinates xOff,zOff.
         * 
         * @param rand
         * @param x
         * @param z
         * @return 
         */
        public float[,] Process(Xorshift rand, int x, int z)
        {
            field = new float[sizex, sizez];
            currentInterval = interval;
            while (currentInterval > cutoff)
            {
                ProcessOne(rand);
                currentInterval /= 2;
            }
            return field;
        }


        public Vector2 GetRandomVector(Xorshift rand)
        {
            float x = rand.NextFloat() * 2.0f - 1.0f;
            float y = rand.NextFloat() * 2.0f - 1.0f;
            return new Vector2(x, y);
        }


        /**
         * Generate a noise map for map coordinates 0,0.
         * 
         * @param rand
         * @return 
             */
        public float[,] Process(Xorshift rand)
            {
                field = new float[sizex, sizez];
                currentInterval = interval;
                while (currentInterval > 2)
                {
                    ProcessOne(rand);
                    currentInterval /= 2;
                }
                return field;
            }


            private void ProcessOne(Xorshift rand)
            {
                int nodesX = sizex / currentInterval + 2;
                int nodesY = sizez / currentInterval + 2;
                Vector2[,] nodes = new Vector2[nodesX, nodesY];
                for (int i = 0; i < nodesX; i++)
                    for (int j = 0; j < nodesY; j++)
                    {
                        nodes[i,j] = GetRandomVector(rand);
                    }
                for (int i = 0; i < sizex; i++)
                    for (int j = 0; j < sizez; j++)
                    {
                        field[i, j] += ProcessPoint(nodes, i, j) * scale;
                    }
            }


            public float ProcessPoint(Vector2[,] nodes, int x, int y)
            {
                float output = 0.0f;

                float ci = (float)currentInterval;
                float dx = FullFade(x % currentInterval);
                float dy = FullFade(y % currentInterval);
                int px = x / currentInterval;
                int py = y / currentInterval;

            output += CalcLoc(nodes[px, py],
                        new Vector2(dx, dy), ci);
            output += CalcLoc(nodes[px + 1, py],
                        new Vector2((ci - dx), dy), ci);
            output += CalcLoc(nodes[px + 1, py + 1],
                        new Vector2((ci - dx), (ci - dy)), ci);
            output += CalcLoc(nodes[px, py + 1],
                        new Vector2(dx, (ci - dy)), ci);

            output /= interval;
            output /= 2.0f;
            return output;
            }


            private float CalcLoc(Vector2 from, Vector2 at, float ci)
            {
                double dx = at.x / ci;
                double dy = at.y / ci;
                double l = (1 - ((dx * dx) + (dy * dy)));
                if(l > 0) {
                    return (float)(Vector2.Dot(from, at) * l);            
                }        
                return 0.0f;
            }


            private float Fade(float input)
            {
                return input * input * input * (input * (input * 6 - 15) + 10);
            }


            private float FullFade(float input)
            {
                return Fade(input / currentInterval) * currentInterval;
            }
    }

}
