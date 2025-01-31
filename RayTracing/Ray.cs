using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RayTracing;
using Point = RayTracing.Vec3;

public struct Ray
{
    public Ray() {_orig = new Point(); _dir = new Vec3();}
    public Ray(Point origin, Vec3 direction) 
    {
        _orig = origin;
        _dir = direction;
    }

    public Point at(double t)
    {
        return _orig + t * _dir;
    }

    public Point Origin { get { return _orig; } }
    public Vec3 Direction { get { return _dir; }}

    private Point _orig;
    private Vec3 _dir;
}
