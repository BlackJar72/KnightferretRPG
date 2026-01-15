using UnityEngine;


namespace kfutils.noise {

    /// <summary>
    /// A simple, alternate Vector2 representation, used exclusively for my own noise functions.
    /// The core useful feature in this copacity is simply that is hase self-generation specifically
    /// geared toward my noise functions.
    /// </summary>
    public struct Vec2D {
        public const double P2 = Mathf.PI * 2.0;
        public double x, y;

        public Vec2D(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Vec2D(bool randomize) {
                x = Random.value * 2.0 - 1.0;
                y = Random.value * 2.0 - 1.0;
        }

        public Vec2D(SpatialHash random, int px, int py, int pz) {
            x = random.DoubleFor(px, py, pz) * 2.0 - 1.0;
            y = random.DoubleFor(px, py, pz + 1) * 2.0 - 1.0;
        }

        public Vec2D(SpatialHash random, int px, int py, int pz, int t) {
            x = random.DoubleFor(px, py, pz, t) * 2.0 - 1.0;
            y = random.DoubleFor(px, py, pz + 1, t) * 2.0 - 1.0;
        }

        public static double Dot(Vec2D a, Vec2D b) {
            return (a.x * b.x) + (a.y * b.y);
        }

    }

}

