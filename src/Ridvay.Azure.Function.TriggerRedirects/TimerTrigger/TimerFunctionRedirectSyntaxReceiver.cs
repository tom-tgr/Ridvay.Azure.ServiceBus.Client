using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ridvay.Azure.Function.TriggerRedirects.TimerTrigger
{
    public class TimerFunctionRedirectSyntaxReceiver : ISyntaxReceiver
    {
        private const string TimerTriggerAttributeName = "FunctionTimerTriggerAttribute";
        private const string TimerTriggerAttributeNameShort = "FunctionTimerTrigger";
        
        public List<(ClassDeclarationSyntax,AttributeSyntax)> CandidateClasses { get; } = new List<(ClassDeclarationSyntax, AttributeSyntax)>();
        
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
                return;
            
            
            var attributes = classDeclarationSyntax.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Where(attr => attr.Name.ToString() == TimerTriggerAttributeName 
                               || attr.Name.ToString() == TimerTriggerAttributeNameShort);

            foreach (var attributeSyntax in attributes)
            {
                if (attributeSyntax != null)
                {
                    CandidateClasses.Add((classDeclarationSyntax, attributeSyntax));
                }
            }
        }
    }
}


