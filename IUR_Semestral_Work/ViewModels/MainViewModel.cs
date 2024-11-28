using IUR_Semestral_Work.Support;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using LiveCharts.Defaults;
using System.Net.Http;
using IPGeolocation;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Threading;
using IUR_Semestral_Work.Data;
using Microsoft.EntityFrameworkCore;

namespace IUR_Semestral_Work.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ProfileViewModel _profileViewModel;
        private ObservableCollection<PushpinViewModel> mapElements = new ObservableCollection<PushpinViewModel>();
        public ObservableCollection<TrashTypeInfoViewModel> TrashTypesDisposalInfo { get; set; }
        public ObservableCollection<string> TrashTypes { get; } = new ObservableCollection<string>() { "KOV", "PLAST", "PNEU", "ELEKTRO", "SMĚS", "SKLO", "PAPÍR" };
        private RelayCommand _removeTrashCommand;
        private RelayCommand _addTrashCommand;
        private RelayCommand _registerUserCommand;
        private RelayCommand _loginUserCommand;
        private RelayCommand _logOffUserCommand;
        private List<ProfileViewModel> profiles;

        private bool _statsType;
        private bool _isMouseDown;
        private bool _isAddedTrashFiltered;
        private bool _isRemovedTrashFiltered;
        private bool _isLabelVisible; // label stating that trash was added\picked up
        private bool _isRandomGPSON; // just debug because prototype
        private bool _isLoggedIn;
        private bool _isDebug = true;

        private string _usernameTB;
        private string _passwordTB;
        private string _statusMessage;
        private string _selectedTrashType;
        private string _loginValidationText;
        private string _registerValidationText;

        IPGeolocationAPI ipGeoAPI = new IPGeolocationAPI("2c80b82942dc49fa9dd446a65577b58f");

        private ObservableValue _pickedTrashCount;
        private ObservableValue _notPickedTrashCount;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public bool IsLabelVisible
        {
            get { return _isLabelVisible; }
            set
            {
                if (_isLabelVisible != value)
                {
                    _isLabelVisible = value;
                    OnPropertyChanged(nameof(IsLabelVisible));
                }
            }
        }
        public string LoginValidationText
        {
            get => _loginValidationText;
            set => SetProperty(ref _loginValidationText, value);
        }

        public string RegisterValidationText
        {
            get => _registerValidationText;
            set => SetProperty(ref _registerValidationText, value);
        }

        public string UsernameTB
        {
            get => _usernameTB;
            set => SetProperty(ref _usernameTB, value);
        }
        public string PasswordTB
        {
            get => _passwordTB;
            set => SetProperty(ref _passwordTB, value);
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        public bool StatsType
        {
            get => _statsType;
            set => SetProperty(ref _statsType, value);
        }

        public bool IsRandomGPSON
        {
            get => _isRandomGPSON;
            set => SetProperty(ref _isRandomGPSON, value);
        }

        public bool IsAddedTrashFiltered
        {
            get => _isAddedTrashFiltered;
            set => SetProperty(ref _isAddedTrashFiltered, value);
        }

        public bool IsRemovedTrashFiltered
        {
            get => _isRemovedTrashFiltered;
            set => SetProperty(ref _isRemovedTrashFiltered, value);
        }

        public bool IsMouseDown
        {
            get => _isMouseDown;
            set => SetProperty(ref _isMouseDown, value);
        }

        public string SelectedTrashType
        {
            get { return _selectedTrashType; }
            set
            {
                if (_selectedTrashType != value)
                {
                    _selectedTrashType = value;
                    OnPropertyChanged(nameof(SelectedTrashType));
                }
            }
        }

        // All these need to be optimized but idk how :D maybe using sql
        public ObservableValue PickedTrashCount
        {
            get { return new ObservableValue(MapElements.Count(item => item.Picked)); }
            set => SetProperty(ref _pickedTrashCount, value);
        }

        public ObservableValue NotPickedTrashCount
        {
            get { return new ObservableValue(MapElements.Count(item => !item.Picked)); }
            set => SetProperty(ref _notPickedTrashCount, value);
        }

        private ObservableValue GetUserPickedTrashCount()
        {
            if (IsLoggedIn && ProfileViewModel != null)
                return new ObservableValue(MapElements.Count(item => item.Picked && item.UserID == ProfileViewModel.UID));
            else return new ObservableValue(0);
        }

        private ObservableValue GetUserNotPickedTrashCount()
        {
            if (IsLoggedIn && ProfileViewModel != null)
                return new ObservableValue(MapElements.Count(item => !item.Picked && item.UserID == ProfileViewModel.UID));
            else return new ObservableValue(0);
        }

        // Pushpins on map
        public ObservableCollection<PushpinViewModel> MapElements
        {
            get { return mapElements; }
            set => SetProperty(ref mapElements, value);
        }

        private void UpdateTrashStats()
        {
            // OVERALL
            OverallTrashStats[0].Values[0] = NotPickedTrashCount;
            OverallTrashStats[1].Values[0] = PickedTrashCount;

            // PERSONAL
            ObservableValue userNotPickedTrashCount = GetUserNotPickedTrashCount();
            ObservableValue userPickedTrashCount = GetUserPickedTrashCount();
            PersonalTrashStats[0].Values[0] = new ObservableValue(NotPickedTrashCount.Value - userNotPickedTrashCount.Value);
            PersonalTrashStats[1].Values[0] = new ObservableValue(PickedTrashCount.Value - userPickedTrashCount.Value);
            PersonalTrashStats[2].Values[0] = userNotPickedTrashCount;
            PersonalTrashStats[3].Values[0] = userPickedTrashCount;

            // TRASH TYPES
            for (int i = 0; i < OverallTrashTypeStats.Count; i++)
            {
                OverallTrashTypeStats[i].Values[0] = GetTrashCountForType(OverallTrashTypeStats[i].Title);
                PersonalTrashTypeStats[i].Values[0] = GetUserTrashCountForType(PersonalTrashTypeStats[i].Title);
            }

        }

        // Label under main button
        private void ShowLabel(string message, int length = 10)
        {
            StatusMessage = message;
            IsLabelVisible = true;

            // Set up a timer to hide the label after length seconds
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(length) };
            timer.Tick += (sender, args) =>
            {
                IsLabelVisible = false;
                timer.Stop();
            };
            timer.Start();
        }

        // useless since I save new ones and delete from database, but imma keep it here just in case 
        private void SaveData()
        {
            using (var context = new PushpinContext())
            {
                context.Database.EnsureCreated();
                foreach (var pushpin in mapElements)
                {
                    context.PushpinData.Add(new PushpinData
                    {
                        Latitude = pushpin.Location.Latitude,
                        Longitude = pushpin.Location.Longitude,
                        PinType = pushpin.PinType,
                        Picked = pushpin.Picked,
                        UserID = pushpin.UserID,
                        DateAdded = pushpin.DateAdded,
                        DateRemoved = pushpin.DateRemoved
                    });
                }

                context.SaveChanges();
            }
        }

        // Load pushpins from database
        private void LoadData()
        {
            using (var context = new PushpinContext())
            {
                context.Database.EnsureCreated();
                var pushpinDataList = context.PushpinData.ToList();

                foreach (var pushpinData in pushpinDataList)
                {
                    var pushpinViewModel = new PushpinViewModel(this,
                        new Location(pushpinData.Latitude, pushpinData.Longitude),
                        pushpinData.PinType, pushpinData.Picked, pushpinData.UserID);

                    pushpinViewModel.DatabaseId = pushpinData.Id; // Assign the database Id

                    pushpinViewModel.DateAdded = pushpinData.DateAdded;
                    pushpinViewModel.DateRemoved = pushpinData.DateRemoved;

                    mapElements.Add(pushpinViewModel);
                }
            }
        }

        private void AddPushpinToDatabase(PushpinViewModel pushpin)
        {
            using (var context = new PushpinContext())
            {
                context.Database.EnsureCreated();
                var pushpinData = new PushpinData
                {
                    Latitude = pushpin.Location.Latitude,
                    Longitude = pushpin.Location.Longitude,
                    PinType = pushpin.PinType,
                    Picked = pushpin.Picked,
                    UserID = pushpin.UserID,
                    DateAdded = pushpin.DateAdded,
                    DateRemoved = pushpin.DateRemoved
                };

                context.PushpinData.Add(pushpinData);
                context.SaveChanges();

                // Assign the generated database Id back to the PushpinViewModel
                // So I can remove from database if needed
                pushpin.DatabaseId = pushpinData.Id;
            }
        }

        public void RemovePushpinFromDatabase(PushpinViewModel pushpin)
        {
            int pushpinIdToRemove = pushpin.DatabaseId;

            using (var context = new PushpinContext())
            {
                context.Database.EnsureCreated();
                
                // Find the PushpinData with the specified Id
                var pushpinDataToRemove = context.PushpinData.Find(pushpinIdToRemove);

                if (pushpinDataToRemove != null)
                {
                    // Remove the found PushpinData from the DbSet
                    context.PushpinData.Remove(pushpinDataToRemove);
                    context.SaveChanges();
                }
                else
                {
                    // Pushpin not in database
                }
            }
        }

        public void AddPushpinToTheMap(Location location, string pinType, bool picked)
        {
            Guid id = IsLoggedIn && ProfileViewModel != null ? ProfileViewModel.UID : Guid.NewGuid();
            PushpinViewModel pushpin = new PushpinViewModel(this, location, pinType, picked, id);
            MapElements.Add(pushpin);
            AddPushpinToDatabase(pushpin);
        }

        public PushpinViewModel FindClosestPushpin(Location targetLocation)
        {
            PushpinViewModel closestPushpin = null;
            double treshold = 15; // Maximum distance of the trash near user in meters, phone GPS is pretty accurate these days (not everywhere ofc)
            double minDistance = double.MaxValue;

            foreach (var pushpin in mapElements)
            {
                double distance = CalculateDistance(targetLocation, pushpin.Location);

                if (pushpin.PinType == SelectedTrashType && distance < treshold && distance < minDistance)
                {
                    minDistance = distance;
                    closestPushpin = pushpin;
                }
            }

            return closestPushpin;
        }

        private double CalculateDistance(Location location1, Location location2)
        {
            // Using Haversine formula to calculate the distance between two locations
            // This function assumes the Earth is a perfect sphere, so it may not be extremely accurate for all cases
            // https://en.wikipedia.org/wiki/Haversine_formula
            double R = 6371; // Earth radius in kilometers
            double dLat = ToRadians(location2.Latitude - location1.Latitude);
            double dLon = ToRadians(location2.Longitude - location1.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(location1.Latitude)) * Math.Cos(ToRadians(location2.Latitude)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            // Convert distance to meters
            return distance * 1000;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public RelayCommand RemoveTrashCommand
        {
            get { return _removeTrashCommand ?? (_removeTrashCommand = new RelayCommand(RemoveTrash, RemoveTrashCanExecute)); }
        }

        private void RemoveTrash(object obj)
        {
            IsMouseDown = false;
            if (MapElements.Count > 0 && !_isDebug) {
                // TODO: change this so that it removes the closest trash
                // ignore this
                PushpinViewModel pushpin = MapElements.ElementAt(0);
                AddPushpinToTheMap(pushpin.Location, pushpin.PinType, true);
                RemovePushpinFromDatabase(pushpin);
                MapElements.RemoveAt(0);
            }
            else if (_isDebug)
            {
                // Since real GPS of the device isn't working in WPF as sated before, I'm using this
                // Basically this allows the user to not worry if the trash has been added
                // to the database as unpicked before.
                // It automatically checks if there is trash of same type near user and changes it to picked
                // If there is no trash of same type as unpicked near user, it creates a new pushpin, already as picked
                // Therefore there is no need to first add the trash and then immediately click to pick it up

                // very low probability of this being close to some trash of same type
                Location randomLocation = GetRandomLocation(); 
                // try this instead or by using any other unpicked trash location from database
                // Location randomLocation = new Location(37.8115773262087, 37.9258516112469);
                PushpinViewModel closestPin = FindClosestPushpin(randomLocation);
                if (closestPin != null)
                {
                    // There is trash of same type near the user, changing the pushpin in database from unpicked to picked
                    AddPushpinToTheMap(closestPin.Location, SelectedTrashType, true);
                    closestPin.RemovePushpin(closestPin);
                }
                else
                {
                    // There is no trash of same type near the user, adding it as new picked trash
                    AddPushpinToTheMap(randomLocation, SelectedTrashType, true);
                }
            }
            else
            {
                // TODO: change random location to current gps location 
                AddPushpinToTheMap(GetRandomLocation(), SelectedTrashType, true);
            }
            UpdateTrashStats();
            ShowLabel(string.Format("Picked up trash of type ({0}), thanks for making the Earth cleaner! :)", SelectedTrashType));
        }

        private bool RemoveTrashCanExecute(object obj)
        {
            return true;
        }

        public RelayCommand AddTrashCommand
        {
            get { return _addTrashCommand ?? (_addTrashCommand = new RelayCommand(AddTrash, AddTrashCanExecute)); }
        }

        private void AddTrash(object obj)
        {
            IsMouseDown = false;
            Location location = IsRandomGPSON ? GetRandomLocation() : GetLocationFromIP();
            AddPushpinToTheMap(location, SelectedTrashType, false);
            UpdateTrashStats();
            ShowLabel(string.Format("Added new trash of type ({0}), try to find some time to come pick it up in the future! :)", SelectedTrashType));
        }

        private bool AddTrashCanExecute(object obj)
        {
            return true;
        }

        public RelayCommand RegisterUserCommand
        {
            get { return _registerUserCommand ?? (_registerUserCommand = new RelayCommand(RegisterUser, RegisterUserCanExecute)); }
        }

        private void RegisterUser(object obj)
        {
            IsLoggedIn = true;
            
            ProfileViewModel = new ProfileViewModel(_usernameTB, _passwordTB);
            profiles.Add(ProfileViewModel);

            // Save the updated list of profiles
            ProfileManager.WriteToFile(profiles);
            
            UpdateTrashStats();
        }

        private bool RegisterUserCanExecute(object obj)
        {
            if (!ProfileManager.UserExists(_usernameTB))
            {
                RegisterValidationText = "";
                return true;
            }
            else
            {
                RegisterValidationText = "Username is already taken!";
                return false;
            }
        }

        public RelayCommand LoginUserCommand
        {
            get { return _loginUserCommand ?? (_loginUserCommand = new RelayCommand(LoginUser, LoginUserCanExecute)); }
        }

        private void LoginUser(object obj)
        {
            IsLoggedIn = true;
            ProfileViewModel = ProfileManager.GetMatchingUser(_usernameTB, _passwordTB);
            UpdateTrashStats();
        }

        private bool LoginUserCanExecute(object obj)
        {
            if(ProfileManager.UserExists(_usernameTB, _passwordTB))
            {
                LoginValidationText = "";
                return true;
            }
            else
            {
                LoginValidationText = "Username or password is not correct!";
                return false;
            }
        }

        public RelayCommand LogOffUserCommand
        {
            get { return _logOffUserCommand ?? (_logOffUserCommand = new RelayCommand(LogOffUser, LogOffUserCanExecute)); }
        }

        private void LogOffUser(object obj)
        {
            IsLoggedIn = false;
            UpdateTrashStats();
            // ProfileViewModel = null;
        }

        private bool LogOffUserCanExecute(object obj)
        {
            return true;
        }

        public ProfileViewModel ProfileViewModel
        {
            get { return _profileViewModel; }
            set => SetProperty(ref _profileViewModel, value);
        }
        /*
            Since the GPS from IP is 'always' the same, I have this to generate random locations
         */
        private Location GetRandomLocation()
        {
            Random random = new Random();

            double minValueLat = -90.0; // South pole
            double maxValueLat = 90.0;  // North pole

            double minValueLong = -180.0;
            double maxValueLong = 180.0;

            double randomLatitude = minValueLat + (random.NextDouble() * (maxValueLat - minValueLat));
            double randomLongitude = minValueLong + (random.NextDouble() * (maxValueLong - minValueLong));

            return new Location(randomLatitude, randomLongitude);
        }

        private string GetIPv4()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            return a3[0];
        }

        private string GetIPv6()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    foreach (UnicastIPAddressInformation ipAddress in ipProperties.UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            // Check if it's not a link-local or loopback address
                            if (!ipAddress.Address.IsIPv6LinkLocal && !IPAddress.IsLoopback(ipAddress.Address))
                            {
                                // The router address is often the first non-link-local IPv6 address
                                string routerIPv6Address = ipAddress.Address.ToString();
                                return routerIPv6Address;
                            }
                        }
                    }
                }
            }
            // if device is using ipv4
            return GetIPv4();
        }

        /*
         Note: I have tried getting precise GPS location of the device, but since System.Device.Location, Windows.Devices.Geolocation or even
         the Microsoft.Windows.SDK.Contracts NuGet aren't working, I didn't find any other way in WPF to get GPS location other than using the IP
         address of the device. Just for the sake of proving the prototype I used this solution, but if I made this in MAUI, I would make it work properly.
         */
        private Location GetLocationFromIP()
        {
            GeolocationParams geoParams = new GeolocationParams();
            geoParams.SetIPAddress(GetIPv6());
            geoParams.SetFields("geo,time_zone,currency");
            IPGeolocation.Geolocation geolocation = ipGeoAPI.GetGeolocation(geoParams);

            if (geolocation.GetStatus() == 200)
            {
                return new Location(double.Parse(geolocation.GetLatitude(), CultureInfo.InvariantCulture), double.Parse(geolocation.GetLongitude(), CultureInfo.InvariantCulture));
            }
            else
            {
                return GetRandomLocation();
            }
        }

        private ObservableValue GetTrashCountForType(string trashType)
        {
            int count = mapElements.Count(pushpin => pushpin.PinType == trashType);
            return new ObservableValue(count);
        }

        private ObservableValue GetUserTrashCountForType(string trashType)
        {
            if(IsLoggedIn && _profileViewModel != null)
            {
                int count = mapElements.Count(pushpin => pushpin.PinType == trashType && pushpin.UserID == _profileViewModel.UID);
                return new ObservableValue(count);
            }
            else return new ObservableValue(0);
        }

        public SeriesCollection OverallTrashStats { get; set; }
        public SeriesCollection PersonalTrashStats { get; set; }
        public SeriesCollection OverallTrashTypeStats { get; set; }
        public SeriesCollection PersonalTrashTypeStats { get; set; }

        public MainViewModel()
        {
            LoadData();

            profiles = ProfileManager.ReadFromFile();

            IsAddedTrashFiltered = true;
            ShowLabel("Press & Hold -> Choose Type & Release", 20);

            OverallTrashStats = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Unpicked",
                    Values = new ChartValues<ObservableValue> { NotPickedTrashCount },
                    Fill = Brushes.MediumSpringGreen,
                    Foreground = Brushes.Black,
                    FontSize = 20,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Picked Up",
                    Values = new ChartValues<ObservableValue> { PickedTrashCount },
                    Fill = Brushes.IndianRed,
                    FontSize = 20,
                    DataLabels = true
                }
            };

            PersonalTrashStats = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Other's Unpicked",
                    Values = new ChartValues<ObservableValue> { NotPickedTrashCount },
                    Fill = Brushes.MediumSpringGreen,
                    Foreground = Brushes.Black,
                    FontSize = 20,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Other's Picked Up",
                    Values = new ChartValues<ObservableValue> { PickedTrashCount },
                    Fill = Brushes.IndianRed,
                    FontSize = 20,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "User Unpicked",
                    Values = new ChartValues<ObservableValue> { GetUserNotPickedTrashCount() },
                    Fill = Brushes.Green,
                    FontSize = 20,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "User Picked Up",
                    Values = new ChartValues<ObservableValue> { GetUserPickedTrashCount() },
                    Fill = Brushes.DarkRed,
                    FontSize = 20,
                    DataLabels = true
                }
            };

            OverallTrashTypeStats = new SeriesCollection();

            for (int i = 0; i < TrashTypes.Count; i++)
            {
                string trashType = TrashTypes[i];
                string color = PushpinViewModel.GetColorForTrashType(trashType);

                OverallTrashTypeStats.Add(new PieSeries
                {
                    Title = trashType,
                    Values = new ChartValues<ObservableValue> { GetTrashCountForType(trashType) },
                    Fill = (Brush)new BrushConverter().ConvertFromString(color),
                    Foreground = (color == "Yellow" || color == "White" || color == "Pink") ? Brushes.Black : Brushes.White,
                    FontSize = 20,
                    DataLabels = true
                });
            }

            PersonalTrashTypeStats = new SeriesCollection();

            for (int i = 0; i < TrashTypes.Count; i++)
            {
                string trashType = TrashTypes[i];
                string color = PushpinViewModel.GetColorForTrashType(trashType);

                PersonalTrashTypeStats.Add(new PieSeries
                {
                    Title = trashType,
                    Values = new ChartValues<ObservableValue> { GetUserTrashCountForType(trashType) },
                    Fill = (Brush)new BrushConverter().ConvertFromString(color),
                    Foreground = (color == "Yellow" || color == "White" || color == "Pink") ? Brushes.Black : Brushes.White,
                    FontSize = 20,
                    DataLabels = true
                });
            }

            UpdateTrashStats();

            // I used these links
            // https://www.ekokom.cz/cz/ostatni/pro-verejnost/kratce-o-trideni/
            // https://www.samosebou.cz/2018/02/28/kam-proc-vyhodit-pneumatiky/
            TrashTypesDisposalInfo = new ObservableCollection<TrashTypeInfoViewModel>
            {
                new TrashTypeInfoViewModel { Header = "Informace jak třídit odpad", Description=""},
                new TrashTypeInfoViewModel { Header = "Papír", Description = "Modrý kontejner na papír\r\n\r\nANO\r\nHodit sem můžeme například časopisy, noviny, sešity, krabice, papírové obaly , cokoliv z lepenky, nebo knihy. Obálky s fóliovými okýnky sem můžete také vhazovat, zpracovatelé si s tím umí poradit. Bublinkové obálky vhazujeme pouze bez plastového vnitřku! Nevadí ani papír s kancelářskými sponkami. Ty se během zpracování samy oddělí.\r\n\r\nNE\r\nDo modrého kontejneru nepatří uhlový, mastný, promáčený nebo jakkoliv znečištěný papír. Tyto materiály nelze už nadále recyklovat. Pozor, použité dětské pleny opravdu nepatří do kontejneru na papír, ale do popelnice\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "Plast", Description = "Žlutý kontejner na plasty\r\n\r\nANO\r\nDo kontejnerů na plasty patří fólie, sáčky, plastové tašky, sešlápnuté PET láhve, obaly od pracích, čistících a kosmetických přípravků, kelímky od jogurtů, mléčných výrobků, balící fólie od spotřebního zboží, obaly od CD disků a další výrobky z plastů.Pěnový polystyren sem vhazujeme v menších kusech.\r\n\r\nNE\r\nNaopak sem nepatří mastné obaly se zbytky potravin nebo čistících přípravků, obaly od žíravin, barev a jiných nebezpečných látek, podlahové krytiny či novodurové trubky\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "Sklo", Description = "Recyklační značky skla dělíme na tři základní označení pro „čisté“ sklo a pak čtyři značky pro tzv. kompozitní materiály. Tedy pro takové výrobky a obaly, které se skládají z více druhů materiálů. Jedná se často o kombinace papíru, skla a plastu s lepenkou a kovy nebo mezi sebou navzájem.\r\n\r\nRecyklační značky: \n\rGL / 70 – čiré sklo: v případě rozdělení třídicích kontejnerů na sklo na zelenou a bílou část třídíme čiré sklo do vhozu třídicí nádoby bílé barvy;\r\nGL / 71 – zelené sklo: sem patří sklo obarvené do zelena. Klasickým příkladem bývají zelené láhve třeba od vína, piva, sektů. Tyto skleněné výrobky patří do zelené části kontejneru určeného pro třídění skla;\r\nGL / 72 – hnědé sklo: patří, stejně jako to zelené, do zelené popelnice.\r\n\r\nVedle klasických celoskleněných výrobků se setkáme i s jinými recyklačními značkami, které mají jinou podobu, přestože obsahují skleněný materiál.\r\n\r\nČasto si s nimi nevíme rady, jelikož naše intuice nám naznačuje, že do bílého ani do zeleného kontejneru na třídění skla nepatří. A naše intuice se nemýlí.\r\n\r\nNíže popsané identifikační kódy označují kompozitní, tedy vícesložkové kombinované materiály, které nesplňují podmínky pro recyklaci. Všechny výrobky a obaly označené těmito recyklačními značkami nepatří do nádob na tříděné sklo.\r\n\r\nC / 95: sklo + plast;\r\nC / 96: sklo + hliník;\r\nC / 97: sklo + ocelový pocínovaný plech;\r\nC / 98: sklo + různé kovy.\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "Metals", Description = "Kontejner označený šedou nálepkou na kovy\r\n\r\nANO\r\nDo kontejnerů na kovy patří drobnější kovový odpad, který lze skrz otvor bez problémů prostrčit – typicky plechovky od nápojů a konzerv, kovové tuby, alobal, kovové zátky, víčka, krabičky, hřebíky, šroubky, kancelářské sponky a další drobné kovové odpady.\r\nNa sběrné dvory lze kromě těchto menších odpadů odvážet i další kovové odpady – trubky, roury, plechy, hrnce, vany, kola a další objemnější předměty. Samostatnou kapitolou jsou kovové elektrospotřebiče, které lze na sběrných dvorech odkládat pouze kompletní.\r\n\r\nNE\r\nDo kontejnerů určených pro sběr kovů na ulici nepatří plechovky od barev, tlakové nádobky se zbytky nebezpečných látek, ani domácí spotřebiče a jiná vysloužilá zařízení složená z více materiálů. Tyto druhy odpadů se třídí na sběrných dvorech samostatně. Nepatří do nich ani těžké nebo toxické kovy, jakou jsou olovo či rtuť. Samostatnou kapitolu pak tvoří autovraky, které převezmou a doklad o ekologické likvidaci vystaví na vrakovištích.\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "Elektroodpad", Description = "Domácí bílá technika – lednice, pračky, myčky;\r\nmalé přístroje – mobily, fény, hračky;\r\nžehličky, varné konvice, kávovary;\r\npřístroje pro zahradu a hobby – sekačky na trávu, pily, ruční nářadí;\r\nzářivky a výbojky;\r\nbaterie.\r\n\r\nStaré vysloužilé spotřebiče můžete odevzdat k recyklaci několika způsoby. Aby lidé neměli s vysloužilými spotřebiči zbytečné starosti, zřizují kolektivní systémy svá sběrná místa ve sběrných dvorech. Když tam lidé vyvážejí nejrůznější odpady, mohou s sebou přibrat i starou ledničku nebo televizi.\r\n\r\nV poslední době se zavádí také sběr do speciálních červených kontejnerů. Ty jsou umisťovány přímo ve sběrných hnízdech s ostatními barevnými kontejnery nebo tam, kde chodí hodně lidí (například u parkovišť před nákupními centry), a jsou vyváženy podle potřeby, tedy až se naplní.\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "Pneumatiky", Description = "Místy zpětného odběru pneumatik jsou autoservisy a místa prodeje pneumatik. Ne sběrné dvory. Použité pneumatiky je možné odevzdávat zdarma a bez ohledu na značku výrobce v pneuservisech a autoservisech. Na pneumatiky se vztahuje zpětný odběr.\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "Směsný odpad", Description = "Tento odpad patří do jakékoliv běžné popelnice na směsný odpad.\r\n\r\n" },
                new TrashTypeInfoViewModel { Header = "General Tips", Description = "- Znáte takové ty tři šipky na obalech, co se v podstatě neustále dokola nahánějí? Jistěže ano! Jedná se totiž o recyklační symbol, který značí, že tento obal je určen k recyklaci.\r\n\r\n" },
                // Add more trash disposal information as needed
            };
        }
    }
}
