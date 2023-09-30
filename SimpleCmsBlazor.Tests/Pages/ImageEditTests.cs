using SimpleCmsBlazor.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCmsBlazorTests.Pages;

public class ImageEditTests : BunitTestContext
{
    [Test]
    public void TestProjection()
    {
        var cut = RenderComponent<ImageEdit>();

        cut.Instance.ProcessImage();

        Assert.Pass();
    }
}
