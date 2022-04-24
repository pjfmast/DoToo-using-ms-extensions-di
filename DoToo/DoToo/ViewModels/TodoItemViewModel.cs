using DoToo.Models;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace DoToo.ViewModels
{
    public class TodoItemViewModel : ViewModel
    {
        public TodoItem Item { get; private set; }
        public event EventHandler ItemStatusChanged;
        public string StatusText => Item.Completed ? "Reactivate" : "Completed";

        public ICommand ToggleCompleted => new Command((arg) => {
            Item.Completed = !Item.Completed;
            ItemStatusChanged?.Invoke(this, new EventArgs());
        });

        public TodoItemViewModel(TodoItem item) => Item = item;
    }

}