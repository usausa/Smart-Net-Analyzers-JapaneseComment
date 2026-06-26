# Smart.Analyzers.JapaneseComment

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.Analyzers.JapaneseComment.svg)](https://www.nuget.org/packages/Usa.Smart.Analyzers.JapaneseComment)

Roslyn analyzer that enforces character width conventions in C# code comments.

## Rules

Each rule flags a specific full-width character (or half-width kana) in comments and suggests its counterpart.
Off-by-default rules must be enabled explicitly, e.g. `dotnet_diagnostic.SAJFF06.severity = warning` in `.editorconfig`.

| Rule | Target | Replace with | Default |
|---|:---:|:---:|:---:|
| SAJ0001 | half-width kana (U+FF61–FF9F) | full-width kana | on |
| SAJ0002 | `Ａ`-`Ｚ` `ａ`-`ｚ` | `A`-`Z` `a`-`z` | on |
| SAJ0003 | `０`-`９` | `0`-`9` | on |
| SAJ0004 | ideographic space (U+3000) | space | on |
| SAJ2019 | `’` | `'` | on |
| SAJ201D | `”` | `"` | on |
| SAJFF01 | `！` | `!` | off |
| SAJFF03 | `＃` | `#` | on |
| SAJFF04 | `＄` | `$` | on |
| SAJFF05 | `％` | `%` | on |
| SAJFF06 | `＆` | `&` | off |
| SAJFF08 | `（` `）` | `(` `)` | on |
| SAJFF0A | `＊` | `*` | on |
| SAJFF0B | `＋` | `+` | on |
| SAJFF0C | `，` | `,` | off |
| SAJFF0D | `－` | `-` | on |
| SAJFF0E | `．` | `.` | off |
| SAJFF0F | `／` | `/` | on |
| SAJFF1A | `：` | `:` | on |
| SAJFF1B | `；` | `;` | on |
| SAJFF1C | `＜` | `<` | on |
| SAJFF1D | `＝` | `=` | on |
| SAJFF1E | `＞` | `>` | on |
| SAJFF1F | `？` | `?` | off |
| SAJFF20 | `＠` | `@` | on |
| SAJFF3B | `［` `］` | `[` `]` | on |
| SAJFF5B | `｛` `｝` | `{` `}` | on |
| SAJFFE5 | `￥` | `¥` | on |
