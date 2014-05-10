using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
   public class Categorie
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Categorie(string id, string name)
        {
            Id = Int32.Parse(id);
            Name = name;
        }
   }

   public class Categories
   {
       private ObservableCollection<Categorie> _Items = new ObservableCollection<Categorie>();
       public ObservableCollection<Categorie> Items
       {
           get
           {
               return this._Items;
           }
       }
   }

}
   