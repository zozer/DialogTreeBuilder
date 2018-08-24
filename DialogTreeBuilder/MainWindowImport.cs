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
    public partial class MainWindow : Window
    {
        private bool CheckLayout()
        {
            if (displays.Count != 0)
            {
                MessageBoxResult res = MessageBox.Show("You have unsaved work.\n" +
                    " Importing a new file will clear your progress without saving." +
                    "\nDo you wish to proceed?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (res != MessageBoxResult.Yes)
                {
                    return false;
                }
                displays.Clear();
                MainCanvas.Children.Clear();
                FirstChat = null;
            }
            return true;
        }

        private enum LoadType { Import, Load};

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLayout())
            {
                return;
            }
            ImportFile(LoadType.Load);
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLayout())
            {
                return;
            }
            ImportFile(LoadType.Import);
        }

        private void ImportFile(LoadType loadType) {
            ChatTreeSave treeSave = null;
            ChatTreeContainer chatTree;
            XmlSerializer serializer = null;
            string defaultType = "";
            switch (loadType)
            {
                case LoadType.Import:
                    serializer = new XmlSerializer(typeof(ChatTreeContainer));
                    defaultType = ".xml";
                    break;
                case LoadType.Load:
                    serializer = new XmlSerializer(typeof(ChatTreeSave));
                    defaultType = ".gz";
                    break;
                default:
                    return;
            }
            
            OpenFileDialog openFile = new OpenFileDialog()
            {
                DefaultExt = defaultType,
                Filter = $"{defaultType.Substring(1)} files|*{defaultType}"
            };
            bool? result = openFile.ShowDialog();
            if (result == true)
            {
                using (var stream = new FileStream(openFile.FileName, FileMode.Open))
                {
                    switch (loadType)
                    {
                        case LoadType.Import:
                            chatTree = serializer.Deserialize(stream) as ChatTreeContainer;
                            break;
                        case LoadType.Load:
                            treeSave = serializer.Deserialize(stream) as ChatTreeSave;
                            chatTree = treeSave.Container;
                            break;
                        default:
                            return;
                    }
                    
                }
                foreach (Chat chat in chatTree.chatTree)
                {
                    ChatTreeDisplay display = new ChatTreeDisplay(MainCanvas, chat);
                    if (loadType == LoadType.Load)
                    {
                        DataPoint point = treeSave.points.First(e => e.name == display.Name);
                        Canvas.SetLeft(display.Display, point.X);
                        Canvas.SetTop(display.Display, point.Y);
                    }
                    displays.Add(display);
                }
                UpdateLayout();
                foreach (ChatTreeDisplay display in displays)
                {
                    if (!display.IsExit)
                    {
                        DrawLines(display);
                    }
                }
            }

            void DrawLines(ChatTreeDisplay display)
            {
                if (display.IsDirect)
                {
                    Rectangle source = display.Display.FindChild("NextStep") as Rectangle;
                    Chat tag = source.Tag as Chat;
                    ChatTreeDisplay nextChat = displays.First(c => c.Name == tag.nextStep);
                    DrawLine(source, display, nextChat);
                    display.nextChat = nextChat;
                    source.Tag = display;
                }
                else
                {
                    int count = 1;
                    List<TextBox> keys = new List<TextBox>(display.paths.Keys);
                    foreach (TextBox option in keys)
                    {
                        Option tag = option.Tag as Option;
                        Rectangle source = display.Display.FindChild($"NextStep{count++}") as Rectangle;
                        ChatTreeDisplay nextChat = displays.First(c => c.Name == tag.nextStep);
                        DrawLine(source, display, nextChat);
                        display.paths[option] = nextChat;
                        option.Tag = display;
                    }
                }
            }

            void DrawLine(Rectangle source, ChatTreeDisplay display, ChatTreeDisplay nextChat)
            {
                Rectangle stop = nextChat.Display.FindChild("MoveBar") as Rectangle;

                Point start = source.TranslatePoint(new Point(source.Width / 2, source.Height / 2), MainCanvas);

                Point end = stop.TranslatePoint(new Point(stop.Width / 2, stop.Height / 2), MainCanvas);
                Line newLine = PointLine(start, end);
                newLine.Tag = new Tuple<TextBox,ChatTreeDisplay>(source.Tag as TextBox,nextChat);
                Panel.SetZIndex(newLine, -1);
                MainCanvas.Children.Add(newLine);
                display.AddOutLine(newLine);
                nextChat.AddInLine(newLine);
            }
        }
        public static Line PointLine(Point a, Point b)
        {
            return new Line()
            {
                X1 = a.X,
                Y1 = a.Y,
                X2 = b.X,
                Y2 = b.Y,
                StrokeThickness = 4,
                Stroke = Brushes.Black
            };
        }
    }
}
