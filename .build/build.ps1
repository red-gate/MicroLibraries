# This file cannot be invoked directly; it simply contains a bunch of Invoke-Build tasks. To use it, invoke
# _init.ps1 which declares four global functions (build, clean, rebuild, package), then invoke one of those functions.

[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$NuGetFeedUrl,
    [string]$NuGetFeedApiKey
)

# Useful paths used by multiple tasks.
$RepositoryRoot = "$PsScriptRoot\.." | Resolve-Path
$SourceDir = "$RepositoryRoot\Source" | Resolve-Path
$SolutionPath = "$SourceDir\Redgate.MicroLibraries.sln" | Resolve-Path
$NuGetPath = "$PsScriptRoot\nuget.exe" | Resolve-Path
$DistDir = "$RepositoryRoot\Dist"


# Helper function for clearer logging of each task.
function Write-Info {
    [CmdletBinding()]
    param ([string] $Message)

    Write-Host "## $Message ##" -ForegroundColor Magenta
}


function Get-BranchName {
    # If the branch name is specified via an environment variable (i.e. on TeamCity), use it.
    if ($env:BRANCH_NAME) {
        return $env:BRANCH_NAME
    }

    # If the .git folder is present, try to get the current branch using Git.
    $DotGitDirPath = "$RepositoryRoot\.git"
    if (Test-Path $DotGitDirPath) {
        return git rev-parse --abbrev-ref HEAD
    }

    # Otherwise, assume 'dev'
    Write-Warning "Unable to determine the current branch name using either git or the BRANCH_NAME environment variable. Defaulting to 'dev'."
    return 'dev'
}


# Clean task, deletes all build output folders.
task Clean {
    Write-Info 'Cleaning build output'

    Get-ChildItem $RepositoryRoot -Exclude @('packages') -Include @('Dist', 'bin', 'obj') -Directory -Recurse | ForEach-Object {
        Write-Host "Deleting $_"
        Remove-Item $_ -Force -Recurse
    }
}

task Compile {
    Write-Info "Running 'dotnet build'"

    Push-Location -Path $SourceDir
    try {
        exec {
            dotnet build --configuration $Configuration
        }
    } finally {
        Pop-Location
    }
}


# Test task, builds the solution and runs the automated tests.
task Test  Compile, {
    Write-Info "Running 'dotnet test'"

    Push-Location -Path $SourceDir
    try {
        exec {
            dotnet test --configuration $Configuration
        }
    } finally {
        Pop-Location
    }
}


