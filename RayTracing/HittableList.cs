using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RayTracing;

public class HittableList : Hittable
{
    public List<Hittable> Objects = new List<Hittable>();
    public HittableList() {}
    public HittableList(Hittable obj)
    {
        Add(obj);
    }
    public void Clear()
    {
        Objects.Clear();
    }

    public void Add(Hittable obj)
    {
        Objects.Add(obj);
    }

    public override bool hit(ref Ray r, Interval rayT, ref HitRecord rec)
    {
        bool hitAnything = false;
        var closestSoFar = rayT.Max;

        foreach (Hittable obj in Objects)
        {
            if (obj.hit(ref r, new Interval(rayT.Min, closestSoFar), ref rec))
            {
                hitAnything = true;
                closestSoFar = rec.t;
            }
        }

        return hitAnything;
    }
}
