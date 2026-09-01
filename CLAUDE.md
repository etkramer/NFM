# NFM

## Rules

- Keep this file minimal and rules-only. No architecture overviews, no file tours, no explanations.
- Build the whole solution:
  `& (& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -find "MSBuild\**\Bin\MSBuild.exe") Source/Solution.sln`. Never target individual .csproj files.
- Minimize the contract, not the line count. Heavy shared machinery is fine when it makes what gets
  written often - callers, subclasses, plugins - nearly free. Prefer an inline Guard assert over a
  defensive branch.
- Comments are one-line labels that let a block be skimmed past. Never a paragraph.
- Write them as if the code was always this way. No narrating fixes, regressions or what it "used
  to" do, and no justifying code by the failure it avoids ("so X doesn't happen", "otherwise Y").
- When editing near an existing comment, leave it alone unless the change made it wrong.
