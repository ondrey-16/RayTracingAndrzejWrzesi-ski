using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RayTracing;

public struct Vec3
{
    public double[] e = new double[3];

    public Vec3() {}
    public Vec3(double e0, double e1, double e2) { e[0] = e0; e[1] = e1; e[2] = e2; }

    public double x {get { return e[0]; }}
    public double y {get { return e[1]; }}
    public double z {get { return e[2]; }}

    public static Vec3 operator-(Vec3 v)
    {
        return new Vec3(-v.e[0], -v.e[1], -v.e[2]);
    }

    public double this[int i]
    {
        set { e[i] = value; }
        get { return e[i]; }
    }

    public static Vec3 operator+(Vec3 a, Vec3 b)
    {
        return new Vec3(a.e[0] + b.e[0], 
        a.e[1] + b.e[1], a.e[2] + b.e[2]);
    }

    public static Vec3 operator+(Vec3 a, double b)
    {
        return new Vec3(a.e[0] + b,
        a.e[1] + b, a.e[2] + b);
    }

    public static Vec3 operator-(Vec3 a, Vec3 b)
    {
        return new Vec3(a.e[0] - b.e[0], 
        a.e[1] - b.e[1], a.e[2] - b.e[2]);
    }

     public static Vec3 operator-(Vec3 a, double b)
    {
        return new Vec3(a.e[0] - b,
        a.e[1] - b, a.e[2] - b);
    }

    public static Vec3 operator*(Vec3 a, Vec3 b)
    {
        return new Vec3(a.e[0] * b.e[0], 
        a.e[1] * b.e[1], a.e[2] * b.e[2]);
    }
    
    public static Vec3 operator*(Vec3 a, double t)
    {
        return new Vec3(a.e[0] * t, a.e[1] * t, a.e[2] * t);
    }

    public static Vec3 operator*(double t, Vec3 a)
    {
        return new Vec3(a.e[0] * t, a.e[1] * t, a.e[2] * t);
    }

    public static Vec3 operator/(Vec3 a, double t)
    {
        return new Vec3(a.e[0] / t, a.e[1] / t, a.e[2] / t);
    }

    public static double Dot(Vec3 a, Vec3 b)
    {
        return a.e[0] * b.e[0] +
        a.e[1] * b.e[1] + a.e[2] * b.e[2];
    }

    public static Vec3 Cross(Vec3 a, Vec3 b)
    {
        return new Vec3(a.e[1] * b.e[2] - a.e[2] * b.e[1],
        a.e[2] * b.e[0] - a.e[0] * b.e[2],
        a.e[0] * b.e[1] - a.e[1] * b.e[0]);
    }

    public double LengthSquared()
    {
        return e[0] * e[0] + e[1] * e[1] + e[2] * e[2];
    }
    public bool NearZero()
    {
        var s = 1e-8;
        return (Math.Abs(e[0]) < s) && (Math.Abs(e[1]) < s) && (Math.Abs(e[2]) < s);
    } 
    public static Vec3 Random()
    {
        return new Vec3(Rtweekend.RandomDouble(), Rtweekend.RandomDouble(), 
        Rtweekend.RandomDouble());
    }
    public static Vec3 Random(double min, double max)
    {
        return new Vec3(Rtweekend.RandomDouble(min, max), Rtweekend.RandomDouble(min, max), 
        Rtweekend.RandomDouble(min, max));
    }
    public double Length()
    {
        return Math.Sqrt(LengthSquared());
    }
    public static Vec3 UnitVector(Vec3 v)
    {
        return v / v.Length();
    }
    public static Vec3 RandomUnitVector()
    {
        while(true)
        {
            var p = Vec3.Random(-1,1);
            var lensq = p.LengthSquared();
            if (1e-160 < lensq && lensq<= 1)
                return p / Math.Sqrt(lensq);
        }
    }
    public static Vec3 RandomOnHemisphere(Vec3 normal)
    {
        Vec3 onUnitSphere = Vec3.RandomUnitVector();
        if (Vec3.Dot(onUnitSphere, normal) > 0.0)
            return onUnitSphere;
        else
            return -onUnitSphere;
    }
    public static Vec3 Reflect(Vec3 v, Vec3 n)
    {
        return v - 2*Vec3.Dot(v,n)*n;
    }

    public static Vec3 Refract(Vec3 uv, Vec3 n, double etaiOverEtat)
    {
        double cosTheta = Math.Min(Vec3.Dot(-uv, n), 1.0);
        Vec3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
        double lengthSquared = rOutPerp.LengthSquared();
        if (lengthSquared > 1.0)
        {
            lengthSquared = 1.0; // Zabezpieczenie przed przekroczeniem 1
        }
        Vec3 rOutParallel = -Math.Sqrt(Math.Max(0.0, 1.0 - lengthSquared)) * n;
        return rOutPerp + rOutParallel;
    }

    public static Vec3 RandomInUnitDisk()
    {
        while (true)
        {
            var p = new Vec3(Rtweekend.RandomDouble(-1,1), Rtweekend.RandomDouble(-1,1), 0);
            if (p.LengthSquared() < 1)
                return p;
        }
    }
    public override string ToString()
    {
        return $"{e[0]} {e[1]} {e[2]}";
    }
};


