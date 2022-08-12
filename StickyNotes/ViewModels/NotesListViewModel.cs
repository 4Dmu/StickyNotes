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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            NotesList = new();
            FilteredNotesList = new();
            NoteMoreOptionsCommand = new(NoteMoreOptions, () => true);
            ToggleNoteOpenedCommand = new(ToggleNoteOpened, () => true);
            DeleteNoteCommand = new(DeleteNote, () => true);
            PropertyChanged += OnNotesListViewModelPropertyChanged;
            Shell.Current.EventAggregator.Subscribe<NewStickyNoteMessage>(OnNewNewStickyNoteMessageRecieved);
            Shell.Current.EventAggregator.Subscribe<DeletedNoteMessage>(OnDeleteStickyNoteMessageRecieved);
            Shell.Current.EventAggregator.Subscribe<NoteColorChangedMessage>(OnNoteColorChangedMessageRecieved);
            Shell.Current.EventAggregator.Subscribe<NoteTextChangedMessage>(OnNoteTextChangedMessageRecieved);
        }

        private void OnNoteTextChangedMessageRecieved(NoteTextChangedMessage obj)
        {
            var note = NotesList.First(x => x.Id == obj.Note.Id);
            var index = NotesList.IndexOf(note);
            NotesList.RemoveAt(index);
            NotesList.Insert(index, obj.Note);
            Filter("");
        }

        private void OnNoteColorChangedMessageRecieved(NoteColorChangedMessage obj)
        {
            var note = NotesList.First(x => x.Id == obj.Note.Id);
            var index = NotesList.IndexOf(note);
            NotesList.RemoveAt(index);
            NotesList.Insert(index, obj.Note);
            Filter("");
        }

        private void OnDeleteStickyNoteMessageRecieved(DeletedNoteMessage obj)
        {
            NotesList.Remove(obj.ToDelete);
            Filter("");
        }

        private void OnNewNewStickyNoteMessageRecieved(NewStickyNoteMessage obj)
        {
            if (obj is null || obj.Note is null)
                return;

            NotesList.Add(obj.Note);
            Filter("");
        }

        private void OnNotesListViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void Filter(string text)
        {
            FilteredNotesList.Clear();
            var notes = NotesList.Where(x => x.Text.ToLower().Contains(text.ToLower()));

            foreach (var note in notes)
                FilteredNotesList.Add(note);
        }

        public async Task GetNotesAsync()
        {
            var dataBase = await StickyNotesDatabase.Instance;

            List<StickyNote> notes = await dataBase.GetItemsAsync<StickyNote>();
            NotesList.AddRange(notes);
           
            foreach (var note in NotesList)
                FilteredNotesList.Add(note);
        }

        void NoteMoreOptions(MouseButtonEventArgs args)
        {
            if (args.Source is PackIconControl icon && icon.Parent is Grid parent && parent.Children.OfType<Grid>().FirstOrDefault() is Grid control && control.Visibility == System.Windows.Visibility.Collapsed)
            {
                control.Visibility = System.Windows.Visibility.Visible;
                args.Handled = true;
            }
        }

        void ToggleNoteOpened(Button btn)
        {
            if (btn.DataContext is StickyNote note && btn.Parent is Grid parent)
            {
                parent.Visibility = System.Windows.Visibility.Collapsed;   
            }
        }
        void DeleteNote(Button btn)
        {
            if (btn.DataContext is StickyNote note && btn.Parent is Grid parent)
            {
                parent.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
