using System.Text.RegularExpressions;

namespace DependencyCop.Analyzers
{
    public class NamespacePattern : NamespaceSpecification
    {
        public const string WildcardString = "*";

        public NamespacePattern(string namespaceAsAString)
            : base(namespaceAsAString)
        {
        }

        public override bool Matches(Namespace ns)
        {
            var pattern = $"^{Regex.Escape(this.NamespaceSpecificationAsString)}$"
                .Replace($"\\{WildcardString}", ".+");

            return Regex.IsMatch(ns.NamespaceSpecificationAsString, pattern);
        }
    }
}