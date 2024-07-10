using Amazon.Library.Models;
using Amazon.Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.MAUI.ViewModels
{
    public class ShopViewModel : INotifyPropertyChanged
    {

        public ShopViewModel()
        {
            InventoryQuery = string.Empty;
        }

        private string inventoryQuery;
        public string InventoryQuery
        {
            set
            {
                inventoryQuery = value;
                NotifyPropertyChanged();
            }
            get { return inventoryQuery; }
        }
        public List<ProductViewModel> Products
        {
            get
            {
                return InventoryServiceProxy.Current.Products.Where(p => p != null && p.Quantity > 0)
                    .Where(p => p?.Name?.ToUpper()?.Contains(InventoryQuery.ToUpper()) ?? false)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }

        public List<ProductViewModel> ProductInCart
        {
            get
            {
                return ShoppingCartServiceProxy.Current?.Cart?.Contents?.Where(p => p != null && p.Quantity > 0)
                    .Where(p => p?.Name?.ToUpper()?.Contains(InventoryQuery.ToUpper()) ?? false)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }


        private ProductViewModel? productToBuy;
        public ProductViewModel? ProductToBuy
        {
            get => productToBuy;

            set
            {
                productToBuy = value;

                if (productToBuy != null && productToBuy.Model == null)
                {
                    productToBuy.Model = new Product();
                }
                else if(productToBuy != null && productToBuy.Model != null)
                {
                    productToBuy.Model = new Product(productToBuy.Model);
                }

                NotifyPropertyChanged();
            }
        }

        // NEW STUFF //

        private ProductViewModel? selectedProduct;

        public ProductViewModel? SelectedProduct
        {
            get => selectedProduct;

            set
            {
                selectedProduct = value;

                if (selectedProduct != null && selectedProduct.Model == null)
                {
                    selectedProduct.Model = new Product();
                }
                else if (selectedProduct != null && selectedProduct.Model != null)
                {
                    selectedProduct.Model = new Product(selectedProduct.Model);
                }

                NotifyPropertyChanged();
            }
        }

        // //
        public ShoppingCart Cart {
            get
            {
                return ShoppingCartServiceProxy.Current.Cart;
            }
        }

        public void Refresh()
        {
            InventoryQuery = string.Empty;
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(TotalPrice));
        }

        public void Search()
        {
            NotifyPropertyChanged(nameof(Products));
        }

        public void PlaceInCart()
        {
            if (ProductToBuy?.Model == null)
            {
                return;
            }
            //ProductToBuy.Model = new Product(ProductToBuy.Model);
            ProductToBuy.Model.Quantity = 1;
            ShoppingCartServiceProxy.Current.AddToCart(ProductToBuy.Model);

            //productToBuy = null;
            ProductToBuy = null;
            NotifyPropertyChanged(nameof(ProductInCart));
            NotifyPropertyChanged(nameof(Products));

            NotifyPropertyChanged(nameof(TotalPrice));
        }

        // Remove //
        public void RemoveFromCart()
        {
            if (SelectedProduct == null)
            {
                return;
            }
            //SelectedProduct.Model.Quantity = 1;
            ShoppingCartServiceProxy.Current.RemoveFromCart(SelectedProduct.Model);

            SelectedProduct = null;
            NotifyPropertyChanged(nameof(ProductInCart));
            NotifyPropertyChanged(nameof(Products));

            NotifyPropertyChanged(nameof(TotalPrice));
        }
        //////

        public decimal TotalPrice => ShoppingCartServiceProxy.Current.GetTotalPrice();

        //////

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
