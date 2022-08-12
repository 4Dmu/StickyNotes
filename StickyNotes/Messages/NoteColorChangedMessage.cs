using EventAggregator.Messages;
using StickyNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.Messages
{
    public class NoteColorChangedMessage  : MessageBase
    {
        public StickyNote Note { get; set; }

        public NoteColorChangedMessage(StickyNote note)
        {
            Note = note;
        }
    }
}
