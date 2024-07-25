using Amazon.Library.Models;
using Amazon.Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace eCommerce.MAUI.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {

        public InventoryViewModel()
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
        public List<ProductViewModel> Products { 
            get {
                return InventoryServiceProxy.Current.Products.Where(p=>p != null)
                    .Select(p => new ProductViewModel(p)).ToList() 
                    ?? new List<ProductViewModel>();
            } 
        }

        // NEW STUFF

        public ProductViewModel? SelectedProduct { get; set; }

        public void EditProduct()
        {
            Shell.Current.GoToAsync($"//Product?productId={SelectedProduct?.Model?.Id ?? 0}");
        }
        public async void DeleteProduct()
        {
            await InventoryServiceProxy.Current.Delete(SelectedProduct?.Model?.Id ?? 0);
            Refresh();
        }
        //

        public async void Refresh()
        {
            await InventoryServiceProxy.Current.Get();
            NotifyPropertyChanged(nameof(Products));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // NEW STUFF AGAIN //
        private string status;
        public string Status
        {
            get => status;
            set
            {
                status = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ImportCsvCommand { get; }

        public async void Import()
        {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select a CSV file",
                });

                if (result != null)
                {
                    ImportCsv(result.FullPath);
                    Refresh();
                }
        }

        private async void ImportCsv(string csvFilePath)
        {
            await InventoryServiceProxy.Current.MassImportFromCsv(csvFilePath);
            Refresh();
        }
        //
    }
}
