using EventAggregator.Messages;
using StickyNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.Messages
{
    public class DeletedNoteMessage : MessageBase
    {
        public StickyNote ToDelete { get; set; }

        public DeletedNoteMessage(StickyNote toDelete)
        {
            ToDelete = toDelete;
        }
    }
}
