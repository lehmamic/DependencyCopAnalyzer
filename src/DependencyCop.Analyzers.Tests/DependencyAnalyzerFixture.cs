using System;
using DependencyCop.Analyzers.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace DependencyCop.Analyzers.Tests
{
    public class DependencyAnalyzerFixture : CodeFixVerifier
    {
        private string currentSettings;

        [Fact]
        public void VerifyCSharpDiagnostic_WithEmptyCode_ReturnsNoDiagnostics()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void VerifyCSharpDiagnostic_WithAllowedFieldTypeDependency_ReturnsNoDiagnostics()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Test.Foo.Bar;

namespace Test.Foo.Bar
{
    class TypeName
    {
    }
}

namespace Test.Foo.Pilot
{
    class Test
    {
        private TypeName x = new TypeName();
    }
}";

            this.currentSettings = @"
{
    ""rules"": [
        { ""type"": ""allow"", ""from"": ""*"", ""to"": ""*"" }
    ]
}";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void VerifyCSharpDiagnostic_WithNotAllowedFieldTypeDependency_ReturnsDiagnostics()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Test.Foo.Bar;

namespace Test.Foo.Bar
{
    class TypeName
    {
    }
}

namespace Test.Foo.Pilot
{
    class Test
    {
        private TypeName x = new TypeName();
    }
}";

            this.currentSettings = @"
{
    ""rules"": [
        { ""type"": ""allow"", ""from"": ""x"", ""to"": ""y"" }
    ]
}";

            var expected = new DiagnosticResult
            {
                Id = DependencyAnalyzer.DiagnosticId,
                Message = string.Format("The dependency between namespace '{0}' and '{1}' violates the dependency constraints.", "Test.Foo.Pilot", "Test.Foo.Bar"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 21, 26)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DependencyAnalyzer();
        }

        protected override string GetSettings()
        {
            return this.currentSettings ?? base.GetSettings();
        }
    }
}
