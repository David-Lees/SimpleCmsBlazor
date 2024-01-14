namespace SimpleCmsBlazorTests;

public abstract class BunitTestContext : TestContextWrapper
{
    [SetUp]
    public virtual void Setup() => TestContext = new Bunit.TestContext();

    [TearDown]
    public virtual void TearDown() => TestContext?.Dispose();
}