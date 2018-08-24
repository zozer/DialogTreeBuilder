using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DialogTreeBuilder
{
    public partial class ChatTreeDisplay
    {
        private Menu NewMenu()
        {
            return new Menu()
            {
                Height = 50,
                Width = 75
            };
        }

        private MenuItem DeleteMenuItem(object tag)
        {
            return new MenuItem()
            {
                Header = "Delete",
                Tag = tag
            };
        }
    }
}
