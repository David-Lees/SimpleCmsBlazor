using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace SimpleCmsBlazor.Shared;
public partial class ColourContrast
{
    [Parameter]
    public string BackgroundColour { get; set; } = string.Empty;

    [Parameter]
    public string TextColour { get; set; } = string.Empty;

    public float Contrast { get; set; } = 0.0f;

    protected override void OnParametersSet()
    {
        Update();
    }

    private void Update()
    {
        var text = FromHtml(TextColour);
        var back = FromHtml(BackgroundColour);
        Contrast = CalculateColourContrast(text, back);
        StateHasChanged();
    }

    private static Color FromHtml(string c)
    {
        if (c.StartsWith("rgba("))
        {
            var nums = c.Replace("rgba(", "").Replace(")", "").Split(',').Select(x => float.Parse(x)).ToList();
            return Color.FromArgb((int)(nums[3] * 255), (int)nums[0], (int)nums[1], (int)nums[2]);
        }
        if (c.StartsWith("rgb("))
        {
            var nums = c.Replace("rgb(", "").Replace(")", "").Split(',').Select(x => float.Parse(x)).ToList();
            return Color.FromArgb(255, (int)nums[0], (int)nums[1], (int)nums[2]);
        }
        return ColorTranslator.FromHtml(c);
    }

    private static float RelativeLuminance(Color color)
    {
        static float ColourPartValue(float part)
        {
            return part <= 0.03928f ? part / 12.92f : MathF.Pow((part + 0.055f) / 1.055f, 2.4f);
        }
        var r = ColourPartValue((float)color.R / 255.0f);
        var g = ColourPartValue((float)color.G / 255.0f);
        var b = ColourPartValue((float)color.B / 255.0f);

        var l = 0.2126f * r + 0.7152f * g + 0.0722f * b;
        return l;
    }

    private static float CalculateColourContrast(Color a, Color b)
    {
        var La = RelativeLuminance(a) + 0.05f;
        var Lb = RelativeLuminance(b) + 0.05f;

        float result = MathF.Max(La, Lb) / MathF.Min(La, Lb);

        return result;
    }
}
