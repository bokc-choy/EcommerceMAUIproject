using Amazon.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.Library.Utilities;
using eCommerce.Library.DTO;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Amazon.Library.Services
{
    public class InventoryServiceProxy
    {
        private static InventoryServiceProxy? instance;
        private static object instanceLock = new object();

        private List<ProductDTO> products;

        public ReadOnlyCollection<ProductDTO> Products
        {
            get
            {
                return products.AsReadOnly();
            }
        }


        public async Task<ProductDTO> AddOrUpdate(ProductDTO p)
        {
            var result = await new WebRequestHandler().Post("/Inventory", p);
            return JsonConvert.DeserializeObject<ProductDTO>(result);
        }
        
        // NEW STUFF
        public async Task<ProductDTO> Delete(int id)
        {
            //var itemToDelete = Products.FirstOrDefault(p => p.Id == id);
            //if(itemToDelete == null)
            //{
            //    return null;
            //}
            //products.Remove(itemToDelete);

            var response = await new WebRequestHandler().Delete($"/{id}");
            var itemToDelete = JsonConvert.DeserializeObject<ProductDTO>(response);
            return itemToDelete;
        }
        //public void Delete(int id)
        //{
        //    if(products == null)
        //    {
        //        return;
        //    }
        //    var productToDelete = products.FirstOrDefault(p => p.Id == id);

        //    if(productToDelete != null)
        //    {
        //        products.Remove(productToDelete);
        //    }
        //}

        private InventoryServiceProxy()
        {

            // make a web call
            var response = new WebRequestHandler().Get("/Inventory").Result;
            products = JsonConvert.DeserializeObject<List<ProductDTO>>(response);
        }

        public async Task<IEnumerable<ProductDTO>> Get()
        {
            var result = await new WebRequestHandler().Get("/Inventory");
            var deserializedResult = JsonConvert.DeserializeObject<List<ProductDTO>>(result);
            products = deserializedResult?.ToList() ?? new List<ProductDTO>();
            return products;
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

        //
        public async Task MassImportFromCsv(string Path)
        {
            var newProducts = ReadProductsFromCsv(Path);

            foreach (var product in newProducts)
            {
                await AddOrUpdate(product);
            }
        }

        private List<ProductDTO> ReadProductsFromCsv(string Path)
        {
            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                return csv.GetRecords<ProductDTO>().ToList();
            }
        }
        //
    }
}
