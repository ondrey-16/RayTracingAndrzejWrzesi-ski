using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Point = RayTracing.Vec3;

namespace RayTracing;

public class HitRecord
{
    public Point p = new Point();
    public Vec3 normal = new Vec3();
    public Material mat = new Material();
    public double t;
    public bool frontFace;

    public void setFaceNormal(Ray r, Vec3 outwardNormal)
    {
        frontFace = Vec3.Dot(r.Direction, outwardNormal) < 0;
        normal = frontFace ? outwardNormal : -outwardNormal;
    }
}

public abstract class Hittable
{
    public abstract bool hit(ref Ray r, Interval rayT, ref HitRecord rec);
}
