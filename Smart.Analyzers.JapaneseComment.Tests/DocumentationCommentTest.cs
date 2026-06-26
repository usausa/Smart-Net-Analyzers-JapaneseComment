namespace Smart.Analyzers.JapaneseComment;

using Microsoft.CodeAnalysis;

public sealed class DocumentationCommentTest
{
    [Theory]
    [InlineData(DocumentationMode.None)]
    [InlineData(DocumentationMode.Parse)]
    public void TripleSlashContentIsChecked(DocumentationMode mode)
    {
        var diagnostics = AnalyzerTestRunner.Run("/// ｱ", mode);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Theory]
    [InlineData(DocumentationMode.None)]
    [InlineData(DocumentationMode.Parse)]
    public void XmlSummaryContentIsChecked(DocumentationMode mode)
    {
        const string source =
            """
            /// <summary>ｱ</summary>
            class C { }
            """;

        var diagnostics = AnalyzerTestRunner.Run(source, mode);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Theory]
    [InlineData(DocumentationMode.None)]
    [InlineData(DocumentationMode.Parse)]
    public void MultiLineDocumentationCommentContentIsChecked(DocumentationMode mode)
    {
        var diagnostics = AnalyzerTestRunner.Run("/** ｱ */", mode);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Fact]
    public void XmlTagsAndAttributeNamesAreNotFalsePositives()
    {
        const string source =
            """
            /// <summary>ascii</summary>
            /// <param name="value">ok</param>
            class C { }
            """;

        Assert.Empty(AnalyzerTestRunner.Run(source, DocumentationMode.Parse));
    }

    [Theory]
    [InlineData(DocumentationMode.None)]
    [InlineData(DocumentationMode.Parse)]
    public void OrdinaryCommentIsUnaffectedByDocumentationParsing(DocumentationMode mode)
    {
        var diagnostics = AnalyzerTestRunner.Run("// ｱ", mode);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }
}
