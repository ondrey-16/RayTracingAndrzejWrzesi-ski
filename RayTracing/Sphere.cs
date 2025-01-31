using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Point = RayTracing.Vec3;

namespace RayTracing;

public class Sphere : Hittable
{
    public Sphere(Point center, double radius, Material mat)
    {
        _center = center;
        _radius = Math.Max(radius, 0);
        _mat = mat;
    }

    public override bool hit(ref Ray r, Interval rayT, ref HitRecord rec)
    {
        Vec3 oc = _center - r.Origin;
        var a = r.Direction.LengthSquared();
        var h = Vec3.Dot(r.Direction, oc);
        var c = oc.LengthSquared() - _radius*_radius;

        var discriminant = h*h - a*c;
        if (discriminant < 0)
        {
            return false;
        }

        var sqrtd = Math.Sqrt(discriminant);

        var root = (h - sqrtd) / a;
        if (!rayT.Surrounds(root))
        {
            root = (h +sqrtd) / a;
            if (!rayT.Surrounds(root))
            {
                return false;
            }
        }

        rec.t = root;
        rec.p = r.at(rec.t);
        Vec3 outwardNormal = (rec.p - _center) / _radius;
        rec.setFaceNormal(r, outwardNormal);
        rec.mat = _mat;

        return true;
    }

    private Point _center;
    private double _radius;
    private Material _mat;
}
