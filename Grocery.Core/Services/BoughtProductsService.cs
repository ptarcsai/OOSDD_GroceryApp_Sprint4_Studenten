
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            var result = new List<BoughtProducts>(); //Lijst die later aan de ViewModel wordt gegeven

            if (productId == null) return result; //Bij geen product niks tonen

            var product = _productRepository.Get(productId.Value); //Ophalen product en als niet bestaat dan ook niks 
            if (product == null) return result;

            var items = _groceryListItemsRepository //Lijst filteren op product en positief aantal
                .GetAll()
                .Where(i => i.ProductId == product.Id && i.Amount > 0);

            foreach (var listId in items //Unieke id van boodschappenlijst waar dit product in voorkomt
                .Select(i => i.GroceryListId)
                .Distinct())
            {

                var list = _groceryListRepository.Get(listId); //Boodschappenlijst ophalen
                if (list == null) continue;

                var client = _clientRepository.Get(list.ClientId); //Bijbehorende klant ophalen
                if (client == null) continue;

                result.Add(new BoughtProducts(client, list, product)); //Regel voor de CollectionView met client, list en product
            }

            return result;
        }
    }
}
