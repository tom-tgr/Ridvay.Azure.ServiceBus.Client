using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ridvay.Azure.Function.TriggerRedirects.TimerTrigger;

namespace Ridvay.Azure.Function.TriggerRedirects.Test;

public class TimerFunctionRedirectGeneratorTests
{
  
    [Test]
    public void GetAttributeValues_ReturnsCorrectValues()
    {
        var source = @"
using Ridvay.Azure.Function.TriggerRedirects.TimerTrigger;
using System;

namespace TestNamespace
{
    [FunctionTimerTrigger(""MySupperTimerTrigger"", ""5 * * * * *"", typeof(BasicMessage))]
    public class TestClass
    {
    }

    public class BasicMessage
    {
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create("TestAssembly",
            new[] {syntaxTree},
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TimerFunctionRedirectGenerator).Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var attributeSyntax = syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>().First();
        var generator = new TimerFunctionRedirectGenerator();

        var (functionName, timerCron, messageTypeNamespace, messageTypeName) = 
            generator.GetAttributeValues(attributeSyntax, semanticModel);

        functionName.Should().Be("MySupperTimerTrigger");
        timerCron.Should().Be("5 * * * * *");
        messageTypeNamespace.Should().Be("TestNamespace");
        messageTypeName.Should().Be("global::TestNamespace.BasicMessage");
    }
    
    [Test]
    public void GetNamespaceName_ReturnsCorrectNamespaceName_ForNamespaceDeclarationSyntax()
    {
        var source = @"
namespace TestNamespace
{
    public class TestClass
    {
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var namespaceDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>().First();
        var generator = new TimerFunctionRedirectGenerator();

        var namespaceName = generator.GetNamespaceName(namespaceDeclaration);

        namespaceName.Should().Be("TestNamespace");
    }
    
    [Test]
    public void GetNamespaceName_ReturnsCorrectNamespaceName_ForFileScopedNamespaceDeclarationSyntax()
    {
        var source = @"
namespace TestNamespace;

public class TestClass
{
}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var fileScopedNamespaceDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().First();
        var generator = new TimerFunctionRedirectGenerator();

        var namespaceName = generator.GetNamespaceName(fileScopedNamespaceDeclaration);

        namespaceName.Should().Be("TestNamespace");
    }
    
    [Test]
    public void Execute_GeneratesExpectedFunction_WhenValidSyntaxReceiverData2()
    {
        var sourceCode = @"
using Ridvay.Azure.Function.TriggerRedirects.TimerTrigger;
using System;

namespace TestNamespace
{
    [FunctionTimerTrigger(""MyFunction"", ""0 * * * * *"", typeof(MyMessage))]
    public class TestFunction
    {
    }

    public class MyMessage
    {
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var generator = new TimerFunctionRedirectGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilation = CSharpCompilation.Create("TestCompilation", new[] { syntaxTree }, references);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
        var generatedFile = updatedCompilation.SyntaxTrees.FirstOrDefault(a=>a.FilePath.Contains("MyFunctionFunction.cs"));

        diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error).Should().BeFalse();
        generatedFile.Should().NotBeNull();
        var generatedCode = generatedFile?.GetText().ToString();
        generatedCode.Should().Contain("public async Task Run([TimerTrigger(\"0 * * * * *\")] TimerInfo timer)");
    }

}


