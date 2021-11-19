using System.Windows;
using System.Windows.Controls;

namespace Galgje.Controls
{
    public class GameStackPanel : StackPanel
    {
        public GameStackPanel()
        {
            Orientation = Orientation.Horizontal;

            Thickness margin = Margin;

            margin.Right = 10;
            margin.Left = 10;

            Margin = margin;
        }
    }
}
