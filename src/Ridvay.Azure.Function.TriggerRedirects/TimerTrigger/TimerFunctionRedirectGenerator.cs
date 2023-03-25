using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Ridvay.Azure.Function.TriggerRedirects.TimerTrigger
{

    [Generator]
    public class TimerFunctionRedirectGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
           context.RegisterForSyntaxNotifications(() => new TimerFunctionRedirectSyntaxReceiver());
        }
        
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            // Get the syntax receiver containing the collected information
            if (context.SyntaxReceiver is not TimerFunctionRedirectSyntaxReceiver receiver)
                return;
            
            foreach (var (classDeclaration, attributeSyntax) in receiver.CandidateClasses)
            {
                var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);

                var (functionName, timerCron, messageTypeNamespace, messageTypeName) = GetAttributeValues(attributeSyntax, semanticModel);
                
                
                if (string.IsNullOrEmpty(messageTypeName) || string.IsNullOrEmpty(messageTypeNamespace) 
                                                          || string.IsNullOrEmpty(functionName)) continue;

                var namespaceDeclaration = classDeclaration.Parent;
                
                if (namespaceDeclaration == null) continue;

                var namespaceName = GetNamespaceName(namespaceDeclaration);
                if (string.IsNullOrEmpty(namespaceName)) continue;

                var source = GenerateFunctionSource(namespaceName!, messageTypeNamespace!, messageTypeName!, functionName!, timerCron ?? "%TimerTrigger%");
                context.AddSource($"{functionName}Function", SourceText.From(source, Encoding.UTF8));
            }
        }

        internal (string? functionName, string? timerCron, string? messageTypeNamespace, string? messageTypeName) GetAttributeValues(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            if (attributeSyntax.ArgumentList != null)
            {
                var functionName = semanticModel.GetConstantValue(attributeSyntax.ArgumentList.Arguments[0].Expression).Value?.ToString();
                var timerCron = semanticModel.GetConstantValue(attributeSyntax.ArgumentList.Arguments[1].Expression).Value?.ToString();

                string? messageTypeNamespace = string.Empty;
                string? messageTypeName = string.Empty;
                if (attributeSyntax.ArgumentList.Arguments[2].Expression is TypeOfExpressionSyntax typeOfExpression)
                {
                    var typeSymbol = semanticModel.GetTypeInfo(typeOfExpression.Type).Type;
                    messageTypeName = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    messageTypeNamespace = typeSymbol?.ContainingNamespace.ToDisplayString();
                }

                return (functionName, timerCron, messageTypeNamespace, messageTypeName);
            }

            return default;
        }

        internal string? GetNamespaceName(SyntaxNode namespaceDeclaration)
        {
            return namespaceDeclaration switch
            {
                NamespaceDeclarationSyntax ns => ns.Name.ToString(),
                FileScopedNamespaceDeclarationSyntax fsns => fsns.Name.ToString(),
                _ => null
            };
        }
        private static string GenerateFunctionSource(string namespaceName, string messageTypeNamespace, string messageType, string functionName, string timer)
        {
            var source = $@"
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Ridvay.Azure.ServiceBus.Client;
using System.Threading.Tasks;
using {namespaceName};
using {messageTypeNamespace};

namespace Ridvay.Azure.Function.TriggerRedirects.TimerTrigger.Generated
{{
    public class {functionName}    
    {{
        private readonly IMessageSender _sender;
        private readonly ILogger<{functionName}> _logger;
        public {functionName}(IMessageSender sender, ILogger<{functionName}> logger)
        {{
              _sender = sender;
              _logger = logger; 
        }}

        [FunctionName(""{functionName}"")]
        public async Task Run([TimerTrigger(""{timer}"")] TimerInfo timer)
        {{
            _logger.LogInformation(""Timer trigger triggered"");
            await _sender.SendAsync(new {messageType}());
            _logger.LogInformation(""Event pushed to Service Bus"");
        }}
    }}
}}
";
            return source;
        }
    }
}
