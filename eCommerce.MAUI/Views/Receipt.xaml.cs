using eCommerce.MAUI.ViewModels;

namespace eCommerce.MAUI.Views;

public partial class Receipt : ContentPage
{
	public Receipt()
	{
		InitializeComponent();
        BindingContext = new ShopViewModel();
    }

    private void DoneClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Shop");
    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (BindingContext as ShopViewModel).Refresh();
    }
}