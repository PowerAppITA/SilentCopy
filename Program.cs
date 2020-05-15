using System;
using System.IO;
using System.Xml.Serialization;
using SilentCopy.Model;

namespace SilentCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            Packages packages = null;
            string path_to_xml = Path.Combine(args);
            string[] path_to_bin = { args[0], args[1].Replace("\\packages.config", ""), "bin" };
            string destinationDir = Path.Combine(path_to_bin);
            bool exists = Directory.Exists(destinationDir);

            if (!exists)
                Directory.CreateDirectory(destinationDir);

            XmlSerializer serializer = new XmlSerializer(typeof(Packages));

            StreamReader reader = new StreamReader(path_to_xml);
            packages = (Packages)serializer.Deserialize(reader);

            foreach (Package package in packages.Package)
            {

                string[] path_to_lib = { args[0], "packages", package.Id + "." + package.Version, "lib"};
                string sourceDir = Path.Combine(path_to_lib);
                if (Directory.Exists(sourceDir))
                {

                    ProcessDirectory(sourceDir, destinationDir);

                    if (Directory.Exists(Path.Combine(sourceDir, package.TargetFramework)))
                    {
                        ProcessDirectory(Path.Combine(sourceDir, package.TargetFramework), destinationDir);
                    } else
                    {
                        if (package.TargetFramework.Contains("net4"))
                        {
                            if (Directory.Exists(Path.Combine(sourceDir, "net40")))
                            {
                                ProcessDirectory(Path.Combine(sourceDir, "net40"), destinationDir);
                            }
                        }
                    }
                }
            }
        }

        public static void ProcessDirectory(string sourceDir, string destinationDir)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(sourceDir, "*.dll");
            foreach (string file in fileEntries)
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(destinationDir, fileName), true);
            }

        }
    }
}
