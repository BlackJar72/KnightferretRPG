using System;

namespace kfutils
{
    public class Xorshift
    {
        private ulong seed;
        private ulong val;
        private float nextGaussian;


        public Xorshift(ulong seed)
        {
            val = this.seed = seed;
            nextGaussian = float.NaN;
        }


        public Xorshift()
        {
            val = seed = (ulong)System.DateTime.Now.Ticks;
            nextGaussian = float.NaN;
        }


        public ulong GetSeed()
        {
            return seed;
        }

        public bool NextBool()
        {
            return (NextUlong() & 0x1) == 1U;
        }

        public byte NextByte()
        {
            long v = NextLong();
            return (byte)(v >> (int)(((v >> 8) & 31) + 16) & 0xff);
        }

        public double NextDouble()
        {
            return ((double)NextUlong() / (double)ulong.MaxValue);
        }

        public float NextFloat()
        {
            return (float)((double)NextUlong() / (double)ulong.MaxValue);
        }


        public int NextInt()
        {
            return (int)(NextLong() & 0x7fffffff);
        }


        public int NextSInt()
        {
            return (int)(NextLong() & 0xffffffff);
        }


        public uint NextUint()
        {
            return (uint)(NextLong() & uint.MaxValue);
        }


        public int NextInt(int mod)
        {
            return (int)(NextLong() % mod);
        }


        public int NextInt(int min, int max)
        {
            return NextInt(1 + max - min) + min;
        }


        public long NextLong()
        {
            return (long)(NextUlong() & 0x7fffffffffffffffU);
        }


        public long NextSLong()
        {
            return (long)(NextUlong());
        }


        public ulong NextUlong()
        {
            val *= 5443;
            val += 1548586312338621L;
            val ^= val >> 19;
            val ^= val << 31;
            val ^= val >> 23;
            val ^= val << 7;
            return val;
        }

        public short NextShort()
        {
            return (short)(NextLong() & 0xffff);
        }


        public void Reset()
        {
            seed = val;
        }


        public ulong PeekVal()
        {
            return val;
        }


        public void SetSeed(ulong seed)
        {
            val = this.seed = seed;
        }


        public float NextGaussian()
        {
            if (float.IsNaN(nextGaussian))
            {
                float x1, x2, w;

                do
                {
                    x1 = 2f * NextFloat() - 1f;
                    x2 = 2f * NextFloat() - 1f;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = (float)Math.Sqrt((-2f * Math.Log(w)) / w);
                nextGaussian = x2 * w;
                return x1 * w;
            }
            else
            {
                float xw = nextGaussian;
                nextGaussian = float.NaN;
                return xw;
            }
        }

    }
}