using System;
using DependencyCop.Analyzers.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace DependencyCop.Analyzers.Tests
{
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [Fact]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
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
    //     [Fact]
    //     public void TestMethod2()
    //     {
    //         var test = @"
    // using System;
    // using System.Collections.Generic;
    // using System.Linq;
    // using System.Text;
    // using System.Threading.Tasks;
    // using System.Diagnostics;
    // namespace ConsoleApplication1
    // {
    //     class TypeName
    //     {   
    //     }
    // }";
    //         var expected = new DiagnosticResult
    //         {
    //             Id = "$saferootidentifiername$",
    //             Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
    //             Severity = DiagnosticSeverity.Warning,
    //             Locations =
    //                 new[] {
    //                         new DiagnosticResultLocation("Test0.cs", 11, 15)
    //                     }
    //         };

    //         VerifyCSharpDiagnostic(test, expected);

    //         var fixtest = @"
    // using System;
    // using System.Collections.Generic;
    // using System.Linq;
    // using System.Text;
    // using System.Threading.Tasks;
    // using System.Diagnostics;
    // namespace ConsoleApplication1
    // {
    //     class TYPENAME
    //     {   
    //     }
    // }";
    //         VerifyCSharpFix(test, fixtest);
        // }

        // protected override CodeFixProvider GetCSharpCodeFixProvider()
        // {
        //     return new $saferootidentifiername$CodeFixProvider();
        // }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DependencyAnalyzer();
        }
    }
}
