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

            string assemblyPath = @"c:\Program Files\PlasticSCM5\client\plasticgui.dll";
            string alreadySeenFilePath = Path.Combine(applicationDataFolder, @"plastic4\guihelp-alreadyseen.conf");
            string doNotShowAgainPath = Path.Combine(applicationDataFolder, @"plastic4\guihelp-dontshowagain.conf");

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
