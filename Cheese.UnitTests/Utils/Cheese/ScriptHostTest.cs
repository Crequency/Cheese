using Cheese.Utils.General;

namespace Cheese.UnitTests.Utils.Cheese;

[TestFixture]
[TestOf(typeof(ScriptHost))]
public class ScriptHostTest
{
    [Test]
    public void NamespaceAccessibilityTest()
    {
        var code = """
                   using Cheese.Utils.General;
                   
                   ConsoleHelper.Instance.WriteLine("Test");
                   
                   return "Done";
                   """;

        var task = ScriptHost.Instance.ExecuteCodesAsync(code, false);

        task.Wait();
        
        Assert.That(task.Result, Is.EqualTo("Done"));
    }
}