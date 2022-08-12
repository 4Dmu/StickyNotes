using Lib4Mu.WPF.Core.Markup;
using Lib4Mu.WPF.ShellControl.Controls;
using Lib4Mu.WPF.ShellUI.Controls;
using MahApps.Metro.IconPacks;
using StickyNotes.Data;
using StickyNotes.Messages;
using StickyNotes.Models;
using StickyNotes.ViewModels;
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
   
    public partial class NoteWindow : Lib4Mu.WPF.ShellUI.Controls.ShellWindow
    {
        public NoteViewModel viewModel => this.DataContext as NoteViewModel;

        public NoteWindow(NoteViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this.Closed += async (s, e) =>
            {
                


                foreach (var window in App.Current.Windows.OfType<Window>().ToList())
                    if (window.IsVisible)
                        return;
                App.Current.Shutdown();
            };

            txtBox.AppendText(vm.Note.Text);
        }

        private async void PackIconControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var database = await StickyNotesDatabase.Instance;

            StickyNote note = new();

            note.SetColor(Constants.DefaultBrush);
            note.Id = Guid.NewGuid();
            note.Date = DateTime.Now;
            note.LastModification = DateTime.Now;
            note.Text = String.Empty;

            var res = await database.SaveGuidItemAsync(note);

            Shell.Current.EventAggregator.Publish<NewStickyNoteMessage>(new NewStickyNoteMessage(note));
        }

        private void PackIconControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIconControl con)
                con.Background = new SolidColorBrush { Color = Colors.Black, Opacity = 0.3 };
        }

        private void PackIconControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIconControl con)
                con.Background = Brushes.Transparent;
        }

        private void PackIconControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Content is Grid parent && parent.Children.OfType<Grid>().FirstOrDefault() is Grid child)
            {
                child.Visibility = child.Visibility ==Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void RichTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Content is Grid parent && parent.Children.OfType<Grid>().FirstOrDefault() is Grid child)
            {
                child.Visibility = Visibility.Collapsed;
            }
        }

        private async void ColorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Color c)
            {
                viewModel.Note.SetColor(new StickyColor(c.R, c.G, c.B, c.A));
                TitleBarBackground = viewModel.Note.WPFBrush;
                var database = await StickyNotesDatabase.Instance;
                var res = await database.SaveGuidItemAsync(viewModel.Note);
                if (Shell.Current is not null)
                    Shell.Current.EventAggregator.Publish<NoteColorChangedMessage>(new NoteColorChangedMessage(viewModel.Note));
            }
        }

        private void noteListBtn_Click(object sender, RoutedEventArgs e)
        {
            App.ShowMainWindowAndCreateNewIfNull();
        }

        private async void deleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = Lib4Mu.WPF.ShellUI.Controls.MessageBox.Show(this,"Do you want to delete this note?", "Confirmation", Lib4Mu.WPF.ShellUI.Controls.MessageBoxButton.YesNo, Lib4Mu.WPF.ShellUI.Controls.MessageBoxImage.Exclamation);

            if (result == Lib4Mu.WPF.ShellUI.Controls.MessageBoxResult.Yes)
            {
                var database = await StickyNotesDatabase.Instance;

                await database.DeleteItemAsync(viewModel.Note);

                App.ShowMainWindowAndCreateNewIfNull();
                Shell.Current.EventAggregator.Publish<DeletedNoteMessage>(new DeletedNoteMessage(viewModel.Note));

                this.Close();
            }
        }

        private async void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (viewModel is null)
                return;
            var database = await StickyNotesDatabase.Instance;

            viewModel.Note.Text = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd).Text;
            var res = await database.SaveGuidItemAsync(viewModel.Note);

            if (Shell.Current is not null)
                Shell.Current.EventAggregator.Publish<NoteTextChangedMessage>(new NoteTextChangedMessage(viewModel.Note));
        }

        protected override void InitCloseButton(Button closeButton)
        {

            closeButton.SetBinding(Button.VisibilityProperty,MarkupExtensions.CreateBinding(new PropertyPath(nameof(IsActive)), this, Lib4Mu.WPF.Core.Converters.BetterBoolToVisibilityConverter.Instance));
            base.InitCloseButton(closeButton);
        }
    }
}
