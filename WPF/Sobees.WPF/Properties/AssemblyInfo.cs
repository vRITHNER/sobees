using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Sobees")] 
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Sobees")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

// In order to begin building localizable applications, set 
// <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
// inside a <PropertyGroup>.  For example, if you are using US english
// in your source files, set the <UICulture> to en-US.  Then uncomment
// the NeutralResourceLanguage attribute below.  Update the "en-US" in
// the line below to match the UICulture setting in the project file.
[assembly: NeutralResourcesLanguage("en-US",
    UltimateResourceFallbackLocation.MainAssembly)]

// ResourceDictionaryLocation.None, where theme specific resource dictionaries are located
// (used if a resource is not found in the page, 
// or application resource dictionaries)
// ResourceDictionaryLocation.SourceAssembly where the generic resource dictionary is located
// (used if a resource is not found in the page, 
// app, or any theme specific resource dictionaries)
[assembly: ThemeInfo(ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]

[assembly: AssemblyVersion("0.8.9.2")]
[assembly: AssemblyFileVersion("0.8.9.2")]

// Permission requests
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true, ControlEvidence = true, ControlThread = true, ControlPrincipal = true, RemotingConfiguration = true)]
[assembly: EnvironmentPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
[assembly: FileIOPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
[assembly: RegistryPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
[assembly: ReflectionPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
[assembly: PerformanceCounterPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
