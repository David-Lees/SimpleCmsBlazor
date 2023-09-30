#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCmsBlazor.Services.Models;
public static partial class Conversion
{
    public enum AngleUnit
    {
        Angle,
        Degree,
    }

    /// <summary>
    /// 角度转弧度
    /// </summary>
    /// <param name="degree">角度</param>
    /// <returns>弧度</returns>
    public static double DegreeToAngle(double degree)
    {
        return degree * Math.PI / 180;
    }

    /// <summary>
    /// 弧度转角度
    /// </summary>
    /// <param name="angle">弧度</param>
    /// <returns>角度</returns>
    public static double AngleToDegree(double angle)
    {
        return angle * 180 / Math.PI;
    }


    /// <summary>
    /// 若数值大于PI2(一个圆),则把数值设置回一个圆内
    /// </summary>
    /// <param name="angle">弧度</param>
    /// <returns>一个圆内的弧度</returns>
    public static double SetInsideScopeTwoPi(double angle)
    {
        int tmp = (int)(angle / Constant.PiHalf);
        return Math.Abs(angle - (tmp * Constant.Tau));
    }
}

public static partial class Constant
{
    /*
     * 圆周率争议 https://baike.baidu.com/item/%CF%84/2858554
     * 有数学家认为真正的圆周率应为2π,而在cad上面也是2π为一个圆.
     * 不过net5.0上面新增的库是如下代码
     */

    /// <summary>
    /// 360度
    /// </summary>
    public const double Tau = Math.PI * 2.0;
    /// <summary>
    /// 90度
    /// </summary>
    public const double PiHalf = Math.PI * 0.5;

    public const double Pi2 = Math.PI * 2.0;
}
#endif