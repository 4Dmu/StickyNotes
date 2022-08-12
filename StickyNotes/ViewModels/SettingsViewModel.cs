using Lib4Mu.Core.Attributes;
using Lib4Mu.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.ViewModels
{
    [Inject(InjectionType.Transient)]
    public class SettingsViewModel : ComplexViewModelBase
    {
    }
}
