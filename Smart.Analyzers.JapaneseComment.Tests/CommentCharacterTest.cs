namespace Smart.Analyzers.JapaneseComment;

public sealed class CommentCharacterTest
{
    [Theory]
    [InlineData("// ｱ", RuleIdentifiers.KanaCharacterInCommentShouldBeWide)]
    [InlineData("// Ａ", RuleIdentifiers.AlphabetInCommentShouldBeNarrow)]
    [InlineData("// １", RuleIdentifiers.NumericCharacterInCommentShouldBeNarrow)]
    [InlineData("//　", RuleIdentifiers.SpaceInCommentShouldBeNarrow)]
    [InlineData("// ’", RuleIdentifiers.SingleQuotationInCommentShouldBeNarrow)]
    [InlineData("// ”", RuleIdentifiers.DoubleQuotationInCommentShouldBeNarrow)]
    [InlineData("// ＃", RuleIdentifiers.SharpInCommentShouldBeNarrow)]
    [InlineData("// ＄", RuleIdentifiers.DollarInCommentShouldBeNarrow)]
    [InlineData("// ％", RuleIdentifiers.PercentInCommentShouldBeNarrow)]
    [InlineData("// （", RuleIdentifiers.ParenthesisInCommentShouldBeNarrow)]
    [InlineData("// ＊", RuleIdentifiers.AsteriskInCommentShouldBeNarrow)]
    [InlineData("// ＋", RuleIdentifiers.PlusInCommentShouldBeNarrow)]
    [InlineData("// －", RuleIdentifiers.HyphenInCommentShouldBeNarrow)]
    [InlineData("// ／", RuleIdentifiers.SlashInCommentShouldBeNarrow)]
    [InlineData("// ：", RuleIdentifiers.ColonInCommentShouldBeNarrow)]
    [InlineData("// ；", RuleIdentifiers.SemicolonInCommentShouldBeNarrow)]
    [InlineData("// ＜", RuleIdentifiers.LessThanInCommentShouldBeNarrow)]
    [InlineData("// ＝", RuleIdentifiers.EqualsInCommentShouldBeNarrow)]
    [InlineData("// ＞", RuleIdentifiers.GreaterThanInCommentShouldBeNarrow)]
    [InlineData("// ＠", RuleIdentifiers.AtMarkInCommentShouldBeNarrow)]
    [InlineData("// ［", RuleIdentifiers.SquareBracketInCommentShouldBeNarrow)]
    [InlineData("// ｛", RuleIdentifiers.CurlyBracketInCommentShouldBeNarrow)]
    [InlineData("// ￥", RuleIdentifiers.YenInCommentShouldBeNarrow)]
    public void EnabledByDefaultRuleReportsSingleDiagnostic(string source, string expectedId)
    {
        var diagnostics = AnalyzerTestRunner.Run(source);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(expectedId, diagnostic.Id);
    }

    [Theory]
    [InlineData("// abc 123 () : ; <> = @ []")]
    [InlineData("// ascii only")]
    [InlineData("//")]
    [InlineData("// 日本語のコメント")]
    public void NarrowOrUntargetedContentReportsNothing(string source) =>
        Assert.Empty(AnalyzerTestRunner.Run(source));

    [Fact]
    public void MultipleIssuesInOneCommentAreAllReported()
    {
        // Half-width kana + full-width alphabet + full-width digit in a single comment.
        var ids = AnalyzerTestRunner.RunIds("// ｱ Ａ １");

        Assert.Equal(3, ids.Length);
        Assert.Contains(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, ids);
        Assert.Contains(RuleIdentifiers.AlphabetInCommentShouldBeNarrow, ids);
        Assert.Contains(RuleIdentifiers.NumericCharacterInCommentShouldBeNarrow, ids);
    }

    [Fact]
    public void SameIssueTwiceIsReportedOnce()
    {
        // Two half-width kana collapse into a single diagnostic (flags are deduplicated).
        var diagnostics = AnalyzerTestRunner.Run("// ｱｲ");

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Fact]
    public void MultiLineCommentContentIsChecked()
    {
        var diagnostics = AnalyzerTestRunner.Run("/* ｱ */");

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Theory]
    [InlineData("/**/")]
    [InlineData("/* ascii */")]
    public void MultiLineCommentWithoutTargetReportsNothing(string source)
    {
        Assert.Empty(AnalyzerTestRunner.Run(source));
    }

    [Fact]
    public void UnterminatedMultiLineCommentContentIsChecked()
    {
        var diagnostics = AnalyzerTestRunner.Run("/* ｱ");

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(RuleIdentifiers.KanaCharacterInCommentShouldBeWide, diagnostic.Id);
    }

    [Theory]
    [InlineData("/*")]
    [InlineData("/* ascii")]
    public void UnterminatedMultiLineCommentWithoutTargetReportsNothing(string source)
    {
        Assert.Empty(AnalyzerTestRunner.Run(source));
    }
}
