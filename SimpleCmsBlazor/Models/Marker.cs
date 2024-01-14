using OpenCvSharp;

namespace SimpleCmsBlazor.Models;

public class Marker(double x, double y)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;

    public Point2f ToPoint2f(decimal factorx, decimal factory)
    {
        return new Point2f((float)(X * (double)factorx), (float)(Y * (double)factory));
    }
}