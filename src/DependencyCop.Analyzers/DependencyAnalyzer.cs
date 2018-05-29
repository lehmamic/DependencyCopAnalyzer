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
        private static readonly string Title = "Type name contains lowercase letters";
        private static readonly string MessageFormat = "Type name '{0}' contains lowercase letters";
        private static readonly string Description = "Type names should be all uppercase";
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeDependencySymbol, SymbolKind.Field);
            //context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.);
        }

        // private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        // {
        //     var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
        //     Console.WriteLine(localDeclaration);
        // }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeDependencySymbol(SymbolAnalysisContext context, DependencyCopSettings settings)
        {
            Console.WriteLine(context.Symbol.Name);
        }
    }
}