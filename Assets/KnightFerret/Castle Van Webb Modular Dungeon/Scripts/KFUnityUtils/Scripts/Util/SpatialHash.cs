using System;

namespace kfutils {

    public class SpatialHash {
        public const int INT_MASK = 0x7fffffff;
        public const ulong LONG_MASK = 0x7fffffffffffffffUL;
        public const double MAX_LONG_D = (double)long.MaxValue;
        public const float MAX_LONG_F = (float)long.MaxValue;
        public const double MAX_ULONG_D = (double)ulong.MaxValue;
        public const float MAX_ULONG_F = (float)ulong.MaxValue;
        private readonly ulong seed1;
        protected readonly ulong seed2;


        /*=====================================*
         * CONSTRUCTORS & BASIC CLASS METHODS  *
         *=====================================*/


        public SpatialHash() {
            ulong theSeed = (ulong)System.DateTime.Now.Ticks;
            seed1 = theSeed;
            seed2 = GenSecondSeed(theSeed);
        }


        public SpatialHash(ulong theSeed) {
            seed1 = theSeed;
            seed2 = GenSecondSeed(theSeed);
        }


        public SpatialHash(ulong theSeed, ulong altSeed) {
            seed1 = theSeed;
            seed2 = altSeed;
        }


        public static bool operator ==(SpatialHash one, SpatialHash other)
                => ((one.seed1 == other.seed1) && (one.seed2 == other.seed2));


        public static bool operator !=(SpatialHash one, SpatialHash other)
                => !(one == other);


        private ulong GenSecondSeed(ulong val) {
            val *= 5443;
            val += 1548586312338621L;
            val ^= val >> 19;
            val ^= val << 31;
            val ^= val >> 23;
            val ^= val << 7;
            return val;
        }


        public override int GetHashCode() {
            return (int)seed1;
        }


        public bool Equals(SpatialHash other) {
            return ((seed1 == other.seed1) && (seed2 == other.seed2));
        }


        override public bool Equals(Object other) {
            if(other == null || !(other is SpatialHash)) {
                return false;
            }
            return ((seed1 == ((SpatialHash)other).seed1) && (seed2 == ((SpatialHash)other).seed2));
        }


        protected ulong[] GetSeed() {
            return new ulong[] {seed1, seed2};
        }


        /*====================================*
         *  NON-STATIC METHODS USING THE SEED *
         *====================================*/


        /**
         * Generate a boolean from a given seed and coords.
         *
         * @param
         * @param z
         * @param t a fake iteration
         * @return
         */
        public bool BoolFor(int x, int z, int t) {
            return ((UlongFor(x, z, t) & 1) == 1);
        }


