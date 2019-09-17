using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

        private MenuItem AddTagMenuItem(object tag)
        {
            return new MenuItem()
            {
                Header = "Add Tag",
                Tag = tag
            };
        }

        private TextBox NewChatTextBox()
        {
            return new TextBox
            {
                Width = 100,
                Height = 30,
                Background = Brushes.LightSteelBlue,
                Tag = this
            };
        }

        private Rectangle NewNextChatButton()
        {
            return new Rectangle()
            {
                Name = "NextStep",
                Height = 10,
                Width = 10,
                Fill = Brushes.Black
            };
        }

        private Canvas NewMenuOpenButton()
        {
            Canvas ret = new Canvas()
            {
                Height = 10,
                Width = 5
            };
            Ellipse el1 = smallButton();
            Ellipse el2 = smallButton();
            Ellipse el3 = smallButton();

            Canvas.SetLeft(el1, 1);
            Canvas.SetTop(el1, 0);

            Canvas.SetLeft(el2, 1);
            Canvas.SetTop(el2, 3.5);

            Canvas.SetLeft(el3, 1);
            Canvas.SetTop(el3, 7);

            ret.Children.Add(el1);
            ret.Children.Add(el2);
            ret.Children.Add(el3);

            return ret;

            Ellipse smallButton()
            {
                return new Ellipse()
                {
                    Height = 3,
                    Width = 3,
                    Fill = Brushes.Black
                };
            }

        }
    }
}
