using System.Text;
using Microsoft.CodeAnalysis;

namespace DependencyCop.Analyzers
{
    public static class NamespaceSymbolExtensions
    {
        public static Namespace GetNamespace(this INamespaceSymbol symbol)
        {
            return new Namespace(symbol.GetNamespaceAsString());
        }

        private static string GetNamespaceAsString(this INamespaceSymbol symbol)
        {
            if(symbol.IsGlobalNamespace || symbol.ContainingNamespace == null)
            {
                return string.Empty;
            }

            string parentNamespace = symbol.ContainingNamespace.GetNamespaceAsString();

            return string.IsNullOrWhiteSpace(parentNamespace) ? symbol.Name : $"{parentNamespace}.{symbol.Name}";
        }
    }
}