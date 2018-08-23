using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DialogTreeBuilder
{
    public static class Extensions
    {
        public static UIElement FindChild(this Canvas canvas, string name)
        {
            foreach (UIElement e in canvas.Children)
            {
                if ((e as FrameworkElement)?.Name == name)
                {
                    return e;
                }
            }
            return null;
        }
    }
}
