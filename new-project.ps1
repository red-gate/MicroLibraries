#requires -Version 3.0

# This script is used to generate a new micro-library project. You only need to provide a
# name for the new library, and optionally specify which template project will be copied
# (currently there's only one template, and it's chosen by default), and the script will
# then generate everything you need to build your new micro-library.

# Run code in a script block to avoid polluting the global namespace.
& {
  # Helper functions.
  function Write-Info ($Message) { Write-Host $Message -ForegroundColor Yellow }

  function Test-HasValue ($String) { ![string]::IsNullOrWhiteSpace($String) }
  
  function Test-HasNoValue ($String) { !(Test-HasValue $String) }
  
  # Tidies up a project name, adding a `ULibs.` prefix if absent, and capitalising the first letter.
  function Tidy-ProjectName ($Name) {
    if ($Name.Split('.')[0] -eq 'ULibs') { $Name = $Name.Substring(6) }
    $Name = "$($Name.Substring(0, 1).ToUpperInvariant())$($Name.Substring(1))"
    "ULibs.$Name"
  }
  
  # Prompt the user for the name of a new project.
  $NewProjectName = Read-Host -Prompt 'Please enter the new project name'
  if (Test-HasNoValue $NewProjectName) { throw 'No project name specified' }
  $NewProjectName = Tidy-ProjectName($NewProjectName)
  Write-Info "New project name = $NewProjectName"
  
  # Check that the project folder doesn't already exist.
  $NewProjectDir = "$PSScriptRoot\Source\$NewProjectName"
  if (Test-Path $NewProjectDir) { throw "Project path $NewProjectDir already exists" }
  
  # Prompt the user for the name of the existing project to clone.
  $DefaultSourceProjectName = 'ULibs.UlibsProjectTemplate'
  $SourceProjectName = Read-Host -Prompt "Please enter the name of the existing project to clone [$DefaultSourceProjectName]"
  if (Test-HasNoValue($SourceProjectName)) { $SourceProjectName = $DefaultSourceProjectName}
  $SourceProjectName = Tidy-ProjectName($SourceProjectName)
  Write-Info "Project to clone = $SourceProjectName"
  
  # Check that the source project exists.
  $SourceProjectDir = "$PSScriptRoot\Source\$SourceProjectName"
  if (-not (Test-Path $SourceProjectDir)) { throw "No existing project found at $SourceProjectDir" }
  
  # Create the new project folder.
  Write-Info 'Creating new project folder'
  $Null = mkdir $NewProjectDir
  
  # Create a regex used for replacing the old name with the new one. We use a regex rather than
  # a simple `string.Replace($SourceProjectName, $NewProjectName)` to accomodate any accidental
  # case variations in any of the source project files.
  $NameRegex = [regex] "(?in)(?<=(^|\W))$($SourceProjectName.Replace('.', '\.'))(?=($|\W))"
  
  # The GUID for the new project.
  $NewProjectGuid = [guid]::NewGuid().ToString('B').ToUpperInvariant()
  Write-Host "New project GUID = $NewProjectGuid"
  
  # Now we can start copying files. We begin by looping through all the files in the source project.
  Write-Info 'Copying project files'
  Get-ChildItem $SourceProjectDir -Exclude *.user -Recurse |
  Where-Object { $_.FullName -notmatch '\\(obj|bin)($|\\)'} |
  ForEach-Object {
    # Establish paths for the current file.
    $SourceFilePath = $_.FullName
    Write-Info "  Source file: $SourceFilePath"
    
    $SourceFileRelativePath = $SourceFilePath.Substring($SourceProjectDir.Length + 1)
    Write-Host "    Source relative path: $SourceFileRelativePath"
    
    $OutputFileRelativePath = $NameRegex.Replace($SourceFileRelativePath, $NewProjectName)
    Write-Host "    Output relative path: $OutputFileRelativePath"
    
    $OutputFilePath = "$NewProjectDir\$OutputFileRelativePath"
    Write-Host "    Output file: $OutputFilePath"
    
    # Handle copying a folder.
    if ($_.PSIsContainer) {
      Write-Host "    Creating folder $OutputFilePath"
      $Null = mkdir $OutputFilePath
    }
    
    # Handle copying a file.
    else {
      Write-Host "    Creating file $OutputFilePath"

      # Read the contents of the source file.
      $Contents = [IO.File]::ReadAllText($SourceFilePath, [Text.Encoding]::UTF8)
    
      # Replace the source project name with the new project name wherever it occurs in the file.
      if ($NameRegex.IsMatch($Contents)) {
        Write-Host '    Substituting new project name in file contents'
        $Contents = $NameRegex.Replace($Contents, $NewProjectName)
      }
      
      # Special treatment for the project file, to update the copyright year.
      if ([IO.Path]::GetExtension($OutputFileRelativePath) -eq '.csproj') {
        Write-Host '    Updating copyright year in project file contents'
        $CopyrightYearRegex = [regex] '(?<=\<Copyright\>).+?(?=\</Copyright\>)'
        $CurrentYear = Get-Date -Format yyyy
        $Contents = $CopyrightYearRegex.Replace($Contents, $CurrentYear)
      }
      
      # Write the contents to the output file.
      [IO.File]::WriteAllText($OutputFilePath, $Contents, [Text.Encoding]::UTF8)
    }
  }
  
  # Update the solution file to include the new project.
  $SolutionFilePath = "$PSScriptRoot\Source\Redgate.MicroLibraries.sln" | Resolve-Path
  Write-Info "Adding new project to solution file $SolutionFilePath"
  
  $Lines = [Collections.Generic.List[String]] [IO.File]::ReadAllLines($SolutionFilePath, [Text.Encoding]::UTF8)
  
  $Index = $Lines.IndexOf('Global')
  $NewLines = [Collections.Generic.List[String]] @(
    "Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""$NewProjectName"", ""$NewProjectName\$NewProjectName.csproj"", ""$NewProjectGuid""",
    'EndProject'
  )
  $Lines.InsertRange($Index, $NewLines)
  
  $Index = $Lines.IndexOf("`tEndGlobalSection")
  $Index = $Lines.IndexOf("`tEndGlobalSection", $Index + 1)
  $NewLines = [Collections.Generic.List[String]] @(
    "`t`t$NewProjectGuid.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
    "`t`t$NewProjectGuid.Debug|Any CPU.Build.0 = Debug|Any CPU",
    "`t`t$NewProjectGuid.Release|Any CPU.ActiveCfg = Release|Any CPU",
    "`t`t$NewProjectGuid.Release|Any CPU.Build.0 = Release|Any CPU"
  )
  $Lines.InsertRange($Index, $NewLines)
  
  [IO.File]::WriteAllLines($SolutionFilePath, $Lines, [Text.Encoding]::UTF8)
}