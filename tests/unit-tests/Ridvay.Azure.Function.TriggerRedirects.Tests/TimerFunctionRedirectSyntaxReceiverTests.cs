using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Ridvay.Azure.Function.TriggerRedirects.TimerTrigger;

namespace Ridvay.Azure.Function.TriggerRedirects.Test;

public class TimerFunctionRedirectSyntaxReceiverTests
{
    private TimerFunctionRedirectSyntaxReceiver _syntaxReceiver;

    [SetUp]
    public void Setup()
    {
        _syntaxReceiver = new TimerFunctionRedirectSyntaxReceiver();
    }

    [Test]
    public void OnVisitSyntaxNode_WithFunctionTimerTriggerAttribute_FindsCandidateClass()
    {
        var sourceCode = @"
using Ridvay.Azure.Function.TriggerRedirects.TimerTrigger;

namespace TestNamespace
{
    [FunctionTimerTrigger(""FunctionName"", ""0 */5 * * * *"", typeof(SomeMessageType))]
    public class TestClassWithAttribute
    {
    }

    public class TestClassWithoutAttribute
    {
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = syntaxTree.GetCompilationUnitRoot();

        foreach (var node in root.DescendantNodes())
        {
            _syntaxReceiver.OnVisitSyntaxNode(node);
        }

        _syntaxReceiver.CandidateClasses.Should().HaveCount(1);
        _syntaxReceiver.CandidateClasses.First().Item1.Identifier.ToString().Should().Be("TestClassWithAttribute");
    }
}
