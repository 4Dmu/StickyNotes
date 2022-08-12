using EasySQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StickyNotes.Models
{
    public class StickyColor
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; }

        public StickyColor(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
    }

    public class StickyNote : GuidIdItem
    {
        public string Text { get; set; }  
        
        public DateTime Date { get; set; }

        public DateTime LastModification { get; set; }

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; }

        [Ignore]
        public StickyColor Color { get => new StickyColor(Red, Green, Blue, Alpha); }
        [Ignore]
        public Color WPFColor => System.Windows.Media.Color.FromArgb(Color.Alpha, Color.Red, Color.Green, Color.Blue);
        [Ignore]
        public SolidColorBrush WPFBrush => new System.Windows.Media.SolidColorBrush(WPFColor);

        public void SetColor(StickyColor color)
        {
            Red = color.Red;
            Green = color.Green;
            Blue = color.Blue;
            Alpha = color.Alpha;
        }

    }
}
