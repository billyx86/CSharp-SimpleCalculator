# CSharp-SimpleCalculator

A simple command line calculator written in C#. You are asked for two numbers and
an operator, and it prints the result. It then loops back and asks for the next
calculation, so you can keep going until you type `exit`.

It was written as a first full program and GitHub repository, so it is
deliberately small and heavily commented — each line explains what is happening
and why.

## Requirements

The project targets **.NET Framework 4.7.2**, but on Linux/Mac it builds and runs
under **Mono**. On Windows you can also open the `.sln` in Visual Studio.

- Linux/Mac: [Mono](https://www.mono-project.com/) (provides `mcs` and `mono`)
- Windows: .NET Framework 4.7.2 (or Visual Studio)

## Building

With Mono, from the repository root:

```sh
mcs -warn:4 -out:SimpleCalc/bin/Calc.exe SimpleCalc/Program.cs
```

(As with `mcs`, create the output directory first if it does not already exist —
the CI workflow does `mkdir -p SimpleCalc/bin` before compiling.)

## Running

```sh
mono SimpleCalc/bin/Calc.exe
```

Example session:

```
Enter your first number:
12
Enter your second number:
4
Enter your operator type's symbol (or type exit):
*
48
Enter your first number:
10
Enter your second number:
3
Enter your operator type's symbol (or type exit):
exit
Goodbye.
```

## Input

Two numbers (whole or decimal), then an operator. The accepted operators are:

| Symbol | Meaning     |
|--------|-------------|
| `+`    | Add         |
| `-`    | Subtract    |
| `*`    | Multiply    |
| `/`    | Divide      |
| `^`    | Power       |
| `%`    | Modulo      |
| `exit` | End the session |
| `history` | Show the results of the last 3 calculations |
| `clear` | Forget all remembered results |

Notes on edge cases:

- **Division by zero** is guarded — it prints `Cannot divide by zero.` rather than `Infinity`.
- **Modulo by zero** is guarded the same way — `1 % 0` prints
  `Cannot take the modulo of zero.` rather than `NaN`.
- **`NaN`, `Infinity` and `-Infinity`** are rejected as numbers even though
  `double.TryParse` accepts them, so they can never sneak into a result.
- **Overflowing results** are reported, not printed. Valid finite inputs can
  still overflow the arithmetic (`1e308 * 10`, `1e308 + 1e308`, `1e308 - -1e308`
  and `1e308 ^ 2` all exceed `double.MaxValue`, ~1.8e308); the program prints a
  friendly "result overflowed" message instead of `Infinity`.
- **Undefined powers** are reported separately from overflow. A fractional
  power of a negative base (for example `(-8) ^ 0.5`) is not a number that is
  *too large* — it is simply not defined, and `Math.Pow` reports it as `NaN`.
  The program prints "This power is not defined for the given numbers." for
  that case, keeping it distinct from the overflow message.
- **Invalid numbers or operators** print a message and simply re-prompt; they do
  not end the session.
- **End of input** (for example when the program is run from a pipe and the input
  runs out) ends the session cleanly instead of formatting an empty calculation.
- **Locale-independent numbers**: numbers are parsed and printed using the
  invariant culture, so the decimal point is always a dot no matter which
  locale the program runs under. Under e.g. `de_DE` the input `5.5` still means
  five-and-a-half (it is not read as the integer `55`), and `10 / 4` prints
  `2.5`, not `2,5`.
- **Operator input is forgiving**: the operator is trimmed before it is
  compared, so `+ ` with a stray space works. The exit keyword is matched
  case-insensitively, so `exit`, `Exit` and ` EXIT ` all end the session. The
  arithmetic operators themselves keep exact matching — `+` is still not the
  same as `&`.
- **Power (`^`)** uses `Math.Pow`, because C#'s `^` operator is bitwise XOR
  and is not defined for `double`. Like every other operation its result is
  routed through the overflow guard, so `1e308 ^ 2` is reported rather than
  printed as `Infinity`.

## History

Typing `history` (case-insensitive, like `exit`) where the operator is
expected prints the results of the last three calculations, oldest first, and
then continues the session:

```
Enter your first number:
1
Enter your second number:
2
Enter your operator type's symbol (or type exit):
+
3
Enter your first number:
4
Enter your second number:
5
Enter your operator type's symbol (or type exit):
*
20
Enter your first number:
1
Enter your second number:
1
Enter your operator type's symbol (or type exit):
history
Recent results (oldest first):
3
20
```

Only successful, finite results are remembered — division/modulo by zero and
overflowing or undefined results are not. When more than three results exist,
the oldest one drops off.

Typing `clear` (case-insensitive, like `history`) where the operator is
expected empties the remembered results and prints `History cleared.`:

```
Enter your first number:
1
Enter your second number:
2
Enter your operator type's symbol (or type exit):
+
3
Enter your first number:
1
Enter your second number:
1
Enter your operator type's symbol (or type exit):
clear
History cleared.
Enter your first number:
1
Enter your second number:
1
Enter your operator type's symbol (or type exit):
history
No results yet.
```

## Tests

The [CI workflow](.github/workflows/ci.yml) compiles the program with `mcs -warn:4`
and runs a set of smoke tests (a normal calculation, division by zero, a multi-calc
session, `NaN`/`Infinity` rejection, a clean end-of-file exit, locale-independent
number parsing/printing under `de_DE`, forgiving operator input, power (`^`),
modulo (`%`) including modulo-by-zero, the `history` command, the `clear`
command, overflow results being reported rather than printed as `Infinity`, and
undefined powers (`(-8) ^ 0.5`) getting their own not-defined message) using
`mono`.

## License

GPL-3.0 — see the [LICENSE](LICENSE) file.
