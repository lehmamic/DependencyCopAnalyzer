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
        { ""type"": ""allow"", ""from"": ""Test.Foo.Pilot"", ""to"": ""Test.Foo.Bar"" }
    ]
}";

            var expected = new DiagnosticResult
            {
                Id = DependencyAnalyzer.DiagnosticId,
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 15)
                        }
            };

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
    ]
}";
            var expected = new DiagnosticResult
            {
                Id = DependencyAnalyzer.DiagnosticId,
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 15)
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
