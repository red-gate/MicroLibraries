# Initializes the build system (which declares four global build functions, build, clean, rebuild and package) then starts a build.
# The real work of the build system is defined in .build\build.ps1 and .build\_init.ps1.
& "$PsScriptRoot\.build\_init.ps1"
Rebuild
