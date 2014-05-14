using Newtonsoft.Json;
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
       public Guid Id { get; set; }

       [JsonProperty(PropertyName = "Link")]
       public string Link { get; set; }
         
       [JsonProperty(PropertyName = "Title")]
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
