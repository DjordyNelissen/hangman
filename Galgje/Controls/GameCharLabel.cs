using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Galgje.Controls
{
    class GameCharLabel : Label
    {
        public GameCharLabel()
        {

            FontSize = 20;

            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(37, 55, 102));
            Foreground = brush;
            BorderBrush = brush;

            Thickness margin = Margin;
            margin.Left = 2;
            margin.Right = 2;

            Margin = margin;

            Thickness thickness = BorderThickness;
            thickness.Bottom = 1;

            BorderThickness = thickness;

            Height = 70;
            Width = 30;

            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Bottom;

        }
    }
}
