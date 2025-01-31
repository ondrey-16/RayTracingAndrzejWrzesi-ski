using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ImSh = SixLabors.ImageSharp;

using Color = RayTracing.Vec3;
using Point = RayTracing.Vec3;

namespace RayTracing;

public struct JpegColor
{
    public byte R;
    public byte G;
    public byte B;
}
public class Camera
{
    public double aspectRatio = 1.0;
    public int imageWidth = 400;
    public int samplesPerPixel = 10;
    public int maxDepth = 10;
    public double vfov = 90;
    public Point lookfrom = new Point(0,0,0);
    public Point lookat = new Point(0,0,-1);
    public Vec3 vup = new Vec3(0,1,0);
    public double defocusAngle = 0;
    public double focusDist = 10;
    public async Task Render(Hittable world)
    {
        Initialize();

        Color[] image = new Color[imageHeight * imageWidth];

        var tasks = Enumerable.Range(0, imageHeight).Select(row =>
        Task.Run(() =>
        {
            for (int i = 0; i < imageWidth; i++)
            {
                Color pixelColor = new Color(0, 0, 0);

                for (int sample = 0; sample < samplesPerPixel; sample++)
                {
                    Ray r = GetRay(i, row);
                    pixelColor += RayColor(r, maxDepth, world);
                }

                image[row * imageWidth + i] = pixelSamplesScale * pixelColor;
            }
            })
        ).ToList();
        await Task.WhenAll(tasks);

        var processedImage = WriteColor(image, imageWidth, imageHeight);

        Output(processedImage, imageWidth, imageHeight);

        Console.Error.Write($"\rDone.\n");
    }

    private int imageHeight;
    private double pixelSamplesScale;
    private Point cameraCenter = new Point();
    private Point pixel00Loc = new Point();
    private Vec3 pixelDeltaU = new Vec3();
    private Vec3 pixelDeltaV = new Vec3();
    private static readonly Interval intensity = new Interval(0.000, 0.999);
    private Vec3 u = new Vec3();
    private Vec3 v = new Vec3();
    private Vec3 w = new Vec3();
    private Vec3 defocusDiskU = new Vec3();
    private Vec3 defocusDiskV = new Vec3();

    private void Initialize()
    {
        imageHeight = (int)(imageWidth / aspectRatio);
        imageHeight = (imageHeight < 1) ? 1 : imageHeight;

        pixelSamplesScale = 1.0 / samplesPerPixel;

        cameraCenter = lookfrom;
        var theta = Rtweekend.DegToRad(vfov);
        var h = Math.Tan(theta/2);
        var viewportHeight = 2.0 * h * focusDist;
        var viewportWidth = viewportHeight * ((double)(imageWidth) / imageHeight); 

        w = Vec3.UnitVector(lookfrom - lookat);
        u = Vec3.UnitVector(Vec3.Cross(vup,w));
        v = Vec3.Cross(w,u);

        var viewportU = viewportWidth * u;
        var viewportV = viewportHeight * -v;

        pixelDeltaU = viewportU / imageWidth;
        pixelDeltaV = viewportV / imageHeight;

        var viewportUpperLeft = cameraCenter - (focusDist * w) 
            - viewportU/2 - viewportV/2;
        pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV);

        var defocusRadius = focusDist * Math.Tan(Rtweekend.DegToRad(defocusAngle/2));
        defocusDiskU = u * defocusRadius;
        defocusDiskV = v * defocusRadius;
    }

    private Ray GetRay(int i, int j)
    {
        var offset = SampleSquare();
        var pixelSample = pixel00Loc + ((i + offset.x) * pixelDeltaU) + ((j + offset.y) * pixelDeltaV);
        
        var rayOrigin = (defocusAngle <= 0) ? cameraCenter : DefocusDiskSample();
        var rayDirection = pixelSample - rayOrigin;

        return new Ray(rayOrigin, rayDirection);
    }

    private Color RayColor(Ray r, int depth, Hittable world)
    {
        if (depth <= 0)
            return new Color(0, 0, 0);

        HitRecord rec = new HitRecord();
        if (world.hit(ref r, new Interval(0.01, Double.PositiveInfinity), ref rec))
        {
            Ray scattered = new Ray();
            Color attenuation = new Color();
            if (rec.mat.Scatter(r, rec, ref attenuation, ref scattered))
            {
                return attenuation * RayColor(scattered, depth - 1, world);
            }
            return new Color(0, 0, 0);
        }

        Vec3 unitDirection = Vec3.UnitVector(r.Direction);
        var a = 0.5 * (unitDirection.y + 1.0);
        return (1.0 - a) * new Color(1.0, 1.0, 1.0) + a * new Color(0.5, 0.7, 1.0);
    }

    private Point DefocusDiskSample()
    {
        var p = Vec3.RandomInUnitDisk();
        return cameraCenter + (p[0] * defocusDiskU) + (p[1] * defocusDiskV);
    }

    private Vec3 SampleSquare()
    {
        return new Vec3(Rtweekend.RandomDouble() - 0.5, Rtweekend.RandomDouble() - 0.5,
        0);
    }
    private double LinearToGamma(double linearComponent)
    {
        if (linearComponent > 0)
            return Math.Sqrt(linearComponent);

        return 0;
    }
    private JpegColor[] WriteColor(Color[] unprocessedImage, int width, int height)
    {
        JpegColor[] processedImage = new JpegColor[width * height];

        for (int j = 0; j < height; j++)
        {
            for (int k = 0; k < width; k++)
            {
                var R = unprocessedImage[j * width + k].x;
                var G = unprocessedImage[j * width + k].y;
                var B = unprocessedImage[j * width + k].z;
                R = LinearToGamma(R);
                G = LinearToGamma(G);
                B = LinearToGamma(B);

                processedImage[j * width + k].R = (byte)(256 * intensity.Clamp(R));
                processedImage[j * width + k].G = (byte)(256 * intensity.Clamp(G));
                processedImage[j * width + k].B = (byte)(256 * intensity.Clamp(B));
            }
        }

        return processedImage;
    }

    private static void Output(JpegColor[] array, int width, int height)
    {
        ImSh.Image<ImSh::PixelFormats.Rgb24> image = new(width, height);
        image.DangerousTryGetSinglePixelMemory(out Memory<ImSh::PixelFormats.Rgb24> memory);
        var span = memory.Span;

        for (int j = 0; j < height; j++)
        {
            for (int k = 0; k < width; k++)
            {
                span[j * width + k].R = (byte)array[j * width + k].R;
                span[j * width + k].G = (byte)array[j * width + k].G;
                span[j * width + k].B = (byte)array[j * width + k].B;
            }
        }
        string fileName = $"./FinalImage.jpeg";
        ImSh.Formats.Jpeg.JpegEncoder encoder = new();
        using FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
        encoder.Encode(image, fs);
        image.Dispose();
    }
} 