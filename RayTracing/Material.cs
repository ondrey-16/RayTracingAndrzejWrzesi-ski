using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Color = RayTracing.Vec3;
using Point = RayTracing.Vec3;

namespace RayTracing;

public class Material
{
    public virtual bool Scatter(Ray rIn, HitRecord rec, ref Color attenuation,
        ref Ray scattered)
    {
        return false;
    } 
};

public class Lambertian : Material
{
    public Lambertian(Color albedo)
    {
        _albedo = albedo;
    }
    public override bool Scatter(Ray rIn, HitRecord rec, ref Color attenuation,
        ref Ray scattered)
    {
        var scatterDirection = rec.normal + Vec3.RandomUnitVector();

        if (scatterDirection.NearZero())
            scatterDirection = rec.normal;

        scattered = new Ray(rec.p, scatterDirection);
        attenuation = _albedo;
        return true;
    } 
    private Color _albedo;
};

public class Metal : Material
{
    public Metal(Color albedo, double fuzz)
    {
        _albedo = albedo;
        _fuzz = (fuzz < 1) ? fuzz : 1;
    }
    public override bool Scatter(Ray rIn, HitRecord rec, ref Color attenuation,
        ref Ray scattered)
    {
        Vec3 reflected = Vec3.Reflect(rIn.Direction, rec.normal);
        reflected = Vec3.UnitVector(reflected) + (_fuzz * Vec3.RandomUnitVector());
        scattered = new Ray(rec.p, reflected);
        attenuation = _albedo;
        return (Vec3.Dot(scattered.Direction, rec.normal) > 0);
    } 
    private Color _albedo;
    private double _fuzz;
};

public class Dielectric : Material
{
    public Dielectric(double refractionIndex)
    {
        _refractionIndex = refractionIndex;
    }
    public override bool Scatter(Ray rIn, HitRecord rec, ref Color attenuation,
        ref Ray scattered)
    {
        attenuation = new Color(1.0, 1.0, 1.0);
        double ri = rec.frontFace ? (1.0/_refractionIndex) : _refractionIndex;

        Vec3 unitDirection = Vec3.UnitVector(rIn.Direction);
        double cosTheta = Math.Min(Vec3.Dot(unitDirection * -1, rec.normal), 1.0);
        double sinTheta = Math.Sqrt(1.0 - cosTheta*cosTheta);

        bool cannotRefract = (ri * sinTheta) > 1.0;
        Vec3 direction = new Vec3();

        if (cannotRefract || Reflectance(cosTheta, ri) > Rtweekend.RandomDouble())
            direction = Vec3.Reflect(unitDirection, rec.normal);
        else
            direction = Vec3.Refract(unitDirection, rec.normal, ri);

        scattered = new Ray(rec.p, direction);
        return true;
    } 
    private double _refractionIndex;
    private static double Reflectance(double cosine, double ri)
    {
        double r0 = (1.0 - ri) / (1.0 + ri);
        r0 = r0 * r0;
        return r0 + (double)(1.0-r0) * Math.Pow((double)(1.0-cosine),5.0);
    }
};
