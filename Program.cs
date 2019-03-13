using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace MuteThoseBloodyOwls
{
    class Program
    {
        static int Main(string[] args)
        {
            string applicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string alreadySeenFilePath = Path.Combine(applicationDataFolder, @"plastic4\guihelp-alreadyseen.conf");
            string doNotShowAgainPath = Path.Combine(applicationDataFolder, @"plastic4\guihelp-dontshowagain.conf");

            string plasticInstallRegistryKeyName = @"SOFTWARE\Codice Software S.L.\Codice Software Plastic SCM";
            string plasticInstallRegistryValueName = "Location";

            string plasticInstallLocation = null;

            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey installRoot = localMachine.OpenSubKey(plasticInstallRegistryKeyName, false))
                {
                    if (installRoot == null)
                    {
                        Console.WriteLine("Unable to find PlasticSCM installation location when looking into registry");
                        return 1;
                    }

                    plasticInstallLocation = (string)installRoot.GetValue(plasticInstallRegistryValueName);

                    if (plasticInstallLocation == null)
                    {
                        Console.WriteLine("Unable to find PlasticSCM installation location when looking into registry");
                        return 1;
                    }
                }
            }

            string assemblyPath = Path.Combine(plasticInstallLocation, @"client\plasticgui.dll");

            Assembly plasticGuiAssembly = null;

            try
            {
                plasticGuiAssembly = Assembly.LoadFrom(assemblyPath);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(string.Format("Error: unable to load assembly {0}", assemblyPath));
                return 1;

            }

            string helpEntryTypeName = "PlasticGui.Help.HelpEntry";

            Type helpEntryType = null;
            try
            {
                helpEntryType = plasticGuiAssembly.GetType(helpEntryTypeName, true);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(string.Format("Error: unable to find type {0} in assembly {1}", helpEntryTypeName, assemblyPath));
                return 1;
            }

            string[] helpEntryNames = Enum.GetNames(helpEntryType);

            try
            {
                File.WriteAllLines(alreadySeenFilePath, helpEntryNames);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error writing to {0}", alreadySeenFilePath));
                return 1;
            }
            try
            {
                File.WriteAllLines(doNotShowAgainPath, helpEntryNames);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error writing to {0}", doNotShowAgainPath));
                return 1;
            }

            Console.WriteLine("All the bloody owls have been muted!");
            return 0;
        }
    }
}
