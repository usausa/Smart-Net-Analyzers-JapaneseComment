# Diagnostics

Off-by-default rules must be enabled explicitly, e.g. `dotnet_diagnostic.SAJFF06.severity = warning` in `.editorconfig`.

| ID | Severity | Description | How to fix |
|---|---|---|---|
| SAJ0001 | ⚠️ Warning | Half-width kana (U+FF61-FF9F) in a comment | Replace with full-width kana |
| SAJ0002 | ⚠️ Warning | Full-width `Ａ`-`Ｚ` / `ａ`-`ｚ` in a comment | Replace with `A`-`Z` / `a`-`z` |
| SAJ0003 | ⚠️ Warning | Full-width `０`-`９` in a comment | Replace with `0`-`9` |
| SAJ0004 | ⚠️ Warning | Ideographic space (U+3000) in a comment | Replace with a half-width space |
| SAJ2019 | ⚠️ Warning | Full-width `’` in a comment | Replace with `'` |
| SAJ201D | ⚠️ Warning | Full-width `”` in a comment | Replace with `"` |
| SAJFF01 | ⚠️ Warning (off by default) | Full-width `！` in a comment | Replace with `!` |
| SAJFF03 | ⚠️ Warning | Full-width `＃` in a comment | Replace with `#` |
| SAJFF04 | ⚠️ Warning | Full-width `＄` in a comment | Replace with `$` |
| SAJFF05 | ⚠️ Warning | Full-width `％` in a comment | Replace with `%` |
| SAJFF06 | ⚠️ Warning (off by default) | Full-width `＆` in a comment | Replace with `&` |
| SAJFF08 | ⚠️ Warning | Full-width `（）` in a comment | Replace with `()` |
| SAJFF0A | ⚠️ Warning | Full-width `＊` in a comment | Replace with `*` |
| SAJFF0B | ⚠️ Warning | Full-width `＋` in a comment | Replace with `+` |
| SAJFF0C | ⚠️ Warning (off by default) | Full-width `，` in a comment | Replace with `,` |
| SAJFF0D | ⚠️ Warning | Full-width `－` in a comment | Replace with `-` |
| SAJFF0E | ⚠️ Warning (off by default) | Full-width `．` in a comment | Replace with `.` |
| SAJFF0F | ⚠️ Warning | Full-width `／` in a comment | Replace with `/` |
| SAJFF1A | ⚠️ Warning | Full-width `：` in a comment | Replace with `:` |
| SAJFF1B | ⚠️ Warning | Full-width `；` in a comment | Replace with `;` |
| SAJFF1C | ⚠️ Warning | Full-width `＜` in a comment | Replace with `<` |
| SAJFF1D | ⚠️ Warning | Full-width `＝` in a comment | Replace with `=` |
| SAJFF1E | ⚠️ Warning | Full-width `＞` in a comment | Replace with `>` |
| SAJFF1F | ⚠️ Warning (off by default) | Full-width `？` in a comment | Replace with `?` |
| SAJFF20 | ⚠️ Warning | Full-width `＠` in a comment | Replace with `@` |
| SAJFF3B | ⚠️ Warning | Full-width `［］` in a comment | Replace with `[]` |
| SAJFF5B | ⚠️ Warning | Full-width `｛｝` in a comment | Replace with `{}` |
| SAJFFE5 | ⚠️ Warning | Full-width `￥` in a comment | Replace with `¥` |
