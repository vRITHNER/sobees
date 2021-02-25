using System;
using System.Linq;
using WixSharp;

namespace SobeesMsiSetup
{
    class Program
    {
        static void Main()
        {
            //DON'T FORGET to execute "install-package wixsharp" in the package manager console

            var project = new Project("sobeesSetup",
                                      new Dir(@"%ProgramFiles%\sobees\sobeesDesktop",
                                              new Files(@"bin\debug\*.*"),
                                              new ExeFileShortcut("Uninstall sobeesDesktop", "[System64Folder]msiexec.exe", "/x [ProductCode]"))
                                     ) {UI = WUI.WixUI_InstallDir};

            project.ResolveWildCards();
            project.EmitConsistentPackageId = true;
            project.Version = new Version("0.8.9.2");

            var exeFile = project.AllFiles.Single(f => f.Name.ToLower().EndsWith("sobees.exe"));

            exeFile.Shortcuts = new[] {
                                    new FileShortcut("sobees.exe", "INSTALLDIR") {IconFile = @"bin\debug\sobees.ico"},
                                    new FileShortcut("sobees.exe", @"%Desktop%") {IconFile = @"bin\debug\sobees.ico"}
                                  };

            Compiler.PreserveTempFiles = true;
            Compiler.WixLocation = @"..\..\packages\WixSharp.wix.bin.3.10.3\tools\bin";

            Compiler.BuildMsi(project);
            //project.BuildMsi();
        }
    }
}