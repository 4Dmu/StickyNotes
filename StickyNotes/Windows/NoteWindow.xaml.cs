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

        private async void NewNoteIcon_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (viewModel is null)
                return;

            await viewModel.CreateNoteAsync();
        }

        private void MoreOptionsIcon_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (Content is Grid parent && parent.Children.OfType<Grid>().FirstOrDefault() is Grid child)
            {
                child.Visibility = child.Visibility ==Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void TextBox_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (Content is Grid parent && parent.Children.OfType<Grid>().FirstOrDefault() is Grid child)
            {
                child.Visibility = Visibility.Collapsed;
            }
        }

        private async void ChangeColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not Color color || viewModel is null)
                return;

            await viewModel.ChangeNoteColorAsync(color);
        }

        private void noteListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel is null)
                return;

            viewModel.ShowAppMainWindow();
        }

        private async void deleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel is null)
                return;

            await viewModel.DeleteNoteAsync(this);
        }

        private async void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (viewModel is null)
                return;

            await viewModel.ChangeNoteText(new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd).Text);
        }

        protected override void InitCloseButton(Button closeButton)
        {

            closeButton.SetBinding(Button.VisibilityProperty,MarkupExtensions.CreateBinding(new PropertyPath(nameof(IsActive)), this, Lib4Mu.WPF.Core.Converters.BetterBoolToVisibilityConverter.Instance));
            base.InitCloseButton(closeButton);
        }
    }
}
