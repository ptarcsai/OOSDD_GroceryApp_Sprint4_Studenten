﻿using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            var items = _groceriesRepository.GetAll() ?? new List<GroceryListItem>();

            var grouped = items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Total = g.Sum(x => x.Amount < 0 ? 0 : x.Amount) })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total)
                .ThenBy(x => x.ProductId)
                .Take(topX <= 0 ? 5 : topX)
                .ToList();

            var result = new List<BestSellingProducts>();
            int rank = 1;
            foreach (var g in grouped)
            {
                var product = _productRepository.Get(g.ProductId) ?? new Product(0, string.Empty, 0);
                result.Add(new BestSellingProducts(product.Id, product.Name, product.Stock, g.Total, rank));
                rank++;
            }
            return result;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
