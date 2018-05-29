namespace DependencyCop.Analyzers
{
    public class DependencyRule
    {
        public DependencyRule(string from, string to)
            : this(NamespaceSpecificationParser.Parse(from), NamespaceSpecificationParser.Parse(to))
        {
        }

        public DependencyRule(NamespaceSpecification from, NamespaceSpecification to)
        {
            this.From = from;
            this.To = to;
        }

        public NamespaceSpecification From { get; }

        public NamespaceSpecification To { get; }
    }
}