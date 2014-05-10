using MarktplaatsZoeker.Common;
using MvvmLightApp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MarktplaatsZoeker.ViewModels
{
    public class StartPageViewModel : INotifyPropertyChanged
    {
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand ItemClickedCommand { get; set; }

        public RelayCommand StopNotificationsCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }

        private readonly INavigationService _navigationService = new NavigationService();

        public event PropertyChangedEventHandler PropertyChanged;
        private List<ZoekOpdracht> _myList;
        private string menuText;
        private string key;
       

        public StartPageViewModel()
        {
            SelectionChangedCommand = new RelayCommand(SelectionChanged);
            ItemClickedCommand = new RelayCommand(ItemClicked);
            DeleteCommand = new RelayCommand(Delete);
            StopNotificationsCommand = new RelayCommand(StopNotifications);
        }

        private void StopNotifications(object obj)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                var task = FindTask(cur.Value.Name);
                if (task != null)
                {
                    task.Unregister(true);
                }

            }
        }

        public static BackgroundTaskRegistration FindTask(string taskName)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    // 
                    // The task is registered.
                    // 

                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            return null;
        }



        private void ItemClicked(object obj)
        {
            ItemClickEventArgs e = (ItemClickEventArgs)obj;
            _navigationService.Navigate(typeof(ItemsPage), ((ZoekOpdracht)e.ClickedItem).Link);
        }

        private void SelectionChanged(object obj)
        {
            if (((SelectionChangedEventArgs)obj).AddedItems.Count() > 0)
            {
               key = ((ZoekOpdracht)((SelectionChangedEventArgs)obj).AddedItems[0]).Title;
            }
        }
        private async void Delete(object obj)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.Containers["zoekOpdrachten"];
            ((ApplicationDataContainerSettings)container.Values).Remove(key);

        }
        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        public List<ZoekOpdracht> ZoekOpdrachten
        {
            get
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                var container = localSettings.CreateContainer("ZoekOpdrachten", Windows.Storage.ApplicationDataCreateDisposition.Always);
                _myList = new List<ZoekOpdracht>();
                ZoekOpdracht zoek;

                if (container == null || container.Values.Count() == 0)
                {
                    // AddZoekOpdrachtenToLocalSettings();
                    return null;
                }
                else
                {
                    foreach (KeyValuePair<string, object> s in container.Values)
                    {
                        zoek = new ZoekOpdracht();
                        zoek.Link = new Uri(s.Value.ToString());
                        zoek.Title = s.Key;

                        _myList.Add(zoek);
                    };
                    return _myList;
                }
            }
            set
            {
                if (_myList != value)
                {
                    _myList = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ZoekOpdrachten"));
                    }
                }
            }
        }

        public string lastlogon 
        { 
            get
            {
                var appsettings = ApplicationData.Current.LocalSettings;

                if (appsettings.Values["lastlogon"] == null)
                {
                    appsettings.Values["lastlogon"] = DateTime.Now.ToBinary().ToString();
                }

                long loggedOn = long.Parse(appsettings.Values["lastlogon"].ToString());

                return DateTime.FromBinary(loggedOn).ToString();
            }
        }
    }
}
