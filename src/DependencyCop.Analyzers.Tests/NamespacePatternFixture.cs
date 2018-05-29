using Xunit;

namespace DependencyCop.Analyzers.Tests
{
    public class NamespacePatternFixture
    {

        [Theory]
        [InlineData("*", "Foo.Bar")]
        [InlineData("Foo.Bar", "Foo.Bar")]
        [InlineData("Foo.*", "Foo.Bar")]
        [InlineData("*.Bar", "Foo.Bar")]
        [InlineData("Foo.*.Bar", "Foo.Pilot.Bar")]
        public void Matches_WithMatchingPatternAndNamespace_ReturnsTrue(string fromString, string toString)
        {
            NamespaceSpecification from = new NamespacePattern(fromString);
            Namespace to = new Namespace(toString);

            bool actual = from.Matches(to);

            Assert.True(actual);
        }

        [Theory]
        [InlineData("Foo.Bar", "foo.bar")]
        [InlineData("*.Pilot", "Foo.Bar")]
        [InlineData("Pilot.*", "Foo.Bar")]
        [InlineData("Foo.Bar", "Foo.Bar.Pilot")]
        [InlineData("Foo.Bar.Pilot", "Foo.Bar")]
        public void Matches_WithNotMatchingPatternAndNamespace_ReturnsFalse(string fromString, string toString)
        {
            NamespaceSpecification from = new NamespacePattern(fromString);
            Namespace to = new Namespace(toString);

            bool actual = from.Matches(to);

            Assert.False(actual);
        }
    }
}