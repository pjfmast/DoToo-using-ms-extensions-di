using DoToo.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DoToo.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private SQLiteAsyncConnection connection;

        public event EventHandler<TodoItem> OnItemAdded;
        public event EventHandler<TodoItem> OnItemDeleted;
        public event EventHandler<TodoItem> OnItemUpdated;

        private async Task CreateConnection()
        {
            if (connection != null)
            {
                return;
            }

            // Uit het boek (werkt niet goed voor UWP app):
            // Xamarin will find the closest match to 'MyDocuments' on each platform (Android, iOS, UWP) that we target:
            // In this case, we will choose the MyDocuments folder.
            // var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // var databasePath = Path.Combine(documentPath, "TodoItems.db");
            // connection = new SQLiteAsyncConnection(databasePath);

            // beter oplossing zie:
            //     https://docs.microsoft.com/nl-nl/learn/modules/store-local-data-with-sqlite/2-compare-storage-options
            //    you access the library folder location using Xamarin.Essentials
            // through the FileSystem class.
            var libFolder = FileSystem.AppDataDirectory;
            var databasePath = Path.Combine(libFolder, "TodoItems.db");


            connection = new SQLiteAsyncConnection(databasePath);
            await connection.CreateTableAsync<TodoItem>();

            if (await connection.Table<TodoItem>().CountAsync() == 0)
            {
                await connection.InsertAsync(new TodoItem()
                {
                    Title = "Welcome to DoToo",
                    Due = DateTime.Now
                });
            }
        }
        public async Task AddItem(TodoItem item)
        {
            await CreateConnection();
            await connection.InsertAsync(item);

            // After an item has been inserted into the table,
            // we invoke the OnItemAdded event to notify any subscribers
            OnItemAdded?.Invoke(this, item);
        }

        public async Task AddOrUpdate(TodoItem item)
        {
            if (item.Id == 0)
            {
                await AddItem(item);
            }
            else
            {
                await UpdateItem(item);
            }
        }

        public async Task DeleteItem(TodoItem item)
        {
            //throw new NotImplementedException();
            await CreateConnection();
            await connection.DeleteAsync(item);

            // After an item has been deleted from the table,
            // we invoke the OnItemDeleted event to notify any subscribers
            OnItemDeleted?.Invoke(this, item);
        }

        public async Task<List<TodoItem>> GetItems()
        {
            await CreateConnection();
            return await connection
                .Table<TodoItem>()
                .ToListAsync();
        }

        public async Task UpdateItem(TodoItem item)
        {
            await CreateConnection();
            await connection.UpdateAsync(item);
            OnItemUpdated?.Invoke(this, item);
        }
    }
}