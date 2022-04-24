using DoToo.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoToo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemView : ContentPage
    {
        public ItemView(ItemViewModel viewmodel)
        {
            InitializeComponent();
            viewmodel.Navigation = Navigation;
            BindingContext = viewmodel;
        }
    }
}