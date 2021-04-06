using System;
using System.IO;
using System.Reflection;

namespace WebuyParser
{
    class FilesChecker
    {
        private string[] countries = { "#pl", "uk", "pt", "ie", "it", "es", "nl", "ic" };
        private string[] sellCountries = { "pl", "#uk", "#pt", "#ie", "#it", "#es", "#nl", "#ic" };
        private string[] platforms = { "PS2", "PS3", "PS4", "XBox360", "XBoxOne", "Switch", "Nintendo DS", "3DS", "Wii", "Wii U", "Vita" };

        public bool CheckFilesIntegrity()
        {
            CheckReports();
            if (!CheckSettings())
            {
                RestorePlatformsLists();
                RestoreCountriesLists();
                return false;
            }
            return true;
        }
        private DirectoryInfo[] GetDirectoryInfos()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            DirectoryInfo dI = new FileInfo(location.AbsolutePath).Directory;

            try
            {
                return dI.GetDirectories();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            //return dI.GetDirectories();
            return null;
        }

        private bool CheckReports()
        {
            var dIs = GetDirectoryInfos();

            bool created = false;

            foreach (var i in dIs)
            {
                if (i.Name == "report")
                    created = true;
            }

            if (!created)
            {
                var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
                DirectoryInfo dI = new FileInfo(location.AbsolutePath).Directory;
                Directory.CreateDirectory(dI.Parent.Name + "/reports");
            }

            return created;
        }
        private bool CheckSettings()
        {
            var dIs = GetDirectoryInfos();

            bool created = false;

            foreach (var i in dIs)
            {
                if (i.Name == "settings")
                    created = true;
            }

            if (!created)
            {
                var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
                DirectoryInfo dI = new FileInfo(location.AbsolutePath).Directory;
                Directory.CreateDirectory(dI.Parent.Name + "/settings");
            }
            return created;
        }

        private void RestoreCountriesLists()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            DirectoryInfo dI = new FileInfo(location.AbsolutePath).Directory;

            var path = dI.FullName + "/settings";

            //Directory.CreateDirectory(path);

            StreamWriter sw = File.CreateText(path + "/countries.txt");
            foreach (var country in countries)
            {
                sw.WriteLine(country);
            }
            sw.Close();

            sw = File.CreateText(path + "/sellCountry.txt");
            foreach (var country in sellCountries)
            {
                sw.WriteLine(country);
            }
            sw.Close();
        }

        private void RestorePlatformsLists()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            DirectoryInfo dI = new FileInfo(location.AbsolutePath).Directory;


            var path = dI.FullName + "/settings/";
            Directory.CreateDirectory(path);
            Console.WriteLine(path);

            StreamWriter sw = File.CreateText(path + "platforms.txt");
            foreach (var platform in platforms)
            {
                sw.WriteLine(platform);
            }
            sw.Close();
        }
    }
}


