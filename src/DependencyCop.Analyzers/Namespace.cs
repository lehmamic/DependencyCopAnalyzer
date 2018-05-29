namespace DependencyCop.Analyzers
{
    public class Namespace : NamespaceSpecification
    {
        public Namespace(string namespaceAsAString)
            : base(namespaceAsAString)
        {
        }

        public bool IsSubnamespaceOf(Namespace parentCandidate)
        {
            var parentPrefix = parentCandidate.NamespaceSpecificationAsString + NamespacePartSeparator;
            return this.NamespaceSpecificationAsString.StartsWith(parentPrefix);
        }

        public override bool Matches(Namespace ns)
        {
            return this == ns;
        }
    }
}