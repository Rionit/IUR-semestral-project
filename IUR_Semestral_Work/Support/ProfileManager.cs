using IUR_Semestral_Work.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IUR_Semestral_Work.Support
{
    class ProfileManager
    {
        private const string FileName = "profiles.csv";

        public static void WriteToFile(List<ProfileViewModel> profiles)
        {
            File.WriteAllLines(FileName, profiles.Select(profile => profile.ToCsvString()));
        }

        public static List<ProfileViewModel> ReadFromFile()
        {
            List<ProfileViewModel> profiles = new List<ProfileViewModel>();
            if (File.Exists(FileName))
            {
                profiles = File.ReadAllLines(FileName)
                    .Select(line => ProfileViewModel.FromCsvString(line))
                    .Where(profile => profile != null)
                    .ToList();
            }
            return profiles;
        }

        public static bool UserExists(string username)
        {
            if (File.Exists(FileName))
            {
                return File.ReadAllLines(FileName)
                    .Select(line => ProfileViewModel.FromCsvString(line))
                    .Any(profile => profile != null && profile.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            return false; // File doesn't exist or no matching user found
        }

        public static bool UserExists(string username, string password)
        {
            if (File.Exists(FileName))
            {
                return File.ReadAllLines(FileName)
                    .Select(line => ProfileViewModel.FromCsvString(line))
                    .Any(profile =>
                        profile != null &&
                        profile.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                        profile.Password.Equals(password)
                    );
            }

            return false; // File doesn't exist or no matching user found
        }

        public static ProfileViewModel GetMatchingUser(string username, string password)
        {
            if (File.Exists(FileName))
            {
                return File.ReadAllLines(FileName)
                    .Select(line => ProfileViewModel.FromCsvString(line))
                    .FirstOrDefault(profile =>
                        profile != null &&
                        profile.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                        profile.Password.Equals(password)
                    );
            }

            return null; // File doesn't exist or no matching user found
        }
    }
}
