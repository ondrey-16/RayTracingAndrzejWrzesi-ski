using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RayTracing;

public struct Interval
{
    public double Min;
    public double Max;

    public Interval() 
    {
        Min = Double.NegativeInfinity;
        Max = Double.PositiveInfinity;
    }
    public Interval(double _min, double _max)
    {
        Min = _min;
        Max = _max;
    }

    public bool Contains(double x)
    {
        return Min <= x && x <= Max;
    }

    public bool Surrounds(double x)
    {
        return Min < x && x < Max;
    }

    public double Clamp(double x)
    {
        if (x < Min) return Min;
        if (x > Max) return Max;
        return x;
    }
}

