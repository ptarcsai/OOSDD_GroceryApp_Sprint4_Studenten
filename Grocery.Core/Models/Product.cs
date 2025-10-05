using CommunityToolkit.Mvvm.ComponentModel;

namespace Grocery.Core.Models
{
    public partial class Product : Model
    {
        [ObservableProperty]
        private int _stock;  // generates public int Stock { get; set; } with notifications

        public Product(int id, string name, int stock) : base(id, name)
        {
            _stock = stock;
        }
    }
}
