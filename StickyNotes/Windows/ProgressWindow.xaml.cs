using Lib4Mu.WPF.ShellUI.Controls;
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
using System.Windows.Shapes;

namespace StickyNotes.Windows
{
    public partial class ProgressWindow : ShellWindow
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress",
                                                                                                 typeof(double),
                                                                                                 typeof(ProgressWindow),
                                                                                                 new PropertyMetadata(0.0));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message",
                                                                                                typeof(string),
                                                                                                typeof(ProgressWindow),
                                                                                                new PropertyMetadata(string.Empty));

        public ProgressWindow()
        {
            InitializeComponent();
        }

    }
}
