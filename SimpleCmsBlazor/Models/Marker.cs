using OpenCvSharp;

namespace SimpleCmsBlazor.Models;

public class Marker
{
    public double X { get; set; }
    public double Y { get; set; }

    public Marker(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Point2f ToPoint2f(decimal factorx, decimal factory)
    {
        return new Point2f((float)(X * (double)factorx), (float)(Y * (double)factory));
    }
}