# Smart.Analyzers.JapaneseComment

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.Analyzers.JapaneseComment.svg)](https://www.nuget.org/packages/Usa.Smart.Analyzers.JapaneseComment)

Roslyn analyzer that enforces character width conventions in C# code comments.

## Rules

| Rule | Description |
|---|---|
| SAJ0001 | Kana characters in comments should be wide (no half-width kana) |
| SAJ0002 | Alphabet characters in comments should be narrow (no full-width ABC) |
| SAJ0003 | Numeric characters in comments should be narrow (no full-width 123) |
| SAJ0004 | Space characters in comments should be narrow (no full-width space) |
| SAJ2019 | Single quotation mark in comments should be narrow |
| SAJ201D | Double quotation mark in comments should be narrow |
| SAJFF01 | Exclamation mark in comments should be narrow |
| SAJFF03 | Sharp sign in comments should be narrow |
| SAJFF04 | Dollar sign in comments should be narrow |
| SAJFF05 | Percent sign in comments should be narrow |
| SAJFF06 | Ampersand in comments should be narrow |
| SAJFF08 | Parentheses in comments should be narrow |
| SAJFF0A | Asterisk in comments should be narrow |
| SAJFF0B | Plus sign in comments should be narrow |
| SAJFF0C | Comma in comments should be narrow |
| SAJFF0D | Hyphen in comments should be narrow |
| SAJFF0E | Dot in comments should be narrow |
| SAJFF0F | Slash in comments should be narrow |
| SAJFF1A | Colon in comments should be narrow |
| SAJFF1B | Semicolon in comments should be narrow |
| SAJFF1C | Less-than sign in comments should be narrow |
| SAJFF1D | Equals sign in comments should be narrow |
| SAJFF1E | Greater-than sign in comments should be narrow |
| SAJFF1F | Question mark in comments should be narrow |
| SAJFF20 | At mark in comments should be narrow |
| SAJFF3B | Square brackets in comments should be narrow |
| SAJFF5B | Curly brackets in comments should be narrow |
| SAJFFE5 | Yen sign in comments should be narrow |
