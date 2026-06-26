namespace Smart.Analyzers.JapaneseComment;

using Microsoft.CodeAnalysis;

public sealed class DisabledRuleTest
{
    [Theory]
    [InlineData("// ！", RuleIdentifiers.ExclamationInCommentShouldBeNarrow)]
    [InlineData("// ＆", RuleIdentifiers.AmpersandInCommentShouldBeNarrow)]
    [InlineData("// ，", RuleIdentifiers.CommaInCommentShouldBeNarrow)]
    [InlineData("// ．", RuleIdentifiers.DotInCommentShouldBeNarrow)]
    [InlineData("// ？", RuleIdentifiers.QuestionInCommentShouldBeNarrow)]
    public void DisabledByDefaultRuleIsSilentUnlessEnabled(string source, string ruleId)
    {
        // Off by default.
        Assert.Empty(AnalyzerTestRunner.Run(source));

        // Reported once explicitly enabled (e.g. via .editorconfig severity).
        var diagnostics = AnalyzerTestRunner.Run(source, DocumentationMode.None, ruleId);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(ruleId, diagnostic.Id);
    }
}
