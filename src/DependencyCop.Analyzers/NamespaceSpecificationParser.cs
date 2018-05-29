namespace DependencyCop.Analyzers
{
    public static class NamespaceSpecificationParser
    {
        public static NamespaceSpecification Parse(string namespaceSpecificationAsString)
        {
            return namespaceSpecificationAsString.Contains(NamespacePattern.WildcardString) ?
                (NamespaceSpecification)new NamespacePattern(namespaceSpecificationAsString)
                : new Namespace(namespaceSpecificationAsString);
        }
    }
}