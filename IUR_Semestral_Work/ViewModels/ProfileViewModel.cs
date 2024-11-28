using IUR_Semestral_Work.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IUR_Semestral_Work.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private Guid _uid;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public Guid UID
        {
            get => _uid;
        }

        // when creating a profile
        public ProfileViewModel(string username, string password)
        {
            _uid = Guid.NewGuid();
            Password = password;
            Username = username;
        }

        // when loading from a file
        public ProfileViewModel(string username, string password, Guid uid)
        {
            _uid = uid;
            Password = password;
            Username = username;
        }

        public string ToCsvString()
        {
            return $"{Username},{Password},{UID}";
        }

        // Create a ProfileViewModel instance from a CSV-formatted string
        public static ProfileViewModel FromCsvString(string csv)
        {
            string[] parts = csv.Split(',');
            if (parts.Length == 3 && Guid.TryParse(parts[2], out Guid uid))
            {
                return new ProfileViewModel(parts[0], parts[1], uid);
            }
            return null; // Invalid CSV format
        }

    }
}
