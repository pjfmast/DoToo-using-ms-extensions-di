﻿using DoToo.Models;
using DoToo.Repositories;
using DoToo.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DoToo.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly TodoItemRepository repository;

        public ObservableCollection<TodoItemViewModel> Items { get; set; }

        public bool ShowAll { get; set; }

        public string FilterText => ShowAll ? "All" : "Active";

        public ICommand ToggleFilter => new Command(async () => {
            ShowAll = !ShowAll;
            await LoadData();
        });
        public MainViewModel(TodoItemRepository repository)
        {
            repository.OnItemAdded += (sender, item) =>
                 Items.Add(CreateTodoItemViewModel(item));
            repository.OnItemUpdated += (sender, item) =>
                Task.Run(async () => await LoadData());
            this.repository = repository;
            Task.Run(async () => await LoadData());
        }

        public TodoItemViewModel SelectedItem
        {
            get { return null; }
            set
            {
                Device.BeginInvokeOnMainThread(async () =>
                await NavigateToItem(value));
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }

        private async Task NavigateToItem(TodoItemViewModel item)
        {
            if (item == null)
            {
                return;
            }

            var itemView = Startup.ServiceProvider.GetService<ItemView>();
            var vm = itemView.BindingContext as ItemViewModel;
            vm.Item = item.Item;

            await Navigation.PushAsync(itemView);
        }

        public ICommand AddItem => new Command(async () =>
        {
            var itemView = Startup.ServiceProvider.GetService<ItemView>();
            await Navigation.PushAsync(itemView);
        });

        private async Task LoadData()
        {
            var items = await repository.GetItems();
            // page 89, Creating the filter toggle function using a command
            if (!ShowAll)
            {
                items = items.Where(x => x.Completed == false).ToList();
            }

            var itemViewModels = items.Select(
                i => CreateTodoItemViewModel(i));
            Items = new ObservableCollection<TodoItemViewModel>(itemViewModels);
        }

        private TodoItemViewModel CreateTodoItemViewModel(TodoItem item)
        {
            var itemViewModel = new TodoItemViewModel(item);
            itemViewModel.ItemStatusChanged += ItemStatusChanged;
            return itemViewModel;
        }

        private void ItemStatusChanged(object sender, EventArgs e)
        {
            if (sender is TodoItemViewModel item)
            {
                if (!ShowAll && item.Item.Completed)
                {
                    Items.Remove(item);
                }
                Task.Run(async () => await repository.UpdateItem(item.Item));
            }
        }
    }
}