using Amazon.Library.Models;
using Amazon.Library.Services;
using eCommerce.Library.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.MAUI.ViewModels
{
    public class ShopViewModel : INotifyPropertyChanged
    {

        public ShopViewModel()
        {
            InventoryQuery = string.Empty;
            SelectedCart = Carts.FirstOrDefault();
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

        // NEW STUFF //
        private ShoppingCart? selectedCart;

        public ShoppingCart? SelectedCart
        {
            get
            {
                return selectedCart;
            }

            set
            {
                selectedCart = value;
                NotifyPropertyChanged(nameof(ProductsInCart));
                CalculatePrice();
                NotifyPropertyChanged(nameof(TotalPrice));
            }
        }

        public ObservableCollection<ShoppingCart> Carts
        {
            get
            {
                return new ObservableCollection<ShoppingCart>(ShoppingCartServiceProxy.Current.Carts);
            }
        }
        public List<ProductViewModel> ProductsInCart
        {
            get
            {
                return SelectedCart?.Contents?.Where(p => p != null && p.Quantity > 0)
                    .Where(p => p?.Name?.ToUpper()?.Contains(InventoryQuery.ToUpper()) ?? false)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }
        // NEW STUFF //

        private ProductViewModel? productToBuy;
        public ProductViewModel? ProductToBuy
        {
            get => productToBuy;

            set
            {
                productToBuy = value;

                if (productToBuy != null && productToBuy.Model == null)
                {
                    productToBuy.Model = new ProductDTO();
                }
                else if(productToBuy != null && productToBuy.Model != null)
                {
                    productToBuy.Model = new ProductDTO(productToBuy.Model);
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
                    selectedProduct.Model = new ProductDTO();
                }
                else if (selectedProduct != null && selectedProduct.Model != null)
                {
                    selectedProduct.Model = new ProductDTO(selectedProduct.Model);
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
        public decimal TotalPrice { get; set; }

        public void Refresh()
        {
            InventoryQuery = string.Empty;
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(Carts));
            NotifyPropertyChanged(nameof(TotalPrice));
        }

        public void Search()
        {
            NotifyPropertyChanged(nameof(Products));
        }

        public void PlaceInCart()
        {
            if (ProductToBuy?.Model == null || SelectedCart == null)
            {
                return;
            }
            //ProductToBuy.Model = new Product(ProductToBuy.Model);
            ProductToBuy.Model.Quantity = 1;
            ShoppingCartServiceProxy.Current.AddToCart(ProductToBuy.Model, SelectedCart.Id);
            ProductToBuy = null;
            CalculatePrice();

            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(TotalPrice));
        }

        // Remove //
        public void RemoveFromCart()
        {
            if (SelectedProduct == null || SelectedCart == null)
            {
                return;
            }
            //SelectedProduct.Model.Quantity = 1;
            ShoppingCartServiceProxy.Current.RemoveFromCart(SelectedProduct.Model, SelectedCart.Id);
            SelectedProduct = null;
            CalculatePrice();

            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(TotalPrice));
        }

        public void CalculatePrice()
        {
            TotalPrice = ShoppingCartServiceProxy.Current.GetTotalPrice(SelectedCart?.Id ?? 0);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ////// CHECKOUT STUFF
        public void Checkout()
        {
            if (SelectedCart == null)
            {
                return;
            }
            ShoppingCartServiceProxy.Current.Checkout(SelectedCart.Id);

            CalculatePrice();

            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(TotalPrice));
        }

        //////
    }
}
