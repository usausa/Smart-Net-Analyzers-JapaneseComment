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
            id: RuleIdentifiers.AlphabetCharacterInCommentShouldBeNarrow,
            title: "Alphabet character in comment should be narrow",
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
            id: RuleIdentifiers.SpaceCharacterInCommentShouldBeNarrow,
            title: "Space character in comment should be narrow",
            messageFormat: "Space character in comment should be narrow",
            category: "Style",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: string.Empty);

        private static readonly DiagnosticDescriptor RuleWideAscii = new(
            id: RuleIdentifiers.AsciiCharacterInCommentShouldBeNarrow,
            title: "ASCII character in comment should be narrow",
            messageFormat: "ASCII character in comment should be narrow",
            category: "Style",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: string.Empty);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            RuleNarrowKana,
            RuleWideAlphabet,
            RuleWideNumeric,
            RuleWideSpace,
            RuleWideAscii);

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
                            x.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia))
                .ToArray(); // TODO

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
            var wideAsciiExist = false;

            foreach (var c in range)
            {
                if (!narrowKanaExist && IsNarrowKana(c)) narrowKanaExist = true;

                if (!wideAlphabetExist && IsWideAlphabet(c)) wideAlphabetExist = true;

                if (!wideNumericExist && IsWideNumeric(c)) wideNumericExist = true;

                if (!wideSpaceExist && IsWideSpace(c)) wideSpaceExist = true;

                if (!wideAsciiExist && IsWideAscii(c)) wideAsciiExist = true;
            }

            if (narrowKanaExist)
            {
                var diagnostic = Diagnostic.Create(RuleNarrowKana, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            if (wideAlphabetExist)
            {
                var diagnostic = Diagnostic.Create(RuleWideAlphabet, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            if (wideNumericExist)
            {
                var diagnostic = Diagnostic.Create(RuleWideNumeric, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            if (wideSpaceExist)
            {
                var diagnostic = Diagnostic.Create(RuleWideSpace, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            if (wideAsciiExist)
            {
                var diagnostic = Diagnostic.Create(RuleWideAscii, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWideSpace(char c)
        {
            return c == '　';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWideAscii(char c)
        {
            // ’
            if (c == 0x2019) return true;
            // ”
            if (c == 0x201D) return true;
            // ！
            if (c == 0xFF01) return true;
            // ＃＄％＆
            if (c < 0xFF03) return false;
            if (c <= 0xFF06) return true;
            // （）＊＋，－．／
            if (c < 0xFF08) return false;
            if (c <= 0xFF0F) return true;
            // ：；＜＝＞？＠
            if (c < 0xFF1A) return false;
            if (c <= 0xFF20) return true;
            // ［
            if (c == 0xFF3B) return true;
            // ］＾＿｀
            if (c < 0xFF3D) return false;
            if (c <= 0xFF40) return true;
            // ｛｜｝
            if (c < 0xFF5B) return false;
            if (c <= 0xFF5D) return true;
            // ￣
            if (c == 0xFFE3) return true;
            // ￥
            if (c == 0xFFE5) return true;
            return false;
        }
    }
}
