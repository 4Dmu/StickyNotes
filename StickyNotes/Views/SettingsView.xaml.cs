﻿using Lib4Mu.Core.Attributes;
using Lib4Mu.WPF.MVVM.Views;
using StickyNotes.ViewModels;
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
    public partial class SettingsView : ShellPage
    {
        public SettingsViewModel viewModel => this.DataContext as SettingsViewModel;
        public SettingsView(SettingsViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
