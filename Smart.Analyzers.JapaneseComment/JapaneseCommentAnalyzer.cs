namespace Smart.Analyzers.JapaneseComment;

using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class JapaneseCommentAnalyzer : DiagnosticAnalyzer
{
    [Flags]
    private enum CommentCharFlags : uint
    {
        None = 0,
        NarrowKana = 1u << 0,
        WideAlphabet = 1u << 1,
        WideNumeric = 1u << 2,
        WideSpace = 1u << 3,
        WideSingleQuotation = 1u << 4,
        WideDoubleQuotation = 1u << 5,
        WideExclamation = 1u << 6,
        WideSharp = 1u << 7,
        WideDollar = 1u << 8,
        WidePercent = 1u << 9,
        WideAmpersand = 1u << 10,
        WideParenthesis = 1u << 11,
        WideAsterisk = 1u << 12,
        WidePlus = 1u << 13,
        WideComma = 1u << 14,
        WideHyphen = 1u << 15,
        WideDot = 1u << 16,
        WideSlash = 1u << 17,
        WideColon = 1u << 18,
        WideSemicolon = 1u << 19,
        WideLessThan = 1u << 20,
        WideEquals = 1u << 21,
        WideGreaterThan = 1u << 22,
        WideQuestion = 1u << 23,
        WideAtMark = 1u << 24,
        WideSquareBracket = 1u << 25,
        WideCurlyBracket = 1u << 26,
        WideYen = 1u << 27
    }

    private static DiagnosticDescriptor RuleNarrowKana { get; } = new(
        id: RuleIdentifiers.KanaCharacterInCommentShouldBeWide,
        title: "Kana should be wide",
        messageFormat: "Half-width kana (U+FF61-FF9F) is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideAlphabet { get; } = new(
        id: RuleIdentifiers.AlphabetInCommentShouldBeNarrow,
        title: "Alphabet should be narrow",
        messageFormat: "Full-width alphabet (Ａ-Ｚ, ａ-ｚ) is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideNumeric { get; } = new(
        id: RuleIdentifiers.NumericCharacterInCommentShouldBeNarrow,
        title: "Numeric should be narrow",
        messageFormat: "Full-width numeric (０-９) is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideSpace { get; } = new(
        id: RuleIdentifiers.SpaceInCommentShouldBeNarrow,
        title: "Space should be narrow",
        messageFormat: "Full-width space (U+3000) is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideSingleQuotation { get; } = new(
        id: RuleIdentifiers.SingleQuotationInCommentShouldBeNarrow,
        title: "'’' should be narrow",
        messageFormat: "Full-width '’' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideDoubleQuotation { get; } = new(
        id: RuleIdentifiers.DoubleQuotationInCommentShouldBeNarrow,
        title: "'”' should be narrow",
        messageFormat: "Full-width '”' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideExclamation { get; } = new(
        id: RuleIdentifiers.ExclamationInCommentShouldBeNarrow,
        title: "'！' should be narrow",
        messageFormat: "Full-width '！' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false);  // Default off

    private static DiagnosticDescriptor RuleWideSharp { get; } = new(
        id: RuleIdentifiers.SharpInCommentShouldBeNarrow,
        title: "'＃' should be narrow",
        messageFormat: "Full-width '＃' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideDollar { get; } = new(
        id: RuleIdentifiers.DollarInCommentShouldBeNarrow,
        title: "'＄' should be narrow",
        messageFormat: "Full-width '＄' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWidePercent { get; } = new(
        id: RuleIdentifiers.PercentInCommentShouldBeNarrow,
        title: "'％' should be narrow",
        messageFormat: "Full-width '％' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideAmpersand { get; } = new(
        id: RuleIdentifiers.AmpersandInCommentShouldBeNarrow,
        title: "'＆' should be narrow",
        messageFormat: "Full-width '＆' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false);  // Default off

    private static DiagnosticDescriptor RuleWideParenthesis { get; } = new(
        id: RuleIdentifiers.ParenthesisInCommentShouldBeNarrow,
        title: "'（）' should be narrow",
        messageFormat: "Full-width '（）' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideAsterisk { get; } = new(
        id: RuleIdentifiers.AsteriskInCommentShouldBeNarrow,
        title: "'＊' should be narrow",
        messageFormat: "Full-width '＊' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWidePlus { get; } = new(
        id: RuleIdentifiers.PlusInCommentShouldBeNarrow,
        title: "'＋' should be narrow",
        messageFormat: "Full-width '＋' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideComma { get; } = new(
        id: RuleIdentifiers.CommaInCommentShouldBeNarrow,
        title: "'，' should be narrow",
        messageFormat: "Full-width '，' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false);  // Default off

    private static DiagnosticDescriptor RuleWideHyphen { get; } = new(
        id: RuleIdentifiers.HyphenInCommentShouldBeNarrow,
        title: "'－' should be narrow",
        messageFormat: "Full-width '－' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideDot { get; } = new(
        id: RuleIdentifiers.DotInCommentShouldBeNarrow,
        title: "'．' should be narrow",
        messageFormat: "Full-width '．' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false);  // Default off

    private static DiagnosticDescriptor RuleWideSlash { get; } = new(
        id: RuleIdentifiers.SlashInCommentShouldBeNarrow,
        title: "'／' should be narrow",
        messageFormat: "Full-width '／' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideColon { get; } = new(
        id: RuleIdentifiers.ColonInCommentShouldBeNarrow,
        title: "'：' should be narrow",
        messageFormat: "Full-width '：' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideSemicolon { get; } = new(
        id: RuleIdentifiers.SemicolonInCommentShouldBeNarrow,
        title: "'；' should be narrow",
        messageFormat: "Full-width '；' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideLessThan { get; } = new(
        id: RuleIdentifiers.LessThanInCommentShouldBeNarrow,
        title: "'＜' should be narrow",
        messageFormat: "Full-width '＜' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideEquals { get; } = new(
        id: RuleIdentifiers.EqualsInCommentShouldBeNarrow,
        title: "'＝' should be narrow",
        messageFormat: "Full-width '＝' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideGreaterThan { get; } = new(
        id: RuleIdentifiers.GreaterThanInCommentShouldBeNarrow,
        title: "'＞' should be narrow",
        messageFormat: "Full-width '＞' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideQuestion { get; } = new(
        id: RuleIdentifiers.QuestionInCommentShouldBeNarrow,
        title: "'？' should be narrow",
        messageFormat: "Full-width '？' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false);  // Default off

    private static DiagnosticDescriptor RuleWideAtMark { get; } = new(
        id: RuleIdentifiers.AtMarkInCommentShouldBeNarrow,
        title: "'＠' should be narrow",
        messageFormat: "Full-width '＠' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideSquareBracket { get; } = new(
        id: RuleIdentifiers.SquareBracketInCommentShouldBeNarrow,
        title: "'［］' should be narrow",
        messageFormat: "Full-width '［］' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideCurlyBracket { get; } = new(
        id: RuleIdentifiers.CurlyBracketInCommentShouldBeNarrow,
        title: "'｛｝' should be narrow",
        messageFormat: "Full-width '｛｝' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor RuleWideYen { get; } = new(
        id: RuleIdentifiers.YenInCommentShouldBeNarrow,
        title: "'￥' should be narrow",
        messageFormat: "Full-width '￥' is used in a comment",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly ImmutableArray<DiagnosticDescriptor> Rules =
    [
        RuleNarrowKana,
        RuleWideAlphabet,
        RuleWideNumeric,
        RuleWideSpace,
        RuleWideExclamation,
        RuleWideSharp,
        RuleWideDollar,
        RuleWidePercent,
        RuleWideAmpersand,
        RuleWideParenthesis,
        RuleWideAsterisk,
        RuleWidePlus,
        RuleWideComma,
        RuleWideHyphen,
        RuleWideDot,
        RuleWideSlash,
        RuleWideColon,
        RuleWideSemicolon,
        RuleWideLessThan,
        RuleWideEquals,
        RuleWideGreaterThan,
        RuleWideQuestion,
        RuleWideAtMark,
        RuleWideSquareBracket,
        RuleWideCurlyBracket,
        RuleWideYen,
        RuleWideSingleQuotation,
        RuleWideDoubleQuotation
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Rules;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxTreeAction(AnalyzeComment);
    }

    private static void AnalyzeComment(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);

        string? source = null;
        foreach (var trivia in root.DescendantTrivia())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var kind = trivia.Kind();
            if ((kind != SyntaxKind.MultiLineCommentTrivia) &&
                (kind != SyntaxKind.SingleLineCommentTrivia) &&
                (kind != SyntaxKind.SingleLineDocumentationCommentTrivia) &&
                (kind != SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                continue;
            }

            source ??= context.Tree.GetText(context.CancellationToken).ToString();
            var span = source.AsSpan(trivia.SpanStart, trivia.Span.Length);
            switch (kind)
            {
                case SyntaxKind.MultiLineCommentTrivia:
                    var range = span.Slice(2);
                    if ((range.Length >= 2) && (range[range.Length - 2] == '*') && (range[range.Length - 1] == '/'))
                    {
                        range = range.Slice(0, range.Length - 2);
                    }

                    if (!range.IsEmpty)
                    {
                        CheckRules(context, trivia, range);
                    }
                    break;
                case SyntaxKind.SingleLineCommentTrivia:
                    if (span.Length > 2)
                    {
                        CheckRules(context, trivia, span.Slice(2));
                    }
                    break;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    CheckRules(context, trivia, span);
                    break;
            }
        }
    }

    private static void CheckRules(SyntaxTreeAnalysisContext context, SyntaxTrivia node, ReadOnlySpan<char> range)
    {
        var flags = AnalyzeCharacters(range, context.CancellationToken);
        var location = node.GetLocation();

        if ((flags & CommentCharFlags.NarrowKana) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleNarrowKana, location));
        if ((flags & CommentCharFlags.WideAlphabet) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideAlphabet, location));
        if ((flags & CommentCharFlags.WideNumeric) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideNumeric, location));
        if ((flags & CommentCharFlags.WideSpace) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideSpace, location));
        if ((flags & CommentCharFlags.WideSingleQuotation) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideSingleQuotation, location));
        if ((flags & CommentCharFlags.WideDoubleQuotation) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideDoubleQuotation, location));
        if ((flags & CommentCharFlags.WideExclamation) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideExclamation, location));
        if ((flags & CommentCharFlags.WideSharp) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideSharp, location));
        if ((flags & CommentCharFlags.WideDollar) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideDollar, location));
        if ((flags & CommentCharFlags.WidePercent) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWidePercent, location));
        if ((flags & CommentCharFlags.WideAmpersand) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideAmpersand, location));
        if ((flags & CommentCharFlags.WideParenthesis) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideParenthesis, location));
        if ((flags & CommentCharFlags.WideAsterisk) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideAsterisk, location));
        if ((flags & CommentCharFlags.WidePlus) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWidePlus, location));
        if ((flags & CommentCharFlags.WideComma) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideComma, location));
        if ((flags & CommentCharFlags.WideHyphen) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideHyphen, location));
        if ((flags & CommentCharFlags.WideDot) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideDot, location));
        if ((flags & CommentCharFlags.WideSlash) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideSlash, location));
        if ((flags & CommentCharFlags.WideColon) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideColon, location));
        if ((flags & CommentCharFlags.WideSemicolon) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideSemicolon, location));
        if ((flags & CommentCharFlags.WideLessThan) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideLessThan, location));
        if ((flags & CommentCharFlags.WideEquals) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideEquals, location));
        if ((flags & CommentCharFlags.WideGreaterThan) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideGreaterThan, location));
        if ((flags & CommentCharFlags.WideQuestion) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideQuestion, location));
        if ((flags & CommentCharFlags.WideAtMark) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideAtMark, location));
        if ((flags & CommentCharFlags.WideSquareBracket) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideSquareBracket, location));
        if ((flags & CommentCharFlags.WideCurlyBracket) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideCurlyBracket, location));
        if ((flags & CommentCharFlags.WideYen) != 0) context.ReportDiagnostic(Diagnostic.Create(RuleWideYen, location));
    }

    private static CommentCharFlags AnalyzeCharacters(ReadOnlySpan<char> range, CancellationToken cancellationToken)
    {
        var flags = CommentCharFlags.None;
        for (var i = 0; i < range.Length; i++)
        {
            if ((i & 0x3FFF) == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            flags |= Classify(range[i]);
        }

        return flags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CommentCharFlags Classify(char c)
    {
        if (IsNarrowKana(c)) return CommentCharFlags.NarrowKana;
        if (IsWideAlphabet(c)) return CommentCharFlags.WideAlphabet;
        if (IsWideNumeric(c)) return CommentCharFlags.WideNumeric;

        return c switch
        {
            '　' => CommentCharFlags.WideSpace,
            '’' => CommentCharFlags.WideSingleQuotation,
            '”' => CommentCharFlags.WideDoubleQuotation,
            '！' => CommentCharFlags.WideExclamation,
            '＃' => CommentCharFlags.WideSharp,
            '＄' => CommentCharFlags.WideDollar,
            '％' => CommentCharFlags.WidePercent,
            '＆' => CommentCharFlags.WideAmpersand,
            '（' or '）' => CommentCharFlags.WideParenthesis,
            '＊' => CommentCharFlags.WideAsterisk,
            '＋' => CommentCharFlags.WidePlus,
            '，' => CommentCharFlags.WideComma,
            '－' => CommentCharFlags.WideHyphen,
            '．' => CommentCharFlags.WideDot,
            '／' => CommentCharFlags.WideSlash,
            '：' => CommentCharFlags.WideColon,
            '；' => CommentCharFlags.WideSemicolon,
            '＜' => CommentCharFlags.WideLessThan,
            '＝' => CommentCharFlags.WideEquals,
            '＞' => CommentCharFlags.WideGreaterThan,
            '？' => CommentCharFlags.WideQuestion,
            '＠' => CommentCharFlags.WideAtMark,
            '［' or '］' => CommentCharFlags.WideSquareBracket,
            '｛' or '｝' => CommentCharFlags.WideCurlyBracket,
            '￥' => CommentCharFlags.WideYen,
            _ => CommentCharFlags.None
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNarrowKana(char c) =>
        (uint)(c - 0xFF61) <= 0xFF9F - 0xFF61;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWideAlphabet(char c) =>
        ((uint)(c - 0xFF21) <= 0xFF3A - 0xFF21) ||
        ((uint)(c - 0xFF41) <= 0xFF5A - 0xFF41);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWideNumeric(char c) =>
        (uint)(c - 0xFF10) <= 0xFF19 - 0xFF10;
}
