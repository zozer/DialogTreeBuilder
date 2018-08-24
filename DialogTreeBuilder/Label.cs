using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DialogTreeBuilder
{
    public partial class ChatTreeDisplay
    {
        private void NewLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Height /= 2;
            textBox.Width /= 2;
            textBox.TextWrapping = TextWrapping.NoWrap;
            Panel.SetZIndex(textBox, (int)textBox.Tag);
            textBox.Tag = null;
            Panel.SetZIndex(Display, (int)Display.Tag);
            Display.Tag = null;
        }

        private void NewLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Height *= 2;
            textBox.Width *= 2;
            textBox.TextWrapping = TextWrapping.WrapWithOverflow;
            textBox.Tag = Panel.GetZIndex(textBox);
            Display.Tag = Panel.GetZIndex(Display);
            Panel.SetZIndex(textBox, int.MaxValue);
            Panel.SetZIndex(Display, int.MaxValue);
        }
    }
}
