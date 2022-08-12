using Lib4Mu.Core.Attributes;
using Lib4Mu.MVVM.ViewModels;
using StickyNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.ViewModels
{
    public class NoteViewModel : ComplexViewModelBase
    {
        public StickyNote Note { get; }

        public NoteViewModel(StickyNote note)
        {
            Note = note;
        }
    }
}
