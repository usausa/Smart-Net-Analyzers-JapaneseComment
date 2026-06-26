namespace Smart.Analyzers.JapaneseComment;

using System.Globalization;

using Microsoft.CodeAnalysis;

public sealed class DiagnosticMessageTest
{
    [Theory]
    [InlineData("’", RuleIdentifiers.SingleQuotationInCommentShouldBeNarrow)]
    [InlineData("”", RuleIdentifiers.DoubleQuotationInCommentShouldBeNarrow)]
    [InlineData("！", RuleIdentifiers.ExclamationInCommentShouldBeNarrow)]
    [InlineData("＃", RuleIdentifiers.SharpInCommentShouldBeNarrow)]
    [InlineData("＄", RuleIdentifiers.DollarInCommentShouldBeNarrow)]
    [InlineData("％", RuleIdentifiers.PercentInCommentShouldBeNarrow)]
    [InlineData("＆", RuleIdentifiers.AmpersandInCommentShouldBeNarrow)]
    [InlineData("（", RuleIdentifiers.ParenthesisInCommentShouldBeNarrow)]
    [InlineData("＊", RuleIdentifiers.AsteriskInCommentShouldBeNarrow)]
    [InlineData("＋", RuleIdentifiers.PlusInCommentShouldBeNarrow)]
    [InlineData("，", RuleIdentifiers.CommaInCommentShouldBeNarrow)]
    [InlineData("－", RuleIdentifiers.HyphenInCommentShouldBeNarrow)]
    [InlineData("．", RuleIdentifiers.DotInCommentShouldBeNarrow)]
    [InlineData("／", RuleIdentifiers.SlashInCommentShouldBeNarrow)]
    [InlineData("：", RuleIdentifiers.ColonInCommentShouldBeNarrow)]
    [InlineData("；", RuleIdentifiers.SemicolonInCommentShouldBeNarrow)]
    [InlineData("＜", RuleIdentifiers.LessThanInCommentShouldBeNarrow)]
    [InlineData("＝", RuleIdentifiers.EqualsInCommentShouldBeNarrow)]
    [InlineData("＞", RuleIdentifiers.GreaterThanInCommentShouldBeNarrow)]
    [InlineData("？", RuleIdentifiers.QuestionInCommentShouldBeNarrow)]
    [InlineData("＠", RuleIdentifiers.AtMarkInCommentShouldBeNarrow)]
    [InlineData("［", RuleIdentifiers.SquareBracketInCommentShouldBeNarrow)]
    [InlineData("｛", RuleIdentifiers.CurlyBracketInCommentShouldBeNarrow)]
    [InlineData("￥", RuleIdentifiers.YenInCommentShouldBeNarrow)]
    public void MessageMentionsTargetCharacter(string targetChar, string ruleId)
    {
        var diagnostics = AnalyzerTestRunner.Run("// " + targetChar, DocumentationMode.None, ruleId);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(ruleId, diagnostic.Id);
        Assert.Contains(targetChar, diagnostic.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }
}
