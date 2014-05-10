using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarktplaatsZoeker
{
   public class ZoekOpdracht
    {
       public Uri Link { get; set; }
       public string Title { get; set; }
    }
   public class ZoekOpdrachten
   {
       private List<ZoekOpdracht> _Items = new List<ZoekOpdracht>();
       public List<ZoekOpdracht> Items
       {
           get
           {
               return this.Items;
           }
       }
   }
}
