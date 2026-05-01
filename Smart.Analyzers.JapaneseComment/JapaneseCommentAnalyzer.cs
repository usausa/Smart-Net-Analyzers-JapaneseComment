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

    private static readonly DiagnosticDescriptor RuleNarrowKana = new(
        id: RuleIdentifiers.KanaCharacterInCommentShouldBeWide,
        title: "Kana character in comment should be wide",
        messageFormat: "Kana character in comment should be wide",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideAlphabet = new(
        id: RuleIdentifiers.AlphabetInCommentShouldBeNarrow,
        title: "Alphabet in comment should be narrow",
        messageFormat: "Alphabet character in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideNumeric = new(
        id: RuleIdentifiers.NumericCharacterInCommentShouldBeNarrow,
        title: "Numeric character in comment should be narrow",
        messageFormat: "Numeric character in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideSpace = new(
        id: RuleIdentifiers.SpaceInCommentShouldBeNarrow,
        title: "Space in comment should be narrow",
        messageFormat: "Space in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideSingleQuotation = new(
        id: RuleIdentifiers.SingleQuotationInCommentShouldBeNarrow,
        title: "\"’\" in comment should be narrow",
        messageFormat: "\"’\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideDoubleQuotation = new(
        id: RuleIdentifiers.DoubleQuotationInCommentShouldBeNarrow,
        title: "\"”\" in comment should be narrow",
        messageFormat: "\"”\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideExclamation = new(
        id: RuleIdentifiers.ExclamationInCommentShouldBeNarrow,
        title: "\"！\" in comment should be narrow",
        messageFormat: "\"！\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,  // Default off
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideSharp = new(
        id: RuleIdentifiers.SharpInCommentShouldBeNarrow,
        title: "\"＃\" in comment should be narrow",
        messageFormat: "\"＃\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideDollar = new(
        id: RuleIdentifiers.DollarInCommentShouldBeNarrow,
        title: "\"＄\" in comment should be narrow",
        messageFormat: "\"＄\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWidePercent = new(
        id: RuleIdentifiers.PercentInCommentShouldBeNarrow,
        title: "\"％\" in comment should be narrow",
        messageFormat: "\"％\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideAmpersand = new(
        id: RuleIdentifiers.AmpersandInCommentShouldBeNarrow,
        title: "\"＆\" in comment should be narrow",
        messageFormat: "\"％\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,  // Default off
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideParenthesis = new(
        id: RuleIdentifiers.ParenthesisInCommentShouldBeNarrow,
        title: "\"（）\" in comment should be narrow",
        messageFormat: "\"（）\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideAsterisk = new(
        id: RuleIdentifiers.AsteriskInCommentShouldBeNarrow,
        title: "\"＊\" in comment should be narrow",
        messageFormat: "\"＊\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWidePlus = new(
        id: RuleIdentifiers.PlusInCommentShouldBeNarrow,
        title: "\"＋\" in comment should be narrow",
        messageFormat: "\"＋\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideComma = new(
        id: RuleIdentifiers.CommaInCommentShouldBeNarrow,
        title: "\"，\" in comment should be narrow",
        messageFormat: "\"，\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,  // Default off
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideHyphen = new(
        id: RuleIdentifiers.HyphenInCommentShouldBeNarrow,
        title: "\"－\" in comment should be narrow",
        messageFormat: "\"－\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideDot = new(
        id: RuleIdentifiers.DotInCommentShouldBeNarrow,
        title: "\"．\" in comment should be narrow",
        messageFormat: "\"．\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,  // Default off
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideSlash = new(
        id: RuleIdentifiers.SlashInCommentShouldBeNarrow,
        title: "\"／\" in comment should be narrow",
        messageFormat: "\"／\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideColon = new(
        id: RuleIdentifiers.ColonInCommentShouldBeNarrow,
        title: "\"：\" in comment should be narrow",
        messageFormat: "\"：\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideSemicolon = new(
        id: RuleIdentifiers.SemicolonInCommentShouldBeNarrow,
        title: "\"；\" in comment should be narrow",
        messageFormat: "\"；\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideLessThan = new(
        id: RuleIdentifiers.LessThanInCommentShouldBeNarrow,
        title: "\"＜\" in comment should be narrow",
        messageFormat: "\"＜\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideEquals = new(
        id: RuleIdentifiers.EqualsInCommentShouldBeNarrow,
        title: "\"＝\" in comment should be narrow",
        messageFormat: "\"＝\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideGreaterThan = new(
        id: RuleIdentifiers.GreaterThanInCommentShouldBeNarrow,
        title: "\"＞\" in comment should be narrow",
        messageFormat: "\"＞\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideQuestion = new(
        id: RuleIdentifiers.QuestionInCommentShouldBeNarrow,
        title: "\"？\" in comment should be narrow",
        messageFormat: "\"？\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,  // Default off
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideAtMark = new(
        id: RuleIdentifiers.AtMarkInCommentShouldBeNarrow,
        title: "\"＠\" in comment should be narrow",
        messageFormat: "\"＠\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideSquareBracket = new(
        id: RuleIdentifiers.SquareBracketInCommentShouldBeNarrow,
        title: "\"［］\" in comment should be narrow",
        messageFormat: "\"［］\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideCurlyBracket = new(
        id: RuleIdentifiers.CurlyBracketInCommentShouldBeNarrow,
        title: "\"｛｝\" in comment should be narrow",
        messageFormat: "\"｛｝\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    private static readonly DiagnosticDescriptor RuleWideYen = new(
        id: RuleIdentifiers.YenInCommentShouldBeNarrow,
        title: "\"￥\" in comment should be narrow",
        messageFormat: "\"￥\" in comment should be narrow",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
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

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxTreeAction(AnalyzeComment);
    }

    private static void AnalyzeComment(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);

        foreach (var trivia in root.DescendantTrivia())
        {
            var kind = trivia.Kind();
            if ((kind != SyntaxKind.MultiLineCommentTrivia) &&
                (kind != SyntaxKind.SingleLineCommentTrivia) &&
                (kind != SyntaxKind.DocumentationCommentExteriorTrivia))
            {
                continue;
            }

            var span = trivia.ToString().AsSpan();
            switch (kind)
            {
                case SyntaxKind.MultiLineCommentTrivia:
                    if (span.Length >= 4)
                    {
                        CheckRules(context, trivia, span.Slice(2, span.Length - 4));
                    }
                    break;
                case SyntaxKind.SingleLineCommentTrivia:
                    if (span.Length > 2)
                    {
                        CheckRules(context, trivia, span.Slice(2));
                    }
                    break;
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    if (span.Length > 3)
                    {
                        CheckRules(context, trivia, span.Slice(3));
                    }
                    break;
            }
        }
    }

    private static void CheckRules(SyntaxTreeAnalysisContext context, SyntaxTrivia node, ReadOnlySpan<char> range)
    {
        var flags = AnalyzeCharacters(range);
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

    private static CommentCharFlags AnalyzeCharacters(ReadOnlySpan<char> range)
    {
        var flags = CommentCharFlags.None;
        for (var i = 0; i < range.Length; i++)
        {
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
