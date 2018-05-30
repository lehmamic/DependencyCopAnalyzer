using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using DependencyCop.Analyzers.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DependencyCop.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DependencyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DA0001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly string Title = "Invalid Dependency";
        private static readonly string MessageFormat = "The dependency between namespace '{0}' and '{1}' violates the dependency constraints.";
        private static readonly string Description = "This issue is reported when a code element (namespace, type, member) mapped to a Layer references a code element mapped to another layer, but there is no allowed dependency between these layers in the dependency rule configuration. This is a dependency constraint violation.";
        private const string Category = "Dependencies";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeFieldSymbol, SymbolKind.Field);
        }

        // private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        // {
        //     var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
        //     Console.WriteLine(localDeclaration);
        // }

        // private static void AnalyzeSymbol(SymbolAnalysisContext context)
        // {
        //     // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
        //     var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        //     // Find just those named type symbols with names containing lowercase letters.
        //     if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
        //     {
        //         // For all such symbols, produce a diagnostic.
        //         var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

        //         context.ReportDiagnostic(diagnostic);
        //     }
        // }

        private static void AnalyzeFieldSymbol(SymbolAnalysisContext context, DependencyCopSettings settings)
        {
            var fieldSymbol = (IFieldSymbol)context.Symbol;
            var validator = new DependencyValidator(settings);

            var fromNamespace = fieldSymbol.ContainingNamespace.GetNamespace();
            var toNamespace = fieldSymbol.Type.ContainingNamespace.GetNamespace();

            if(!validator.IsAllowedDependency(fromNamespace, toNamespace))
            {
                var diagnostic = Diagnostic.Create(Rule, fieldSymbol.Locations[0], fromNamespace, toNamespace);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}