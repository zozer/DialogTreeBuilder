using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;
namespace DialogTreeBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double barMoveX;
        private double barMoveY;
        public static List<ChatTreeDisplay> displays = new List<ChatTreeDisplay>();
        public static ChatTreeDisplay FirstChat = null;
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.CanResizeWithGrip;
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (ChatTreeDisplay.isCreatingLine) return;
            if (e.ClickCount == 2)
            {
                displays.Add(new ChatTreeDisplay(MainCanvas));
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.Focusable = true;
            Keyboard.Focus(MainCanvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = Mouse.GetPosition(MainCanvas);
                barMoveX = point.X;
                barMoveY = point.Y;
            }
            if (TotalItemsOver() > 1)
            {
                return;
            }
            ClearAllObjects<Menu>(MainCanvas);

        }

        public static void ClearAllObjects<T> (Canvas canvas) where T : UIElement
        {     
            IEnumerable<T> list = new List<T>(canvas.Children.OfType<T>());
            if (list.Count() > 0)
            {
                foreach (T el in list)
                {
                    canvas.Children.Remove(el);
                }
            }
        }

        public int TotalItemsOver()
        {
            List<DependencyObject> hitResultsList = new List<DependencyObject>();
            VisualTreeHelper.HitTest(MainCanvas, null,
    new HitTestResultCallback(MyHitTestResult),
    new PointHitTestParameters(Mouse.GetPosition(MainCanvas)));
            return hitResultsList.Count;
            HitTestResultBehavior MyHitTestResult(HitTestResult result)
            {
                // Add the hit test result to the list that will be processed after the enumeration.
                hitResultsList.Add(result.VisualHit);
                // Set the behavior to return visuals at all z-order levels.
                return HitTestResultBehavior.Continue;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (TotalItemsOver() > 1)
                {
                    return;
                }
                Point loc = Mouse.GetPosition(MainCanvas);
                foreach (Canvas item in MainCanvas.Children.OfType<Canvas>())
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + (loc.X - barMoveX));
                    Canvas.SetTop(item, Canvas.GetTop(item) + (loc.Y - barMoveY));
                }
                foreach (Line line in MainCanvas.Children.OfType<Line>())
                {
                    line.X1 += loc.X - barMoveX;
                    line.Y1 += loc.Y - barMoveY;
                    line.X2 += loc.X - barMoveX;
                    line.Y2 += loc.Y - barMoveY;
                }
                barMoveX = loc.X;
                barMoveY = loc.Y;
            }
        }
    }
}
