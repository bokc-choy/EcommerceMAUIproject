﻿using Amazon.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Library.Services
{
    public class InventoryServiceProxy
    {
        private static InventoryServiceProxy? instance;
        private static object instanceLock = new object();

        private List<Product> products;

        public ReadOnlyCollection<Product> Products
        {
            get
            {
                return products.AsReadOnly();
            }
        }

        private int NextId
        {
            get
            {
                if(!products.Any())
                {
                    return 1;
                }

                return products.Select(p => p.Id).Max() + 1;
            }
        }

        public Product AddOrUpdate(Product p)
        {
            bool isAdd = false;
            if(p.Id == 0)
            {
                isAdd = true;
                p.Id = NextId;
            }

            if(isAdd)
            {
                products.Add(p);
            }
            else
            {
                var targetProduct = products.FirstOrDefault(product => product.Id == p.Id);
                if(targetProduct != null)
                {
                    targetProduct.Name = p.Name;
                    targetProduct.Quantity = p.Quantity;
                    targetProduct.Price = p.Price;
                }
            }

            return p;
        }
        
        // NEW STUFF
        public void Delete(int id)
        {
            if(products == null)
            {
                return;
            }
            var productToDelete = products.FirstOrDefault(p => p.Id == id);

            if(productToDelete != null)
            {
                products.Remove(productToDelete);
            }
        }

        private InventoryServiceProxy()
        {
            //TODO: remove sample data on checkin
            products = new List<Product>{
                new Product{Id = 1,Name = "Product 1", Price=1.75M, Quantity=1}
                , new Product{Id = 2,Name = "Product 2", Price=10M, Quantity=10}
                , new Product{Id = 3,Name = "Product 3", Price=137.11M, Quantity=20}
            };
        }

        public static InventoryServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InventoryServiceProxy();
                    }
                }
                return instance;
            }
        }
    }
}