        /**
         * Generate a float from a given seed and coords.
         *
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public float FloatFor(int x, int y, int z, int t) {
            return (((float)UlongFor(x, y, z, t)) / MAX_ULONG_F);
        }


        /**
         * Generate a float from a given seed and coords.
         *
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public float FloatFor(int x, int z, int t) {
            return (((float)UlongFor(x, z, t)) / MAX_ULONG_F);
        }


        /**
         * Generate a double from a given seed and coords.
         *
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public double DoubleFor(int x, int y, int z, int t) {
            return (((double)UlongFor(x, y, z, t)) / MAX_ULONG_D);
        }


        /**
         * Generate a double from a given seed and coords.
         *
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public double DoubleFor(int x, int z, int t) {
            return (((double)UlongFor(x, z, t)) / MAX_ULONG_D);
        }


        /**
         * Should produce a random int from seed at coordinates x, y, t
         *
         * @param seed
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public int IntFor(int x, int z, int t) {
            return (int)UlongFor(x, z, t);
        }


        /**
         * Should produce a random int from seed at coordinates x, y, t
         *
         * @param seed
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public int IntFor(int x, int y, int z, int t) {
            return (int)UlongFor(x, y, z, t);
        }


        /**
         * Should produce a random int from seed at coordinates x, y, t
         *
         * @param seed
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public int PintFor(int x, int z, int t) {
            return (int)(UlongFor(x, z, t) & INT_MASK);
        }


        /**
         * Should produce a random int from seed at coordinates x, y, t
         *
         * @param seed
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public int PintFor(int x, int y, int z, int t) {
            return (int)(UlongFor(x, y, z, t) & INT_MASK);
        }


        /**
         * Should produce a random long from seed at coordinates x, y, t
         *
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public ulong UlongFor(int x, int z, int t) {
            ulong put = seed1 + (15485077UL * (ulong)t)
            + (12338621UL * (ulong)x)
            + (14416417UL * (ulong)z);
            ulong alt = seed2 + (179424743UL * (ulong)t)
            + (179426003UL * (ulong)x)
            + (179425819UL * (ulong)z);
            alt ^= rotateLeft(alt, (x % 29) + 13);
            alt ^= rotateRight(alt, (z % 31) + 7);
            alt ^= rotateLeft(alt, (t % 23) + 19);
            alt ^= rotateRight(alt, (z % 31) + 7);
            put ^= rotateLeft(put, ((x & INT_MASK) % 13) + 5);
            put ^= rotateRight(put, ((z & INT_MASK) % 11) + 28);
            put ^= rotateLeft(put, ((t & INT_MASK) % 17) + 45);
            return (put ^ alt);
        }


        /**
         * Should produce a random long from seed at coordinates x, y, t
         *
         * @param x
         * @param z
         * @param t a fake iteration
         * @return
         */
        public ulong UlongFor(int x, int y, int z, int t) {
            ulong put = seed1 + (15485077UL * (ulong)t)
            + (12338621UL * (ulong)x)
            + (15495631UL * (ulong)y)
            + (14416417UL * (ulong)z);
            ulong alt = seed2 + (179424743UL * (ulong)t)
            + (179426003UL * (ulong)x)
            + (29556049UL * (ulong)y)
            + (179425819UL * (ulong)z);
            alt ^= rotateLeft(alt, (x % 29) + 13);
            alt ^= rotateRight(alt, (z % 31) + 7);
            alt ^= rotateLeft(alt, (t % 23) + 19);
            alt ^= rotateRight(alt, (y % 7) + 7);
            put ^= rotateLeft(put, ((x & INT_MASK) % 13) + 5);
            put ^= rotateRight(put, ((z & INT_MASK) % 11) + 28);
            put ^= rotateLeft(put, ((y & INT_MASK) % 23) + 37);
            return (put ^ alt);
        }


        public long LongFor(int x, int y, int t) {
            return (long)UlongFor(x, y, t);
        }


        public long LongFor(int x, int y, int z, int t) {
            return (long)UlongFor(x, y, z, t);
        }


        public long PlongFor(int x, int y, int t) {
            return (long)(UlongFor(x, y, t) & LONG_MASK);
        }


        public long PlongFor(int x, int y, int z, int t) {
            return (long)(UlongFor(x, y, z, t) & LONG_MASK);
        }


        /*=============================*
         *  INTERNAL UNTILITY METHODS  *
         ==============================*/


        /**
         * Performs left bit shift (<<) with wrap-around.
         *
         * @param in
         * @param dist
         * @return
         */
        private static ulong rotateLeft(ulong val, int dist) {
            return (val << dist) | (val >> (64 - dist));
        }


        /**
         * Performs right bit shift (>>) with wrap-around.
         *
         * @param val
         * @param dist
         * @return
         */
        private static ulong rotateRight(ulong val, int dist) {
            return (val >> dist) | (val << (64 - dist));
        }


        public Xorshift GetXorshift(int x, int y, int t) {
            return new Xorshift(UlongFor(x, y, t));
        }


        public Xorshift GetXorshift(int x, int y, int z, int t) {
            return new Xorshift(UlongFor(x, y, z, t));
        }


    }

}