# Package task, generates NuGet packages for each micro-library.
task Package {
    Write-Info 'Generating NuGet packages'

    $BranchName = Get-BranchName
    $IsDefaultBranch = $BranchName -eq 'main'

    # Loop through each project.
    Get-ChildItem $SourceDir -File -Filter '*.nuspec' -Recurse | ForEach-Object {
        $NuSpecPath = $_.FullName
        Write-Host "NuSpec file found: $NuSpecPath"

        $ProjectDir = Split-Path -Parent $NuSpecPath
        $ProjectName = $_.BaseName

        $PackageId = "RedGate.$ProjectName.Sources"
        $ReleaseNotesPath = "$ProjectDir\RELEASENOTES.md" | Resolve-Path
        $ReadmePath = "$ProjectDir\README.md" | Resolve-Path

        # Establish release notes and package version number from the RELEASENOTES.md file.
        $Notes = Read-ReleaseNotes $ReleaseNotesPath -ThreePartVersion
        $ReleaseNotes = $Notes.Content
        $Version = [version] $Notes.Version
        if ($env:BUILD_NUMBER) {
            $Version = [version] "$Version.$($env:BUILD_NUMBER)"
        }

        # Establish the summary and description from the README.md file.
        $Description = [IO.File]::ReadAllText($ReadmePath, [Text.Encoding]::UTF8).Trim()
        $Regex = [regex] '\.\s'
        $Summary = if ($Description -match $Regex) { ($Description -split $Regex)[0] + '.' } else { $Description }

        # Establish NuGet package version.
        $NuGetPackageVersion = New-SemanticNuGetPackageVersion -Version $Version -BranchName $BranchName -IsDefaultBranch $IsDefaultBranch

        # Establish the file header.
        $ProjectFilePath = "$ProjectDir\$ProjectName.csproj" | Resolve-Path
        $ProjectXml = [xml] (Get-Content $ProjectFilePath)
        $CopyrightYear = $ProjectXml.SelectSingleNode('//Copyright').InnerText
        $CurrentYear = Get-Date -Format yyyy
        if ($CurrentYear -ne $CopyrightYear) {
            $CopyrightYear = "$CopyrightYear-$CurrentYear"
        }
        $FileHeader = @"
/*
This file is part of the $PackageId NuGet package, version $NuGetPackageVersion.

Copyright $CopyrightYear Red Gate Software Limited (https://github.com/red-gate/MicroLibraries)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
License and this notice. You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
language governing permissions and limitations under the License.

In addition, the copyright holders grant permission to reproduce and distribute copies of this software or derivative
works thereof in any medium, with or without modifications, in Object form (as defined by the License), without
satisfying the requirements of section 4a of the License. In practice, this means that you are free to include this
library in binary releases of your own software without having to also include this notice, a copy of the Licence, or
any other copyright attribution.
*/

// @formatter:off
// ReSharper disable All
"@
        $FileFooter = @"
// @formatter:on
"@

        # Locate source files to be included in the package, and generate their corresponding .pp files.
        Get-ChildItem $ProjectDir -Filter *.cs | ForEach-Object {
            $InputPath = $_.FullName
            $Encoding = [Text.UTF8Encoding]::new($False, $True)
            $OriginalContents = [IO.File]::ReadAllText($InputPath, $Encoding)
            $ModifiedContents = $OriginalContents.Replace('/***', '').Replace('***/', '')
            if ($OriginalContents -ne $ModifiedContents) {
                Write-Host "  Including file $InputPath"
                
                $ModifiedContents = "$FileHeader`r`n`r`n$ModifiedContents`r`n`r`n$FileFooter"
                
                $OutputPath = "$InputPath.pp"
                Write-Host "    Rewriting to $OutputPath"
                [IO.File]::WriteAllText($OutputPath, $ModifiedContents, $Encoding)
            }
        }

        # Establish the output folder.
        $OutputDir = if ($PackageId -match 'ProjectTemplate' -or (Test-NugetPackage $PackageId $Version) -or (Test-NugetPackage $PackageId $NuGetPackageVersion)) {
            "$DistDir\Not publishable"
        } else {
            "$DistDir\Publishable"
        }
        $Null = mkdir $OutputDir -Force
        
        # Run NuGet pack.
        $Parameters = @(
            'pack',
            "$NuSpecPath",
            '-Version', $NuGetPackageVersion,
            '-OutputDirectory', $OutputDir,
            '-ForceEnglishOutput',
            '-Properties', "copyrightYear=$CopyrightYear;releaseNotes=$ReleaseNotes;summary=$Summary;description=$Description"
        )
        Write-Host "$NuGetPath $Parameters"
        exec {
            & $NuGetPath $Parameters
        }

        # Delete the temporary .pp files.
        Get-ChildItem $ProjectDir -Filter *.pp | Remove-Item
    }

    # Publish the publishable packages as TeamCity artefacts.
    if (Test-Path "$DistDir\Publishable") {
        Get-ChildItem "$DistDir\Publishable" -File -Filter '*.nupkg' | ForEach-Object { TeamCity-PublishArtifact $_.FullName } 
    }
}


# Publish task, publishes the NuGet package for each micro-library that is eligible for publication.
task Publish {
    Write-Info 'Publishing eligible NuGet packages'
    
    if (-not $NuGetFeedUrl -or -not $NuGetFeedApiKey) {
        Write-Warning 'Skipping publication - no NugetFeedUrl or NuGetFeedApiKey parameters specified'
    } else {
        if (-not (Test-Path $DistDir)) {
            throw "The $DistDir folder has not yet been created. Try running the Build or Package tasks first."
        } else {
            $PackagesDir = "$DistDir\Publishable"
            $PublishablePackages = if (Test-Path $PackagesDir) { Get-ChildItem $PackagesDir -Filter *.nupkg }
            Write-Host $PublishablePackages
            if (-not $PublishablePackages) {
                Write-Warning "Skipping publication - no eligible NuGet packages found in $PackagesDir"
            } else {
                $PublishablePackages | ForEach-Object {
                    # Run NuGet push.
                    $Parameters = @(
                        'push',
                        $_.FullName,
                        '-Source', $NuGetFeedUrl,
                        '-ApiKey', $NuGetFeedApiKey,
                        '-ForceEnglishOutput',
                        '-NonInteractive'
                    )
                    Write-Host "$NuGetPath $Parameters"
                    exec {
                        & $NuGetPath $Parameters
                    }
                }
            }
        }
    }
}



task Build  Test, Package, Publish
task Rebuild  Clean, Build
task Default  Build