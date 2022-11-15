namespace Smart.Analyzers.JapaneseComment.Sample;

// ｱｱｱ
// ＡＡＡ
#pragma warning disable CA1052
public class TargetClass1
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
#pragma warning restore CA1052
