namespace Smart.Analyzers.JapaneseComment;

public sealed class DescriptorTest
{
    [Fact]
    public void SupportedDiagnosticsAreUniqueAndComplete()
    {
        var analyzer = new JapaneseCommentAnalyzer();
        var descriptors = analyzer.SupportedDiagnostics;

        Assert.Equal(28, descriptors.Length);
        Assert.Equal(
            descriptors.Length,
            descriptors.Select(static d => d.Id).Distinct(StringComparer.Ordinal).Count());
    }
}
