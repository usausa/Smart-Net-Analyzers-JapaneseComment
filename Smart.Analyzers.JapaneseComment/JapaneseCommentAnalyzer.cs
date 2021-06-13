namespace Smart.Analyzers.JapaneseComment
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class JapaneseCommentAnalyzer : DiagnosticAnalyzer
    {
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
            isEnabledByDefault: true,
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
            isEnabledByDefault: true,
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
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
            RuleWideDoubleQuotation);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(AnalyzeComment);
        }

        private void AnalyzeComment(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            var commentNodes = root.DescendantTrivia()
                .Where(x => x.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                            x.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                            x.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia));

            foreach (var node in commentNodes)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                    case SyntaxKind.DocumentationCommentExteriorTrivia:
                        CheckRules(context, node, node.ToString().TrimStart('/').AsSpan());
                        break;
                    case SyntaxKind.MultiLineCommentTrivia:
                        var text = node.ToString();
                        CheckRules(context, node, text.AsSpan(2, text.Length - 4));
                        break;
                }
            }
        }

        private static void CheckRules(SyntaxTreeAnalysisContext context, SyntaxTrivia node, ReadOnlySpan<char> range)
        {
            var narrowKanaExist = false;
            var wideAlphabetExist = false;
            var wideNumericExist = false;
            var wideSpaceExist = false;
            var wideSingleQuotationExist = false;
            var wideDoubleQuotationExist = false;
            var wideExclamationExist = false;
            var wideSharpExist = false;
            var wideDollarExist = false;
            var widePercentExist = false;
            var wideAmpersandExist = false;
            var wideParenthesisExist = false;
            var wideAsteriskExist = false;
            var widePlusExist = false;
            var wideCommaExist = false;
            var wideHyphenExist = false;
            var wideDotExist = false;
            var wideSlashExist = false;
            var wideColonExist = false;
            var wideSemicolonExist = false;
            var wideLessThanExist = false;
            var wideEqualsExist = false;
            var wideGreaterThanExist = false;
            var wideQuestionExist = false;
            var wideAtMarkExist = false;
            var wideSquareBracketExist = false;
            var wideCurlyBracketExist = false;
            var wideYenExist = false;

            foreach (var c in range)
            {
                if (!narrowKanaExist && IsNarrowKana(c)) narrowKanaExist = true;
                if (!wideAlphabetExist && IsWideAlphabet(c)) wideAlphabetExist = true;
                if (!wideNumericExist && IsWideNumeric(c)) wideNumericExist = true;
                if (!wideSpaceExist && (c == '　')) wideSpaceExist = true;

                if (!wideSingleQuotationExist && (c == '’')) wideSingleQuotationExist = true;
                if (!wideDoubleQuotationExist && (c == '”')) wideDoubleQuotationExist = true;

                if (!wideExclamationExist && (c == '！')) wideExclamationExist = true;
                if (!wideSharpExist && (c == '＃')) wideSharpExist = true;
                if (!wideDollarExist && (c == '＄')) wideDollarExist = true;
                if (!widePercentExist && (c == '％')) widePercentExist = true;
                if (!wideAmpersandExist && (c == '＆')) wideAmpersandExist = true;

                if (!wideParenthesisExist && ((c == '（') || (c == '）'))) wideParenthesisExist = true;

                if (!wideAsteriskExist && (c == '＊')) wideAsteriskExist = true;
                if (!widePlusExist && (c == '＋')) widePlusExist = true;
                if (!wideCommaExist && (c == '，')) wideCommaExist = true;
                if (!wideHyphenExist && (c == '－')) wideHyphenExist = true;
                if (!wideDotExist && (c == '．')) wideDotExist = true;
                if (!wideSlashExist && (c == '／')) wideSlashExist = true;
                if (!wideColonExist && (c == '：')) wideColonExist = true;
                if (!wideSemicolonExist && (c == '；')) wideSemicolonExist = true;
                if (!wideLessThanExist && (c == '＜')) wideLessThanExist = true;
                if (!wideEqualsExist && (c == '＝')) wideEqualsExist = true;
                if (!wideGreaterThanExist && (c == '＞')) wideGreaterThanExist = true;
                if (!wideQuestionExist && (c == '？')) wideQuestionExist = true;
                if (!wideAtMarkExist && (c == '＠')) wideAtMarkExist = true;

                if (!wideSquareBracketExist && ((c == '［') || (c == '］'))) wideSquareBracketExist = true;
                if (!wideCurlyBracketExist && ((c == '｛') || (c == '｝'))) wideCurlyBracketExist = true;

                if (!wideYenExist && (c == '￥')) wideYenExist = true;
            }

            if (narrowKanaExist) context.ReportDiagnostic(Diagnostic.Create(RuleNarrowKana, node.GetLocation()));
            if (wideAlphabetExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideAlphabet, node.GetLocation()));
            if (wideNumericExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideNumeric, node.GetLocation()));
            if (wideSpaceExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideSpace, node.GetLocation()));

            if (wideSingleQuotationExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideSingleQuotation, node.GetLocation()));
            if (wideDoubleQuotationExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideDoubleQuotation, node.GetLocation()));

            if (wideExclamationExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideExclamation, node.GetLocation()));
            if (wideSharpExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideSharp, node.GetLocation()));
            if (wideDollarExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideDollar, node.GetLocation()));
            if (widePercentExist) context.ReportDiagnostic(Diagnostic.Create(RuleWidePercent, node.GetLocation()));
            if (wideAmpersandExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideAmpersand, node.GetLocation()));

            if (wideParenthesisExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideParenthesis, node.GetLocation()));

            if (wideAsteriskExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideAsterisk, node.GetLocation()));
            if (widePlusExist) context.ReportDiagnostic(Diagnostic.Create(RuleWidePlus, node.GetLocation()));
            if (wideCommaExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideComma, node.GetLocation()));
            if (wideHyphenExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideHyphen, node.GetLocation()));
            if (wideDotExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideDot, node.GetLocation()));
            if (wideSlashExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideSlash, node.GetLocation()));
            if (wideColonExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideColon, node.GetLocation()));
            if (wideSemicolonExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideSemicolon, node.GetLocation()));
            if (wideLessThanExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideLessThan, node.GetLocation()));
            if (wideEqualsExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideEquals, node.GetLocation()));
            if (wideGreaterThanExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideGreaterThan, node.GetLocation()));
            if (wideQuestionExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideQuestion, node.GetLocation()));
            if (wideAtMarkExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideAtMark, node.GetLocation()));

            if (wideSquareBracketExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideSquareBracket, node.GetLocation()));
            if (wideCurlyBracketExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideCurlyBracket, node.GetLocation()));

            if (wideYenExist) context.ReportDiagnostic(Diagnostic.Create(RuleWideYen, node.GetLocation()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNarrowKana(char c)
        {
            return (c >= 0xFF61) && (c <= 0xFF9F);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWideAlphabet(char c)
        {
            // Ａ-Ｚ
            if (c < 0xFF21) return false;
            if (c <= 0xFF3A) return true;
            // ａ-ｚ
            if (c < 0xFF41) return false;
            if (c <= 0xFF5A) return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWideNumeric(char c)
        {
            return (c >= 0xFF10) && (c <= 0xFF19);
        }
    }
}
