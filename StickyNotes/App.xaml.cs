using Lib4Mu.Core.ExtensionMethods;
using Lib4Mu.WPF.ShellControl.Controls;
using Microsoft.Extensions.Hosting;
using Squirrel;
using StickyNotes.Data;
using StickyNotes.Models;
using StickyNotes.ViewModels;
using StickyNotes.Views;
using StickyNotes.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StickyNotes
{
    public partial class App : Application
    {
        IHost _host;
        UpdateManager _manager;
        SynchronizationContext _syncContext;

        public App()
        {
            _host = Host.CreateDefaultBuilder().ConfigureServices((host, services) =>
            {
                services.AutoRegisterDependencies(this.GetType().Assembly.GetTypes());
            })
            .Build();

        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            _manager = await UpdateManager
                .GitHubUpdateManager(@"https://github.com/4Dmu/StickyNotes");

            _syncContext = SynchronizationContext.Current;

            await CheckForUpdatesAsync();
            
            StickyNotesDatabase.Flags = Constants.Flags;
            StickyNotesDatabase.Path = Constants.DatabasePath;
            StickyNotesDatabase.AddTable<StickyNote>();

            Shell.DependencyService.SetServiceProvider(_host.Services);

            Shell.Routing.RegisterRoute(nameof(NotesListView), typeof(NotesListView));
            Shell.Routing.RegisterRoute(nameof(SettingsView), typeof(SettingsView));
            Shell.Routing.RegisterRoute(nameof(MoreOptionsModal), typeof(MoreOptionsModal));

            MainWindow = new MainWindow();

            if (await GetLatestNote() is Window w)
                w.Show();
            else
            {
                MainWindow.Show();

                AfterMainWindowShown();
            }

            

            Shell.Current.ToggleTheme();

            base.OnStartup(e);
        }

        private async Task CheckForUpdatesAsync()
        {
            var updates = await _manager.CheckForUpdate();

            if (updates.ReleasesToApply.Count > 0)
            {
                await UpdateAppAsync();
            }
        }

        private async Task UpdateAppAsync()
        {
            var window = new ProgressWindow();
            window.Title = "Update";
            window.Message = "We're downloading and update, please wait...";
            window.Show();

            Action<int> progress = delegate (int value)
            {
                _syncContext.Post(o => window.Progress = value, null);
            };

            await _manager.UpdateApp(progress);

            window.Close();

            Lib4Mu.WPF.ShellUI.Controls.MessageBox.Show("App updated sucessfully, please restart to so the update can take effect");
        }

        private async Task<Window> GetLatestNote()
        {
            var dataBase = await StickyNotesDatabase.Instance;

            var items = await dataBase.GetItemsAsync<StickyNote>();

            if (items.Count > 0)
                return new NoteWindow(new NoteViewModel(items[0]));

            return null;
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;

            base.OnExit(e);
        }

        internal static void AfterMainWindowShown()
        {
            Shell.Current.NavigateTo(nameof(NotesListView));
        }

        public static void ShowMainWindowAndCreateNewIfNull()
        {
            if (Current.MainWindow is null)
                Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
            AfterMainWindowShown();
        }
    }
}
