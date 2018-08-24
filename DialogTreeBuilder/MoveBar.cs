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
        private void MoveBar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle header = sender as Rectangle;
            Point mousePos = Mouse.GetPosition(mainCanvas);
            Menu menu = NewMenu();
            MenuItem test = DeleteMenuItem(this);
            test.Click += DeleteDisplay_Click;
            menu.Items.Add(test);
            AddAtMouse(menu);
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
            Canvas area = bar.Parent as Canvas;
            //sorry! will figure out how to actaully do this later
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point loc = Mouse.GetPosition(mainCanvas);

                Canvas.SetLeft(area, Canvas.GetLeft(area) + (loc.X - barMoveX));
                Canvas.SetTop(area, Canvas.GetTop(area) + (loc.Y - barMoveY));
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
