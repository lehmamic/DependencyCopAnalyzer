using System;
using DependencyCop.Analyzers.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DependencyCop.Analyzers
{
    public static class AnalyzerExtensions
    {
        public static void RegisterSymbolAction(this AnalysisContext context, Action<SymbolAnalysisContext, DependencyCopSettings> action, params SymbolKind[] symbolKinds)
        {
            context.RegisterSymbolAction(
                c =>
                {
                    DependencyCopSettings settings = context.GetDependencyCopSettings(c.Options, c.CancellationToken);
                    action(c, settings);
                },
                symbolKinds);
        }
    }
}