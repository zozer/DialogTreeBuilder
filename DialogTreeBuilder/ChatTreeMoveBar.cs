using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace DialogTreeBuilder
{
    public partial class ChatTreeDisplay
    {
        private void OpenHeaderMenu(object sender, MouseButtonEventArgs e)
        {
            Canvas button = sender as Canvas;
            Point mousePos = Mouse.GetPosition(mainCanvas);
            Menu menu = NewMenu();
            MenuItem test = DeleteMenuItem(this);
            test.Click += DeleteDisplay_Click;
            menu.Items.Add(test);
            Point topleft = button.TranslatePoint(new Point(0, 0), mainCanvas);
            Canvas.SetLeft(menu, topleft.X - menu.Width);
            Canvas.SetTop(menu, topleft.Y);
            mainCanvas.Children.Add(menu);
        }

        private void DeleteDisplay_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            mainCanvas.Children.Remove(item.Parent as UIElement);
            Destroy();
        }

        private void MoveBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (isCreatingLine)
            {
                isCreatingLine = false;
                inLines.Add(createdLine);
                Tuple<TextBox, ChatTreeDisplay> pair = createdLine.Tag as Tuple<TextBox, ChatTreeDisplay>;
                if (pair.Item2.IsDirect)
                {
                    pair.Item2.nextChat = this;
                }
                else
                {
                    pair.Item2.paths[pair.Item1] = this;
                }
                createdLine.Tag = new Tuple<TextBox, ChatTreeDisplay>(pair.Item1, this);
                createdLine.MouseRightButtonDown += CreatedLine_MouseRightButtonDown;
                createdLine = null;
                return;
            }
            Point loc = Mouse.GetPosition(mainCanvas);
            barMoveX = loc.X;
            barMoveY = loc.Y;
        }



        private void MoveBar_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle bar = e.Source as Rectangle;
            //sorry! will figure out how to actaully do this later
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point loc = Mouse.GetPosition(mainCanvas);

                Canvas.SetLeft(Display, Canvas.GetLeft(Display) + (loc.X - barMoveX));
                Canvas.SetTop(Display, Canvas.GetTop(Display) + (loc.Y - barMoveY));
                foreach (Line line in outLines)
                {
                    line.X1 += loc.X - barMoveX;
                    line.Y1 += loc.Y - barMoveY;
                }

                foreach (Line line in inLines)
                {
                    line.X2 += loc.X - barMoveX;
                    line.Y2 += loc.Y - barMoveY;
                }

                barMoveX = loc.X;
                barMoveY = loc.Y;

            }
        }
    }
}
