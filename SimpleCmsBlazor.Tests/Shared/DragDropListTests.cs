namespace SimpleCmsBlazorTests.Shared;

public class DragDropListTests : BunitTestContext
{
    private static Page GetMockData()
    {
        var root = new Page()
        {
            Id = "root",
            Name = "Root",
            Url = "/"
        };
        root.Pages.Add(new Page() { Id = "1", Name = "1", Url = "1" });
        root.Pages.Add(new Page() { Id = "2", Name = "2", Url = "2" });
        root.Pages.Add(new Page() { Id = "3", Name = "3", Url = "3" });
        root.Pages.Add(new Page() { Id = "4", Name = "4", Url = "4" });
        root.Pages.Add(new Page() { Id = "5", Name = "5", Url = "5" });
        root.Pages.Add(new Page() { Id = "6", Name = "6", Url = "6" });

        return root;
    }

    [Test]
    public async Task DropFromExternal_AtStart_InsertsCorrectlyAsync()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
        );

        // Act
        await cut.InvokeAsync(() => dds.SetData(new Page() { Id = "new", Name = "new", Url = "new" }));
        await cut.InvokeAsync(async() => await cut.Find(".drop-target").DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        Assert.That(ddl.Items, Has.Count.EqualTo(7));
        Assert.That(ddl.Items[0].Name, Is.EqualTo("new"));
    }

    [Test]
    public async Task DropFromExternal_AfterFirst_InsertsCorrectlyAsync()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
        );

        // Act
        await cut.InvokeAsync(() => dds.SetData(new Page() { Id = "new", Name = "new", Url = "new" }));
        await cut.InvokeAsync(async () => await cut.FindAll(".drop-target")[1].DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        Assert.That(ddl.Items, Has.Count.EqualTo(7));
        Assert.That(ddl.Items[1].Name, Is.EqualTo("new"));
    }

    [Test]
    public async Task MoveLastToFirstAsync()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
        );

        // Act
        await cut.InvokeAsync(async () => await cut.FindAll(".drag-item")[5].DragStartAsync(new()));
        await cut.InvokeAsync(async () => await cut.FindAll(".drop-target")[0].DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        var order = string.Join("", ddl.Items.Select(p => p.Name).ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(ddl.Items, Has.Count.EqualTo(6));
            Assert.That(order, Is.EqualTo("612345"));
        });
    }

    [Test]
    public async Task MoveLastToSecondAsync()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
        );

        // Act
        await cut.InvokeAsync(async () => await cut.FindAll(".drag-item")[5].DragStartAsync(new()));
        await cut.InvokeAsync(async () => await cut.FindAll(".drop-target")[1].DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        var order = string.Join("", ddl.Items.Select(p => p.Name).ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(ddl.Items, Has.Count.EqualTo(6));
            Assert.That(order, Is.EqualTo("162345"));
        });
    }

    [Test]
    public async Task MoveFirstToLastAsync()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
        );

        // Act
        await cut.InvokeAsync(async () => await cut.FindAll(".drag-item")[0].DragStartAsync(new()));
        await cut.InvokeAsync(async () => await cut.FindAll(".drop-target")[6].DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        var order = string.Join("", ddl.Items.Select(p => p.Name).ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(ddl.Items, Has.Count.EqualTo(6));
            Assert.That(order, Is.EqualTo("234561"));
        });
    }

    [Test]
    public async Task Move2To5Async()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
        );

        // Act
        await cut.InvokeAsync(async () => await cut.FindAll(".drag-item")[1].DragStartAsync(new()));
        await cut.InvokeAsync(async () => await cut.FindAll(".drop-target")[5].DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        var order = string.Join("", ddl.Items.Select(p => p.Name).ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(ddl.Items, Has.Count.EqualTo(6));
            Assert.That(order, Is.EqualTo("134526"));
        });
    }

    [Test]
    public async Task MoveLastToInsideFirstAsync()
    {
        // Arrange
        var root = GetMockData();
        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        var cut = RenderComponent<DragDropList<Page>>(parameters => parameters
            .Add(p => p.ItemTemplate, item => $"<p>{item.Name}</p>")
            .Add(p => p.Items, root.Pages)
            .Add(p => p.ChildName, "Pages")
        );

        // Act
        await cut.InvokeAsync(async () => await cut.FindAll(".drag-item")[5].DragStartAsync(new()));
        await cut.InvokeAsync(async () => await cut.FindAll(".drag-item")[0].DropAsync(new()));

        // Assert
        var ddl = cut.Instance;
        Assert.That(ddl.Items, Has.Count.EqualTo(5));
        Assert.That(ddl.Items[0].Pages, Has.Count.EqualTo(1));          
    }
}
