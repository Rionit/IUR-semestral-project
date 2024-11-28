using IUR_Semestral_Work.Support;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IUR_Semestral_Work.ViewModels
{
    public class PushpinViewModel : ViewModelBase
    {
        private RelayCommand _removePushpinCommand;
        private MainViewModel _mainViewModelReference;
        private Location _location;
        private string _pinColor;
        private string _pinType;
        private bool _picked;
        private DateTime _dateAdded;
        private DateTime _dateRemoved;
        private DateTime _dateLatest;
        private Guid _userID;
        private int _databaseId;

        public int DatabaseId
        {
            get => _databaseId;
            set => SetProperty(ref _databaseId, value);
        }

        public DateTime DateAdded
        {
            get => _dateAdded;
            set => SetProperty(ref _dateAdded, value);
        }
        
        public DateTime DateRemoved
        {
            get => _dateRemoved;
            set => SetProperty(ref _dateRemoved, value);
        }
        
        public DateTime DateLatest
        {
            get
            {
                if(_dateRemoved > _dateAdded) return _dateRemoved;
                else return _dateAdded;
            }
            set => SetProperty(ref _dateRemoved, value);
        }
        
        public Guid UserID
        {
            get => _userID;
            set => SetProperty(ref _userID, value);
        }

        public bool Picked
        {
            get => _picked;
            set
            {
                SetProperty(ref _picked, value);
                PinColor = Picked ? "IndianRed" : "MediumSpringGreen";
            }
        }

        public Location Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        // Red or Green based on if it is picked or not
        public string PinColor
        {
            get => _pinColor;
            set => SetProperty(ref _pinColor, value);
        }

        public string PinTypeColor
        {
            get
            {
                return GetColorForTrashType(_pinType);
            }
            set => SetProperty(ref _pinType, value);
        }

        // The border color signifying the trash type
        public static string GetColorForTrashType(string trashType)
        {
            switch (trashType)
            {
                case "KOV":
                    return "Gray";
                case "PLAST":
                    return "Yellow";
                case "PAPÍR":
                    return "Blue";
                case "ELEKTRO":
                    return "Purple";
                case "PNEU":
                    return "Black";
                case "SMĚS":
                    return "Pink";
                case "SKLO":
                    return "White";
                default:
                    return "Red";
            }
        }

        public string PinType
        {
            get => _pinType;
            set => SetProperty(ref _pinType, value);
        }

        public RelayCommand RemovePushpinCommand
        {
            get { return _removePushpinCommand ?? (_removePushpinCommand = new RelayCommand(RemovePushpin, RemovePushpinCanExecute)); }
        }

        public void RemovePushpin(object obj)
        {
            _mainViewModelReference.RemovePushpinFromDatabase(this);
            _mainViewModelReference.MapElements.Remove(this);
        }

        private bool RemovePushpinCanExecute(object obj)
        {
            return true;
        }

        public PushpinViewModel(MainViewModel mainViewModelReference, Location location, string type, bool picked, Guid userID)
        {
            _mainViewModelReference = mainViewModelReference;
            Location = location;
            PinType = type;
            Picked = picked;
            UserID = userID;
            DateAdded = DateTime.Now;
            if (picked)
            {
                DateRemoved = DateTime.Now;
            }
        }
    }
}
