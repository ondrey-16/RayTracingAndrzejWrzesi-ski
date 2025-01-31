using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RayTracing;
using Color = RayTracing.Vec3;
using Point = RayTracing.Vec3;

public class Program 
{
    public static async Task Main(string[] args)
    {
        HittableList world = new HittableList();

        var mat1 = new Metal(new Color(0.7, 0.6, 0.5), 0.0);
        var mat2 = new Lambertian(new Color(0.4, 0.2, 0.1));
        var mat3 = new Dielectric(1.5);
        world.Add(new Sphere(new Point(4, 1, 0), 1.0, mat1));
        world.Add(new Sphere(new Point(0, 1, 0), 1.0, mat3));
        world.Add(new Sphere(new Point(-4, 1, 0), 1.0, mat2));

        for (int b = 11; b > -11; b--)
        {
            for (int a = 11; a > -11; a--)
            {
                var chooseMat = Rtweekend.RandomDouble();
                Point center = new Point(a + 0.9 * Rtweekend.RandomDouble(), 0.2, b + 0.9 * Rtweekend.RandomDouble());

                if ((center - new Point(4, 0.2, 0)).Length() > 0.9)
                {
                    Material sphereMaterial = new Material();

                    if (chooseMat < 0.8)
                    {
                        var albedo = Color.Random() * Color.Random();
                        sphereMaterial = new Lambertian(albedo);
                        world.Add(new Sphere(center, 0.2, sphereMaterial));
                    }
                    else if (chooseMat < 0.95)
                    {
                        var albedo = Color.Random() * Color.Random();
                        var fuzz = Rtweekend.RandomDouble(0, 0.5);
                        sphereMaterial = new Metal(albedo, fuzz);
                        world.Add(new Sphere(center, 0.2, sphereMaterial));
                    }
                    else
                    {
                        sphereMaterial = new Dielectric(1.5);
                        world.Add(new Sphere(center, 0.2, sphereMaterial));
                    }
                }
            }
        }

        var materialGround = new Lambertian(new Color(0.5, 0.5, 0.5));
        world.Add(new Sphere(new Point(0, -1000, 0), 1000, materialGround));

        
        
        Camera cam = new Camera();

        cam.aspectRatio = 16.0 / 9.0;
        cam.imageWidth = 500;
        cam.samplesPerPixel = 50;
        cam.maxDepth = 10;

        cam.vfov = 20;
        cam.lookfrom = new Point(13, 2, 3);
        cam.lookat = new Point(0, 0, 0);
        cam.vup = new Vec3(0,1,0);

        cam.defocusAngle = 0.6;
        cam.focusDist = 10.0;

        await cam.Render(world);
    }
}