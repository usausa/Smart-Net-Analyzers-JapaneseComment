namespace Smart.Analyzers.JapaneseComment;

using Microsoft.CodeAnalysis;

public sealed class DocumentationCommentTest
{
    [Fact]
    public void TripleSlashContentIsCheckedWithoutDocumentationParsing()
    {
        // DocumentationMode.None: /// is lexed as an ordinary single-line comment, so its content
        // is analyzed. This mirrors a typical project without GenerateDocumentationFile.
        var diagnostics = AnalyzerTestRunner.Run("/// ｱ", DocumentationMode.None);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Fact]
    public void XmlSummaryContentIsCheckedWithoutDocumentationParsing()
    {
        const string source =
            """
            /// <summary>ｱ</summary>
            class C { }
            """;

        var diagnostics = AnalyzerTestRunner.Run(source, DocumentationMode.None);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Fact]
    public void TripleSlashContentIsCurrentlyMissedWhenDocumentationParsingEnabled()
    {
        // Characterization of CURRENT behavior (see 実装プランGPT-5.5.md section 2 / task P2-1).
        //
        // With DocumentationMode.Parse the /// becomes structured documentation trivia and the text
        // "ｱ" is an XML token rather than comment trivia, so the analyzer does NOT see it. This is the
        // documented gap: when P2-1 is implemented, this expectation must flip to a single SAJ0001.
        var diagnostics = AnalyzerTestRunner.Run("/// ｱ", DocumentationMode.Parse);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public void OrdinaryCommentIsUnaffectedByDocumentationParsing()
    {
        // Regular // comments behave identically regardless of documentation mode.
        var diagnostics = AnalyzerTestRunner.Run("// ｱ", DocumentationMode.Parse);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }
}
