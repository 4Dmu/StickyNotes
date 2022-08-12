using Lib4Mu.Core.Attributes;
using Lib4Mu.WPF.MVVM.Views;
using MahApps.Metro.IconPacks;
using StickyNotes.Models;
using StickyNotes.ViewModels;
using StickyNotes.Windows;
using Syncfusion.Windows.Controls.Input;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StickyNotes.Views
{
    [Inject(InjectionType.Transient)]
    public partial class NotesListView : ShellPage
    {
        public NotesListViewModel viewModel => this.DataContext as NotesListViewModel;

        public NotesListView(NotesListViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this.Loaded += async (s, e) =>
            {
                await viewModel.GetNotesAsync();
            };
        }

        private void SfMaskedEdit_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            eventArg.RoutedEvent = UIElement.MouseWheelEvent;

            eventArg.Source = sender;
            mainScroll.RaiseEvent(eventArg);

        }

        private void NoteInstance_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border p && p.Child is Grid p2 && p2.Children.Count >= 2 && p2.Children[0] is TextBlock t && p2.Children[1] is PackIconControl pic)
            {
                t.Visibility = Visibility.Collapsed;
                pic.Visibility = Visibility.Visible;
            }
        }

        private void NoteInstance_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border p && p.Child is Grid p2 && p2.Children.Count >= 2 && p2.Children[0] is TextBlock t && p2.Children[1] is PackIconControl pic)
            {
                t.Visibility = Visibility.Visible;
                pic.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseMoreOptionsMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid p && p.Children.OfType<Grid>().FirstOrDefault() is Grid control && control.Visibility == Visibility.Visible)
            {
                control.Visibility = Visibility.Collapsed;
            }
            else
                if (sender is Grid p1 && p1.DataContext is StickyNote note)
                    OpenNote(note);
        }

        private void OpenNote(StickyNote note)
        {
            var window = new NoteWindow(new NoteViewModel(note));

            window.Show();
        }

        private void SfMaskedEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is SfMaskedEdit ed && ed.Parent is Grid p1 && p1.Children.OfType<Grid>().FirstOrDefault() is Grid control1 && control1.Visibility == Visibility.Visible)
            {
                control1.Visibility = Visibility.Collapsed;
            }
            else
                if (sender is SfMaskedEdit ed1 && ed1.Parent is Grid p2 && p2.DataContext is StickyNote note)
                    OpenNote(note);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Source is TextBox tbox)
                viewModel.Filter(tbox.Text);
        }
    }
}
