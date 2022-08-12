using EventAggregator.Messages;
using StickyNotes.Models;

namespace StickyNotes.Messages
{
    public class NewStickyNoteMessage : MessageBase
    {
        public StickyNote Note { get; set; }

        public NewStickyNoteMessage(StickyNote note)
        {
            Note = note;
        }
    }
}
