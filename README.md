# Redgate micro-libraries
A collection of lightweight, source-only micro-libraries for C#.

These micro-libraries are intended to provide small focused pieces of functionality in an extremely lightweight manner. By lightweight, we mean the following:

- Each micro-library is small, typically consisting of only a single source file.
- Each micro-library is delivered as source code rather than as a dll file. This means you don't need to deal with any additional deployment issues when you take a dependency on a micro-library. For example, you won't need to update any Wix files to ensure that the micro-library is deployed.
- Micro-libraries are independent and internal to each project that references them. You don't need to worry about version conflicts when you combine code that may use different versions of the library.
- The micro-libraries are licensed using the Apache License, version 2.0, with an additional exemption that permits you to distribute code that uses a micro-library without needing to provide a license notice or copyright attribution, or provide a copy of the license. You're free to do those things if you like, but you're not obliged to.

## License

**TL;DR -- Licensed under the Apache License, Version 2.0. Additionally, you're free to release it in binary form as part of your own software without the need to include the license text, the license notice or any copyright attribution notice.**

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this software except in compliance with the License and this notice. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the [License](License) for the specific language governing permissions and limitations under the License.

In addition, the copyright holders grant permission to reproduce and distribute copies of this software or derivative works thereof in any medium, with or without modifications, in Object form (as defined by the License), without satisfying the requirements of section 4a of the License. In practice, this means that you are free to include this library in binary releases of your own software without having to also include this notice, a copy of the Licence, or any other copyright attribution.

## Building the libraries

Prerequisites:

- PowerShell v5 or later
- Visual Studio 2017, or the equivalent build tools
 
To build the libraries, invoke the `build.ps1` PowerShell script. This will compile and test all of the libraries, and then generate a NuGet package for each micro-library in a `Dist` folder.

## Developing the libraries

All of the libraries can be opened simultaneously in Visual Studio 2017 using the `Source\Redgate.MicroLibraries.sln` solution. In addition, each individual micro-library can be opened in isolation using its corresponding `.csproj` file, since there are no dependencies between micro-libraries.

You can create a new micro-library by running the `new-project.ps1` script in PowerShell. This will prompt you for the name of a new micro-library, say `ULibs.MyNewLibrary`, and ask which existing template to base the new library on (you can accept the default `ULibs.UlibsProjectTemplate`). It will then generate a new project for your new micro-library and add it to the solution.       