using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace DependencyCop.Analyzers.Tests
{
    public class NamespaceSymbolExtensionsFixture
    {
        [Fact]
        public void GetNamespace_WithSymbolIsGlobalNamespace_ReturnsEmptyString()
        {
            // arrange
            var globalNamespaceSymbolMock = new Mock<INamespaceSymbol>();
            globalNamespaceSymbolMock.SetupGet(s => s.IsGlobalNamespace).Returns(true);

            // act
            Namespace actual = globalNamespaceSymbolMock.Object.GetNamespace();

            // assert
            Assert.Equal(new Namespace(string.Empty), actual);
        }

        [Fact]
        public void GetNamespace_WithSinglenamespace_ReturnsNamespaceName()
        {
            // arrange
            var globalNamespaceSymbolMock = new Mock<INamespaceSymbol>();
            globalNamespaceSymbolMock.SetupGet(s => s.IsGlobalNamespace).Returns(true);

            var namespaceSymbolMock = new Mock<INamespaceSymbol>();
            namespaceSymbolMock.SetupGet(s => s.IsGlobalNamespace).Returns(false);
            namespaceSymbolMock.SetupGet(s => s.Name).Returns("Foo");
            namespaceSymbolMock.SetupGet(s => s.ContainingNamespace).Returns(globalNamespaceSymbolMock.Object);

            // act
            Namespace actual = namespaceSymbolMock.Object.GetNamespace();

            // assert
            Assert.Equal(new Namespace("Foo"), actual);
        }

        [Fact]
        public void GetNamespace_WithNestedNamespaces_ReturnsConcatedNamespaceString()
        {
            // arrange
            var globalNamespaceSymbolMock = new Mock<INamespaceSymbol>();
            globalNamespaceSymbolMock.SetupGet(s => s.IsGlobalNamespace).Returns(true);

            var parentNamespaceSymbolMock = new Mock<INamespaceSymbol>();
            parentNamespaceSymbolMock.SetupGet(s => s.IsGlobalNamespace).Returns(false);
            parentNamespaceSymbolMock.SetupGet(s => s.Name).Returns("Foo");
            parentNamespaceSymbolMock.SetupGet(s => s.ContainingNamespace).Returns(globalNamespaceSymbolMock.Object);

            var childNamespaceSymbolMock = new Mock<INamespaceSymbol>();
            childNamespaceSymbolMock.SetupGet(s => s.IsGlobalNamespace).Returns(false);
            childNamespaceSymbolMock.SetupGet(s => s.Name).Returns("Bar");
            childNamespaceSymbolMock.SetupGet(s => s.ContainingNamespace).Returns(parentNamespaceSymbolMock.Object);


            // act
            Namespace actual = childNamespaceSymbolMock.Object.GetNamespace();

            // assert
            Assert.Equal(new Namespace("Foo.Bar"), actual);
        }
    }
}