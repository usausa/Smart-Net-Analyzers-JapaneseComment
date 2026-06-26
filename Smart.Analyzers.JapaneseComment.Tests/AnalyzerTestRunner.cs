namespace Smart.Analyzers.JapaneseComment;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

internal static class AnalyzerTestRunner
{
    private static readonly MetadataReference CorlibReference =
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

    // Runs JapaneseCommentAnalyzer over a single source snippet and returns only its diagnostics.
    //
    // - documentationMode controls how /// comments are lexed (None = ordinary comment trivia,
    //   Parse = structured documentation trivia). This is required to characterize the XML
    //   documentation comment behavior.
    // - enabledRules turns on diagnostics that are disabled by default (e.g. SAJFF06), equivalent
    //   to "dotnet_diagnostic.<id>.severity = warning" in an .editorconfig.
    public static Diagnostic[] Run(
        string source,
        DocumentationMode documentationMode = DocumentationMode.None,
        params string[] enabledRules)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview, documentationMode);
        var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);

        var specificOptions = ImmutableDictionary<string, ReportDiagnostic>.Empty;
        foreach (var id in enabledRules)
        {
            specificOptions = specificOptions.SetItem(id, ReportDiagnostic.Warn);
        }

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            specificDiagnosticOptions: specificOptions);

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [CorlibReference],
            compilationOptions);

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new JapaneseCommentAnalyzer());
        // GetAllDiagnosticsAsync applies the full build filtering pipeline (effective severity,
        // isEnabledByDefault, specific options), so disabled-by-default rules behave as in a real build.
        var diagnostics = compilation
            .WithAnalyzers(analyzers)
            .GetAllDiagnosticsAsync()
            .GetAwaiter()
            .GetResult();

        return [.. diagnostics
            .Where(static d => d.Id.StartsWith("SAJ", StringComparison.Ordinal))
            .OrderBy(static d => d.Location.SourceSpan.Start)];
    }

    // Convenience helper that projects diagnostics to their rule ids (in source order).
    public static string[] RunIds(
        string source,
        DocumentationMode documentationMode = DocumentationMode.None,
        params string[] enabledRules) =>
        [.. Run(source, documentationMode, enabledRules).Select(static d => d.Id)];
}
