namespace SimpleCmsBlazorTests.Shared;

public class AdminContentItemTests : BunitTestContext
{

    [Test]
    public void CannotDropIntoChildOfSelf()
    {
        // Arrange
        var grandchild = new Page();
        var child = new Page();
        var item = new Page();
        var site = new Site();

        child.Pages.Add(grandchild);
        item.Pages.Add(child);
        site.Pages.Add(item);

        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        // Act
        var cut = RenderComponent<AdminContentItem>(parameters => parameters
           .Add(p => p.Node, grandchild)
           .Add(p => p.ActivePage, grandchild)           
        );
        var canDrop = cut.Instance.AcceptDrop(item);

        // Assert
        Assert.That(canDrop, Is.False);
    }

    [Test]
    public void CanDropIntoParentOfSelf()
    {
        // Arrange
        var grandchild = new Page();
        var child = new Page();
        var item = new Page();
        var site = new Site();

        child.Pages.Add(grandchild);
        item.Pages.Add(child);
        site.Pages.Add(item);

        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);
        
        // Act

        var cut = RenderComponent<AdminContentItem>(parameters => parameters
           .Add(p => p.Node, item)
           .Add(p => p.ActivePage, item)
        );
        var canDrop = cut.Instance.AcceptDrop(grandchild);

        // Assert
        Assert.That(canDrop, Is.True);
    }

    [Test]
    public void CanDropIntoChildOfOther()
    {
        // Arrange
        var grandchild = new Page();
        var child = new Page();
        var sibling = new Page();
        var item = new Page();       
        var site = new Site();

        child.Pages.Add(grandchild);
        item.Pages.Add(child);
        item.Pages.Add(sibling);
        site.Pages.Add(item);

        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        // Act
        var cut = RenderComponent<AdminContentItem>(parameters => parameters
           .Add(p => p.Node, grandchild)
           .Add(p => p.ActivePage, grandchild)
        );
        var canDrop = cut.Instance.AcceptDrop(sibling);

        // Assert
        Assert.That(canDrop, Is.True);
    }

    [Test]
    public void CanDropIntoSameListAsSelf()
    {
        // Arrange        
        var child = new Page();
        var sibling = new Page();
        var item = new Page();
        var site = new Site();

        item.Pages.Add(child);
        item.Pages.Add(sibling);
        site.Pages.Add(item);

        var dds = new DragDropService();
        TestContext?.Services.AddSingleton(dds);

        // Act
        var cut = RenderComponent<AdminContentItem>(parameters => parameters
           .Add(p => p.Node, item)
           .Add(p => p.ActivePage, child)
        );
        var canDrop = cut.Instance.AcceptDrop(sibling);

        // Assert
        Assert.That(canDrop, Is.True);
    }
}
