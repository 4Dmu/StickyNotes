using Lib4Mu.Core.Attributes;
using Lib4Mu.MVVM.Commands;
using Lib4Mu.MVVM.ViewModels;
using Lib4Mu.WPF.MVVM.Views;
using Lib4Mu.WPF.ShellControl.Controls;
using MahApps.Metro.IconPacks;
using StickyNotes.Data;
using StickyNotes.Messages;
using StickyNotes.Models;
using StickyNotes.Views;
using StickyNotes.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StickyNotes.ViewModels
{
    [Inject(InjectionType.Transient)]
    public class NotesListViewModel : ComplexViewModelBase
    {
        public List<StickyNote> NotesList { get; set; }
        public ObservableCollection<StickyNote> FilteredNotesList { get; set; }

        public RelayCommand<MouseButtonEventArgs> NoteMoreOptionsCommand { get; }
        public RelayCommand<Button> ToggleNoteOpenedCommand { get; }
        public RelayCommand<Button> DeleteNoteCommand { get; }

        public NotesListViewModel()
        {
            // init lists
            NotesList = new();
            FilteredNotesList = new();
            
            // init commands
            NoteMoreOptionsCommand = new(NoteMoreOptions, () => true);
            ToggleNoteOpenedCommand = new(ToggleNoteOpened, () => true);
            DeleteNoteCommand = new(DeleteNote, () => true);

            // subscribe to messages
            Shell.Current.EventAggregator.Subscribe<NewStickyNoteMessage>(OnNewNewStickyNoteMessageRecieved);
            Shell.Current.EventAggregator.Subscribe<DeletedNoteMessage>(OnDeleteStickyNoteMessageRecieved);
            Shell.Current.EventAggregator.Subscribe<NoteColorChangedMessage>(OnNoteColorChangedMessageRecieved);
            Shell.Current.EventAggregator.Subscribe<NoteTextChangedMessage>(OnNoteTextChangedMessageRecieved);
        }

        private void OnNoteTextChangedMessageRecieved(NoteTextChangedMessage obj)
        {
            // check that object and note are not null
            if (obj is null || obj.Note is null)
                return;

            // find the note in list that's id matched the sent note
            var note = NotesList.First(x => x.Id == obj.Note.Id);

            // get the index of that note
            var index = NotesList.IndexOf(note);

            // remove the note
            NotesList.RemoveAt(index);

            // insert the received note at the index
            NotesList.Insert(index, obj.Note);

            // filter 
            Filter("");
        }

        private void OnNoteColorChangedMessageRecieved(NoteColorChangedMessage obj)
        {
            // check that object and note are not null
            if (obj is null || obj.Note is null)
                return;

            // find the note in list that's id matched the sent note
            var note = NotesList.First(x => x.Id == obj.Note.Id);

            // get the index of that note
            var index = NotesList.IndexOf(note);

            // remove the note
            NotesList.RemoveAt(index);

            // insert the received note at the index
            NotesList.Insert(index, obj.Note);

            // filter 
            Filter("");
        }

        private void OnDeleteStickyNoteMessageRecieved(DeletedNoteMessage obj)
        {
            // check that object and note are not null
            if (obj is null || obj.ToDelete is null)
                return;
            
            // remove deleted note
            NotesList.Remove(obj.ToDelete);

            // filter to remove deleted note from visible list
            Filter("");
        }

        private void OnNewNewStickyNoteMessageRecieved(NewStickyNoteMessage obj)
        {
            // check that object and note are not null
            if (obj is null || obj.Note is null)
                return;

            // add new note
            NotesList.Add(obj.Note);

            // filter to add new note to visible list
            Filter("");
        }

        public void Filter(string text)
        {
            // clear the visible list
            FilteredNotesList.Clear();

            // get all notes that match the criteria
            var notes = NotesList.Where(x => x.Text.ToLower().Contains(text.ToLower()));

            // add all notes that match the criteria to the visible list
            foreach (var note in notes)
                FilteredNotesList.Add(note);
        }

        public async Task GetNotesAsync()
        {
            // get database
            var dataBase = await StickyNotesDatabase.Instance;
            
            // get items of type StickyNote
            List<StickyNote> notes = await dataBase.GetItemsAsync<StickyNote>();

            // add notes to list
            NotesList.AddRange(notes);
           
            // add each note to the visible list
            foreach (var note in NotesList)
                FilteredNotesList.Add(note);
        }

        void NoteMoreOptions(MouseButtonEventArgs args)
        {
            // check if source is packicon and get icon's sibling and check if sibling's visiblity is set to collapsed
            if (args.Source is PackIconControl icon && icon.Parent is Grid parent && parent.Children.OfType<Grid>().FirstOrDefault() is Grid control && control.Visibility == System.Windows.Visibility.Collapsed)
            {
                // set sibling's visiblity to visible
                control.Visibility = System.Windows.Visibility.Visible;

                // stop the event from being passes on to other controls
                args.Handled = true;
            }
        }

        void ToggleNoteOpened(Button btn)
        {
            // check if button's datacontext is note and buttons parent is grid
            if (btn.DataContext is StickyNote note && btn.Parent is Grid parent)
            {
                // close more options menu
                parent.Visibility = System.Windows.Visibility.Collapsed;

                // loop through all windows and if window's datacontext's not's id is a match then close the window and return
                foreach (var window in App.Current.Windows.OfType<Window>())
                    if (window is not null && window.DataContext is NoteViewModel vm && vm.Note.Id == note.Id)
                    {
                        window.Close();
                        return;
                    }

                // show a new window for the note
                new NoteWindow(new NoteViewModel(note)).Show();
            }
        }

        async void DeleteNote(Button btn)
        {
            // check if control's datacontext is a note and that it's parent is grid
            if (btn.DataContext is StickyNote note && btn.Parent is Grid parent)
            {
                // close the more options menu
                parent.Visibility = System.Windows.Visibility.Collapsed;
                
                // ask user if they want to delete the note
                var result = Lib4Mu.WPF.ShellUI.Controls.MessageBox.Show(
                Shell.Current,
                "Do you want to delete this note?",
                "Confirmation",
                Lib4Mu.WPF.ShellUI.Controls.MessageBoxButton.YesNo,
                Lib4Mu.WPF.ShellUI.Controls.MessageBoxImage.Exclamation);

                // return if user says no or cancels
                if (result is not Lib4Mu.WPF.ShellUI.Controls.MessageBoxResult.Yes)
                    return;

                // get database
                var database = await StickyNotesDatabase.Instance;

                // delete note
                await database.DeleteItemAsync(note);

                foreach (var window in App.Current.Windows.OfType<Window>())
                    if (window is not null && window.DataContext is NoteViewModel vm && vm.Note.Id == note.Id)
                        window.Close();

                // alerts listeners of deleted note
                if (Shell.Current is not null)
                    Shell
                        .Current
                        .EventAggregator
                        .Publish<DeletedNoteMessage>(new DeletedNoteMessage(note));
            }
        }
    }
}
