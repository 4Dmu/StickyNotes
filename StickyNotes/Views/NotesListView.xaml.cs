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

            // set datacontext to inputed view model
            this.DataContext = vm;

            // subcribe to events
            this.Loaded += async (s, e) =>
            {
                // get the notes
                await viewModel.GetNotesAsync();
            };
        }

        private void SfMaskedEdit_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // make sure event is not propigated to other controls
            e.Handled = true;

            // create new event with the old one's data
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            // set the new event's routed event to UIElement.MouseWheelEvent;
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;

            // set the source to the original events sender
            eventArg.Source = sender;

            // raise this event on the main scroll viewer
            mainScroll.RaiseEvent(eventArg);
        }

        private void NoteInstance_MouseEnter(object sender, MouseEventArgs e)
        {
            // check if sender is border and border's child's children are texblock and packicon
            if (sender is Border parent
                && parent.Child is Grid child
                && child.Children.Count >= 2
                && child.Children[0] is TextBlock textBlock
                && child.Children[1] is PackIconControl packIcon)
            {
                // set textblock's visiblity to collapsed
                textBlock.Visibility = Visibility.Collapsed;

                // set packicon's visibility to visible
                packIcon.Visibility = Visibility.Visible;
            }
        }

        private void NoteInstance_MouseLeave(object sender, MouseEventArgs e)
        {
            // check if sender is border and border's child's children are texblock and packicon
            if (sender is Border parent
                && parent.Child is Grid child
                && child.Children.Count >= 2
                && child.Children[0] is TextBlock t
                && child.Children[1] is PackIconControl pic)
            {
                // set textblock's visiblity to visible
                t.Visibility = Visibility.Visible;

                // set packicon's visibility to collapsed
                pic.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseMoreOptionsMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // check if sender is grid and one of grid's children is grid and grid's visibitly is set to visible
            if (sender is Grid parent
                && parent.Children.OfType<Grid>().FirstOrDefault() is Grid child
                && child.Visibility == Visibility.Visible)
            {
                // set child's visibility to collapsed
                child.Visibility = Visibility.Collapsed;
            }
            
            // else check if sender is grid and grid's datacontext is note
            else if (sender is Grid parent2 && parent2.DataContext is StickyNote note)
            {
                // open the note
                OpenNote(note);
            }
        }

        private void OpenNote(StickyNote note)
        {
            // create a new NoteWindow
            var window = new NoteWindow(new NoteViewModel(note));

            // show the window
            window.Show();
        }

        private void SfMaskedEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // check if sender is SfMaskedEdit and SfMaskedEdit's sibling is grid and grid's visibility is set to visible
            if (sender is SfMaskedEdit child
                && child.Parent is Grid parent
                && parent.Children.OfType<Grid>().FirstOrDefault() is Grid sibling
                && sibling.Visibility == Visibility.Visible)
            {
                // set siblings visibility to collapsed
                sibling.Visibility = Visibility.Collapsed;
            }

            // else check if sender os SfMaskedEdit and SfMaskedEdit's parent's datacontext is note
            else if (sender is SfMaskedEdit child2 && child2.Parent is Grid parent2 && parent2.DataContext is StickyNote note)
            {
                // open the note
                OpenNote(note);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // check if source is textbox
            if (e.Source is TextBox textBox)
            {
                // filter the note list using the textbox's text
                viewModel.Filter(textBox.Text);
            }
        }
    }
}
