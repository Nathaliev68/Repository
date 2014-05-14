using ClassLibrary;
using MarktplaatsZoeker.Common;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace MarktplaatsZoeker.ViewModels
{
    public class ZoekOpdrachtViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand AddZoekOpdrachtToLocalSettingsCommand { get; set; }

        private MobileServiceClient MarktplaatsZoekerClient = new MobileServiceClient(
       "https://marktplaatszoeker.azure-mobile.net/", "BfnHFNOyKXeVwCONycpGGbybTjJBeq74");

        private string _title;
        private string _zoekterm;
        private ObservableCollection<Categorie> _myList;
        private Categorie _selectedValue;
        public string Name { get; set; }
        public string Id { get; set; }


        public ZoekOpdrachtViewModel()
        {
            AddZoekOpdrachtToLocalSettingsCommand = new RelayCommand(AddZoekOpdrachtToLocalSettings);
        }

        private async void AddZoekOpdrachtToLocalSettings(object itemText)
        {
            MobileServiceCollection<ZoekOpdracht, ZoekOpdracht> items;
            IMobileServiceTable<ZoekOpdracht> zoekOpdrachten = MarktplaatsZoekerClient.GetTable<ZoekOpdracht>();
            //items = await zoekOpdrachten.ToCollectionAsync();
            var zoekOpdracht = new ZoekOpdracht()
            {
                Link = string.Format("http://kopen.marktplaats.nl/opensearch.php?s=100&q={0}&g={1}", Zoekterm, MySelectedValue.Id),
                Title = Zoekterm
            };
            await zoekOpdrachten.InsertAsync(zoekOpdracht);
        }

        public Categorie MySelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                    }
                }
            }
        }


        public string Zoekterm
        {
            get { return _zoekterm; }
            set
            {
                if (_zoekterm != value)
                {
                    _zoekterm = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Zoekterm"));
                    }
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }

        public ObservableCollection<Categorie> CategorieList
        {
            get
            {
                if (_myList == null)
                {
                    _myList = new ObservableCollection<Categorie>();
                    XDocument xdoc = XDocument.Load("../AppX/Assets/categorie.xml");
                    IEnumerable<Categorie> categories = from cat in xdoc.Descendants("Categorie")
                                                        select new Categorie(cat.Attributes("ID").First().Value, cat.Attributes("Name").First().Value);
                    foreach (Categorie cat in categories)
                    {
                        _myList.Add(cat);
                    }
                }
                return _myList;
            }
            set
            {
                if (_myList != value)
                {
                    _myList = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("CategorieList"));
                    }
                }
            }
        }
    }
}
