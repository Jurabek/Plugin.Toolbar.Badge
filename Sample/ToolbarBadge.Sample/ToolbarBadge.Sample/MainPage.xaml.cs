using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToolbarBadge.Sample.Annotations;
using Xamarin.Forms;

namespace ToolbarBadge.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel();
            BindingContext = mainViewModel;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand BasketCommand => new Command((p) =>
        {
            count++;
            ItemsCount = count.ToString();
        });

        private string _itemsCount;
        public string ItemsCount
        {
            get => _itemsCount;
            set
            {
                _itemsCount = value;
                OnPropertyChanged();
            }
        }

        private int count = 0;

        public ICommand Increment => new Command((p) =>
        {
            count++;
            ItemsCount = count.ToString();
        });

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
