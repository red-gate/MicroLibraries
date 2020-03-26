# This file is a bootstrapper for the real build file. It's purpose is as follows:
#
# 1. Define some top-level fubctions (build, clean, rebuild) that can be used to kick off the build from the command-line.
# 2. Download nuget.exe and then obtain some NuGet packages that the real build script relies on.
# 3. Import the RedGate.Build module to make available some convenient build cmdlets.

$VerbosePreference = 'Continue'          # Want useful output in our build log files.
$ProgressPreference = 'SilentlyContinue' # Progress logging slows down TeamCity when downloading files with Invoke-WebRequest.
$ErrorActionPreference = 'Stop'          # Abort quickly on error.

function global:Build
{
    [CmdletBinding()]
    param(
        [string[]] $Task = @('Default'),
        [string] $Configuration = 'Release',
        [string]$NuGetFeedUrl,
        [string]$NuGetFeedApiKey
    )

    Push-Location $PsScriptRoot -Verbose
    try
    {
        # Obtain nuget.exe
        $NuGetVersion = [version] '4.1.0'
        $NuGetPath = '.\nuget.exe'
        if (-not (Test-Path $NuGetPath) -or (Get-Item $NuGetPath).VersionInfo.ProductVersion -ne $NuGetVersion)
        {
            $NuGetUrl = "https://dist.nuget.org/win-x86-commandline/v$NuGetVersion/nuget.exe"
            Write-Host "Downloading $NuGetUrl"
            Invoke-WebRequest $NuGetUrl -OutFile $NuGetPath
        }
        
        # Restore the 'build-level' nuget packages into .build/packages if necessary.
        $NuGetConfigXml = [xml](Get-Content 'packages.config')
        $NuGetConfigXml.packages.package | ForEach-Object {
            & $NuGetPath install $_.id `
                -Version $_.version `
                -OutputDirectory 'packages' `
                -ExcludeVersion `
                -PackageSaveMode nuspec
        }

        # Import the RedGate.Build module.
        Import-Module '.\packages\RedGate.Build\tools\RedGate.Build.psm1' -Force

        # Call the actual build script.
        & '.\packages\Invoke-Build\tools\Invoke-Build.ps1' -File .\build.ps1 -Task $Task -Configuration $Configuration -NuGetFeedUrl $NuGetFeedUrl -NuGetFeedApiKey $NuGetFeedApiKey
    }
    finally
    {
        Pop-Location
    }
}

function global:Clean 
{
    Build -Task Clean
}

function global:Rebuild
{
    Build -Task Rebuild
}

function global:Publish
{
    Build -Task Publish
}

Write-Host 'The following commands are now available:' -ForegroundColor Magenta
Write-Host "    Build [-Task <task-list>] [-Configuration <Debug|Release>] [-NuGetFeedUrl <url> -NuGetFeedApiKey <key>]" -ForegroundColor Green
Write-Host "    Clean" -ForegroundColor Green
Write-Host "    Rebuild" -ForegroundColor Green
Write-Host "    Publish" -ForegroundColor Green
