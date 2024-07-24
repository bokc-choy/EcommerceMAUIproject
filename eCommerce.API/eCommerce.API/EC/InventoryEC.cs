using Amazon.Library.Models;
using eCommerce.API.Database;
using eCommerce.Library.DTO;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace eCommerce.API.EC
{
    public class InventoryEC
    {
        //private IEnumerable<Product> Products { get; set; }
        public InventoryEC()
        {

        }
        public async Task<IEnumerable<ProductDTO>> Get()
        {
            return Filebase.Current.Products.Take(100).Select(p => new ProductDTO(p));
        }

        public async Task<ProductDTO> AddOrUpdate(ProductDTO p)
        {
            /*bool isAdd = false;
            if (p.Id == 0)
            {
                isAdd = true;
                p.Id = FakeDatabase.NextProductId;
            }

            if (isAdd)
            {
                FakeDatabase.Products.Add(new Product(p));
            }
            else
            {
                var prodToUpdate = FakeDatabase.Products.FirstOrDefault(a => a.Id == p.Id);
                if (prodToUpdate != null)
                {
                    var index = FakeDatabase.Products.IndexOf(prodToUpdate);
                    FakeDatabase.Products.RemoveAt(index);
                    prodToUpdate = new Product(p);
                    FakeDatabase.Products.Insert(index, prodToUpdate);
                }

                //var targetProduct = FakeDatabase.Products.FirstOrDefault(product => product.Id == p.Id);
                //if (targetProduct != null)
                //{
                //    targetProduct.Name = p.Name;
                //    targetProduct.Quantity = p.Quantity;
                //    targetProduct.Price = p.Price;
                //}
            }*/

            return new ProductDTO(Filebase.Current.AddOrUpdate(new Product(p)));
        }

        public async Task<ProductDTO?> Delete(int id)
        {
            return new ProductDTO(Filebase.Current.Delete(id));
                
                /*.Products.FirstOrDefault(p => p.Id == id);
            if (itemToDelete == null)
            {
                return null;
            }
            FakeDatabase.Products.Remove(itemToDelete);
            return new ProductDTO(itemToDelete);*/
        }
    }
}
