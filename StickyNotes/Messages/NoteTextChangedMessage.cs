using EventAggregator.Messages;
using StickyNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.Messages
{
    internal class NoteTextChangedMessage : MessageBase
    {
        public StickyNote Note { get; set; }

        public NoteTextChangedMessage(StickyNote note)
        {
            Note = note;
        }
    }
}
