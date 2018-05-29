using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;

namespace DependencyCop.Analyzers.Settings
{
    public static class SettingsHelper
    {
        public const string SettingsFileName = "dependencycop.json";
        public const string AltSettingsFileName = ".dependencycop.json";

        private static readonly SourceTextValueProvider<DependencyCopSettings> SettingsValueProvider =
            new SourceTextValueProvider<DependencyCopSettings>(
                text => GetDependencyCopSettings(SettingsFileName, text, DeserializationFailureBehavior.ReturnDefaultSettings));

        public static DependencyCopSettings GetDependencyCopSettings(this SymbolAnalysisContext context, CancellationToken cancellationToken)
        {
            return context.Options.GetDependencyCopSettings(cancellationToken);
        }

        public static DependencyCopSettings GetDependencyCopSettings(this AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetDependencyCopSettings(options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

        public static DependencyCopSettings GetDependencyCopSettings(this AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            return GetDependencyCopSettings(options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>(), failureBehavior, cancellationToken);
        }

        public static DependencyCopSettings GetDependencyCopSettings(this AnalysisContext context, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetDependencyCopSettings(context, options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

        public static DependencyCopSettings GetDependencyCopSettings(this AnalysisContext context, AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            string settingsFilePath;
            SourceText text = TryGetStyleCopSettingsText(options, cancellationToken, out settingsFilePath);
            if (text == null)
            {
                return DependencyCopSettings.Default;
            }

            if (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                DependencyCopSettings settings;
                if (!context.TryGetValue(text, SettingsValueProvider, out settings))
                {
                    return DependencyCopSettings.Default;
                }

                return settings;
            }

            // return GetStyleCopSettings(settingsFilePath, text, failureBehavior);
            return DependencyCopSettings.Default;
        }

        private static SourceText TryGetStyleCopSettingsText(this AnalyzerOptions options, CancellationToken cancellationToken, out string settingsFilePath)
        {
            foreach (var additionalFile in options.AdditionalFiles)
            {
                if (IsDependencyCopSettingsFile(additionalFile.Path))
                {
                    settingsFilePath = additionalFile.Path;

                    return additionalFile.GetText(cancellationToken);
                }
            }

            settingsFilePath = null;

            return null;
        }

        private static DependencyCopSettings GetDependencyCopSettings(string path, SourceText text, DeserializationFailureBehavior failureBehavior)
        {
            try
            {
                var jsonText = text.ToString();
                if(string.IsNullOrWhiteSpace(jsonText))
                {
                    throw new JsonException("The dependency cop settings file is empty.");
                }

                return JsonConvert.DeserializeObject<DependencyCopSettings>(jsonText);
            }
            catch (JsonException) when (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                // The settings file is invalid -> return the default settings.
            }

            return DependencyCopSettings.Default;
        }

        private static DependencyCopSettings GetDependencyCopSettings(ImmutableArray<AdditionalText> additionalFiles, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            foreach (var additionalFile in additionalFiles)
            {
                if (IsDependencyCopSettingsFile(additionalFile.Path))
                {
                    SourceText additionalTextContent = additionalFile.GetText(cancellationToken);
                    return GetDependencyCopSettings(additionalFile.Path, additionalTextContent, failureBehavior);
                }
            }

            return DependencyCopSettings.Default;
        }

        private static bool IsDependencyCopSettingsFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var fileName = Path.GetFileName(path);

            return string.Equals(fileName, SettingsFileName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileName, AltSettingsFileName, StringComparison.OrdinalIgnoreCase);
        }
    }
}