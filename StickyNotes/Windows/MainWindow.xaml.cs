using Lib4Mu.WPF.ShellControl.Controls;
using MahApps.Metro.IconPacks;
using StickyNotes.Data;
using StickyNotes.Messages;
using StickyNotes.Models;
using StickyNotes.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StickyNotes.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Shell
    {
        public static string DateTimeString => DateTime.Now.ToString();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Add_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var database = await StickyNotesDatabase.Instance;

            StickyNote note = new StickyNote();
            note.SetColor(Constants.DefaultBrush);
            note.Id = Guid.NewGuid();
            note.Date = DateTime.Now;
            note.LastModification = DateTime.Now;
            note.Text = String.Empty;

            var res = await database.SaveGuidItemAsync(note);

            Shell.Current.EventAggregator.Publish<NewStickyNoteMessage>(new NewStickyNoteMessage(note));
        }

        private void Settings_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            add.Visibility = Visibility.Collapsed;
            settings.Visibility = Visibility.Collapsed;
            this.BackIconVisibility = BarIconVisibility.Top;
            NavigateTo(nameof(SettingsView));
            OverrideBackButton = true;
            fixedTitle.Text = "Settings";
            fixedTitle.Visibility = Visibility.Visible;
            BackButtonCommand = new Lib4Mu.MVVM.Commands.AsyncRelayCommand( async () => 
            {
                fixedTitle.Text = "";
                fixedTitle.Visibility = Visibility.Collapsed;
                add.Visibility = Visibility.Visible;
                settings.Visibility = Visibility.Visible;
                this.BackIconVisibility = BarIconVisibility.Hidden;
                Shell.Current.NavigateTo(nameof(NotesListView)); 

            }, () => true);
        }

        protected override void InitTitleBarBackButton(PackIconControl icon)
        {
            icon.Width = 14;
            icon.Height = 14;
            base.InitTitleBarBackButton(icon);
        }
    }
}
