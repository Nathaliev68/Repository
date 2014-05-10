using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MarktplaatsZoeker
{
    public class MarktplaatsItem
    {
        public string Title { get; set; }
        public string Prijs { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime PubDate { get; set; }
        public Uri Image { get; set; }
        public Uri Link { get; set; }
        public bool Read { get; set; }

        public string Nieuw
        {
            get { if ( Read ) return string.Empty; else return "Nieuw";}           
        }

    }

    public class MarktplaatsItems
    {
        private ObservableCollection<MarktplaatsItem> _Items = new ObservableCollection<MarktplaatsItem>();
        public ObservableCollection<MarktplaatsItem> Items
        {
            get
            {
                return this._Items;
            }
        }
    }
}
    
