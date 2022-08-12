using Lib4Mu.Core.Attributes;
using Lib4Mu.MVVM.ViewModels;
using Lib4Mu.WPF.ShellControl.Controls;
using StickyNotes.Data;
using StickyNotes.Messages;
using StickyNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StickyNotes.ViewModels
{
    public class NoteViewModel : ComplexViewModelBase
    {
        public StickyNote Note { get; }

        public NoteViewModel(StickyNote note)
        {
            Note = note;
        }

        private StickyNote NewStickyNote()
        {
            StickyNote note = new();

            note.SetColor(Constants.DefaultBrush);
            note.Id = Guid.NewGuid();
            note.Date = DateTime.Now;
            note.LastModification = DateTime.Now;
            note.Text = String.Empty;

            return note;
        }

        private StickyColor GetStickColorFromColor(Color color)
        {
            // returns new sticky color based inputed colors rgba
            return new StickyColor(color.R, color.G, color.B, color.A);
        }

        public async Task CreateNoteAsync()
        {
            // get database 
            var database = await StickyNotesDatabase.Instance;

            // create new Note
            var note = NewStickyNote();

            // save note
            var res = await database.SaveGuidItemAsync(note);

            // allert listeners of note creation
            if (Shell.Current is not null)
                Shell.Current.EventAggregator.Publish<NewStickyNoteMessage>(new NewStickyNoteMessage(note));
        }

        public async Task ChangeNoteColorAsync(Color color)
        {
            // change note color
            Note.SetColor(GetStickColorFromColor(color));

            // get database
            var database = await StickyNotesDatabase.Instance;

            // save updated note
            var res = await database.SaveGuidItemAsync(Note);

            // alert listeners of notes new color
            if (Shell.Current is not null)
                Shell.Current.EventAggregator.Publish<NoteColorChangedMessage>(new NoteColorChangedMessage(Note));

        }

        public void ShowAppMainWindow()
        {
            // creates a new main window if needed and then shows it
            App.ShowMainWindowAndCreateNewIfNull();
        }

        public async Task DeleteNoteAsync(Window window)
        {
            //  ask user if they want to delete the note
            var result = Lib4Mu.WPF.ShellUI.Controls.MessageBox.Show(
                window,
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
            await database.DeleteItemAsync(Note);

            // show main window
            ShowAppMainWindow();

            // alerts listeners of deleted note
            if (Shell.Current is not null)
                Shell
                    .Current
                    .EventAggregator
                    .Publish<DeletedNoteMessage>(new DeletedNoteMessage(Note));

            // Close the window of which this viewmodel instance is the datacontext
            window.Close();
        }

        public async Task ChangeNoteText(string newValue)
        {
            // get database
            var database = await StickyNotesDatabase.Instance;

            // set note's text
            Note.Text = newValue;  

            // save edited note
            var res = await database.SaveGuidItemAsync(Note);

            // alerts listeners of note's changed text
            if (Shell.Current is not null)
                Shell.Current.EventAggregator.Publish<NoteTextChangedMessage>(new NoteTextChangedMessage(Note));
        }
    }
}
