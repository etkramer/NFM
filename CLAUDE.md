# NFM

## Rules

- Keep this file minimal and rules-only. No architecture overviews, no file tours, no explanations.
- Build the whole solution: `dotnet build Source/Solution.sln`. Never target individual .csproj files.
- Avoid comments. One line where genuinely non-obvious, never comment blocks.
- Comments describe what the code does now. Never write a comment that narrates a fix, a past bug, a
  regression, or what the code "used to" do - that context is dead the moment the commit lands.
- When editing near an existing comment, leave it alone unless the change made it wrong.
