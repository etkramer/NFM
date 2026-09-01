# NFM

## Rules

- Keep this file minimal and rules-only. No architecture overviews, no file tours, no explanations.
- Build the whole solution:
  `& (& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -find "MSBuild\**\Bin\MSBuild.exe") Source/Solution.sln`. Never target individual .csproj files.
- Avoid comments. One line where genuinely non-obvious, rarely two. Never comment blocks.
- Write them as if the code was always this way. No narrating fixes, regressions or what it "used
  to" do, and no justifying code by the failure it avoids ("so X doesn't happen", "otherwise Y").
- When editing near an existing comment, leave it alone unless the change made it wrong.
