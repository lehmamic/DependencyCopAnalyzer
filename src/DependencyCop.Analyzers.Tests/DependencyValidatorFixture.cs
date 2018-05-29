using DependencyCop.Analyzers.Settings;
using Xunit;

namespace DependencyCop.Analyzers.Tests
{
    public class DependencyValidatorFixture
    {
        [Fact]
        public void IsAllowedDependency_WithMatchingAllowedRule_ReturnsTrue()
        {
            // arrange
            var settings = new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRuleEntry { Type = DependencyRuleType.Allow, From = "*", To = "Foo.Pilot" }
                }
            };

            var validator = new DependencyValidator(settings);

            // act
            bool actual = validator.IsAllowedDependency(new Namespace("Foo.Bar"), new Namespace("Foo.Pilot"));

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void IsAllowedDependency_WithNoMatchingAllowedRule_ReturnsFalse()
        {
            // arrange
            var settings = new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRuleEntry { Type = DependencyRuleType.Allow, From = "Foo.Fuzzy", To = "Foo.Pilot" }
                }
            };

            var validator = new DependencyValidator(settings);

            // act
            bool actual = validator.IsAllowedDependency(new Namespace("Foo.Bar"), new Namespace("Foo.Pilot"));

            // assert
            Assert.False(actual);
        }

        [Fact]
        public void IsAllowedDependency_WithMatchingDeniedRule_ReturnsFalse()
        {
            // arrange
            var settings = new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRuleEntry { Type = DependencyRuleType.Allow, From = "*", To = "Foo.Pilot" },
                    new DependencyRuleEntry { Type = DependencyRuleType.Deny, From = "Foo.Bar", To = "Foo.Pilot" },
                }
            };

            var validator = new DependencyValidator(settings);

            // act
            bool actual = validator.IsAllowedDependency(new Namespace("Foo.Bar"), new Namespace("Foo.Pilot"));

            // assert
            Assert.False(actual);
        }
    }
}