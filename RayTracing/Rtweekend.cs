using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RayTracing;

public static class Rtweekend
{
    public static double DegToRad(double deg)
    {
        return deg * Math.PI / 180.0;
    } 
    public static double RandomDouble(double min = 0.0, double max = 1.0)
    {
        return min + (max - min) * Random.Shared.NextDouble();
    }

}
