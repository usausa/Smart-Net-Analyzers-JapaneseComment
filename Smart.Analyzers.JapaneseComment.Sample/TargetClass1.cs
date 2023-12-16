// ReSharper disable InvalidXmlDocComment
namespace Smart.Analyzers.JapaneseComment.Sample;

// ｱｱｱ
// ＡＡＡ
public sealed class TargetClass1
{
    /// <summary>
    /// ｱｱｱ
    /// </summary>
    public static void Test()
    {
        // １
        /* [　] */
        /* ？ */
        /* ： */
        /// １

        // ’
        // ”
    }
}
