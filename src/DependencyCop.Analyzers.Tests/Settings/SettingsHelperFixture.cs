using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using DependencyCop.Analyzers.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Xunit;

namespace DependencyCop.Analyzers.Tests.Settings
{
    public class SettingsHelperFixture
    {
        private const string TestProjectName = "TestProject";

        [Fact]
        public void GetDependencyCopSettings_WithoutSpecificSettings_ReturnsSettingsDefaults()
        {
            DependencyCopSettings settings = SettingsHelper.GetDependencyCopSettings(default(SymbolAnalysisContext), CancellationToken.None);

            AssertDefaultSettings(settings);
        }

        [Fact]
        public async Task GetStyleCopSettings_WithSettingsJson_SettingsAreReadCorrectly()
        {
            // arrange
            var settingsJson = @"
{
    ""rules"": [
        { ""type"": ""allow"", ""from"": ""*"", ""to"": ""*"" },
        { ""type"": ""deny"", ""from"": ""fo.bar.*"", ""to"": ""camp.hack.*"" }
    ]
}
";
            var context = await CreateAnalysisContextAsync(settingsJson).ConfigureAwait(false);

            // act
            var settings = context.GetDependencyCopSettings(CancellationToken.None);

            // assert
            var expected = new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRuleEntry { Type = DependencyRuleType.Allow, From = "*", To="*" },
                    new DependencyRuleEntry { Type = DependencyRuleType.Deny, From = "fo.bar.*", To="camp.hack.*" },
                }
            };

            AssertSettings(settings, expected);
        }

        [Fact]
        public async Task GetStyleCopSettings_WithInvalidJson_ReturnsDefaultSettings()
        {
            // arrange
            var settingsJson = @"This is not a JSON file.";
            var context = await CreateAnalysisContextAsync(settingsJson).ConfigureAwait(false);

            // act
            var settings = context.GetDependencyCopSettings(CancellationToken.None);

            // assert
            AssertDefaultSettings(settings);
        }

        [Fact]
        public async Task GetStyleCopSettings_WithEmptyOrMissingFile_ReturnsDefaultSettings()
        {
            // arrange
            var settingsJson = string.Empty;
            var context = await CreateAnalysisContextAsync(settingsJson).ConfigureAwait(false);

            // act
            var settings = context.GetDependencyCopSettings(CancellationToken.None);

            // assert
            AssertDefaultSettings(settings);
        }

        private static async Task<SymbolAnalysisContext> CreateAnalysisContextAsync(string dependencyCopJson, string settingsFileName = SettingsHelper.SettingsFileName)
        {
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .AddDocument(documentId, "Test0.cs", SourceText.From(string.Empty));

            var document = solution.GetDocument(documentId);
            var project = solution.GetProject(projectId);

            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);

            var stylecopJSONFile = new AdditionalTextHelper(settingsFileName, dependencyCopJson);
            var additionalFiles = ImmutableArray.Create<AdditionalText>(stylecopJSONFile);
            var analyzerOptions = new AnalyzerOptions(additionalFiles);

            return new SymbolAnalysisContext(
                Mock.Of<ISymbol>(),
                compilation,
                analyzerOptions,
                d => { },
                d => true,
                CancellationToken.None);
        }

        private static void AssertDefaultSettings(DependencyCopSettings settings)
        {
            var expected = new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRuleEntry { Type = DependencyRuleType.Allow, From = "*", To="*" }
                }
            };

            AssertSettings(settings, expected);
        }

        private static void AssertSettings(DependencyCopSettings actual, DependencyCopSettings expected)
        {
            Assert.NotNull(actual);

            Assert.NotNull(actual.Rules);
            Assert.Equal(actual.Rules.Length, expected.Rules.Length);

            for(int i = 0; i< actual.Rules.Length; i++)
            {
                Assert.Equal(actual.Rules[0].Type, expected.Rules[0].Type);
                Assert.Equal(actual.Rules[0].From, expected.Rules[0].From);
                Assert.Equal(actual.Rules[0].To, expected.Rules[0].To);
            }
        }

        private class AdditionalTextHelper : AdditionalText
        {
            private readonly SourceText sourceText;

            public AdditionalTextHelper(string path, string text)
            {
                this.Path = path;
                this.sourceText = SourceText.From(text);
            }

            public override string Path { get; }

            public override SourceText GetText(CancellationToken cancellationToken = default(CancellationToken))
            {
                return this.sourceText;
            }
        }
    }
}