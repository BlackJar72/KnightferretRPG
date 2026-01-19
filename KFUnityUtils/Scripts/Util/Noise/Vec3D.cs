using UnityEngine;


namespace kfutils.noise {

    /// <summary>
    /// A simple, alternate Vector2 representation, used exclusively for my own noise functions.
    /// The core useful feature in this copacity is simply that is hase self-generation specifically
    /// geared toward my noise functions.
    /// </summary>
    public struct Vec3D {
        public const double P2 = Mathf.PI * 2.0;
        public double x, y, z;

        public Vec3D(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public Vec3D(bool randomize) {
                x = Random.value * 2.0 - 1.0;
                y = Random.value * 2.0 - 1.0;
                z = Random.value * 2.0 - 1.0;
        }


        public Vec3D(SpatialHash random, int px, int py, int pz, int t) {
            x = random.DoubleFor(px, py, pz, t) * 2.0 - 1.0;
            y = random.DoubleFor(px, py, pz, t + 1) * 2.0 - 1.0;
            z = random.DoubleFor(px, py, pz, t + 2) * 2.0 - 1.0;
        }


        public static double Dot(Vec3D a, Vec3D b) {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }


        public double Dot(Vec3D b) {
            return (x * b.x) + (y * b.y) + (z * b.z);
        }

    }

}

