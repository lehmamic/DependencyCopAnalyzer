using System.Linq;

namespace DependencyCop.Analyzers
{
    public abstract class NamespaceSpecification
    {
        public const char NamespacePartSeparator = '.';

        protected NamespaceSpecification(string namespaceSpecificationAsString)
        {
            this.NamespaceSpecificationAsString = namespaceSpecificationAsString;
        }

        public string NamespaceSpecificationAsString { get; }

        public abstract bool Matches(Namespace ns);

        public override string ToString() => NamespaceSpecificationAsString;

        public bool Equals(NamespaceSpecification other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(NamespaceSpecificationAsString, other.NamespaceSpecificationAsString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            
            return Equals((NamespaceSpecification)obj);
        }

        public override int GetHashCode()
        {
            return (NamespaceSpecificationAsString != null ? NamespaceSpecificationAsString.GetHashCode() : 0);
        }

        public static bool operator ==(NamespaceSpecification left, NamespaceSpecification right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NamespaceSpecification left, NamespaceSpecification right)
        {
            return !Equals(left, right);
        }
    }
}