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

        public static List<ChatTreeDisplay> displays = new List<ChatTreeDisplay>();
        public static ChatTreeDisplay FirstChat = null;
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.CanResizeWithGrip;
        }

        private void MainCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (ChatTreeDisplay.isCreatingLine) return;
            displays.Add(new ChatTreeDisplay(MainCanvas));
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ChatTreeContainer container = new ChatTreeContainer();
            foreach (ChatTreeDisplay display in displays)
            {
                container.chatTree.Add(display.BuildChat());
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ChatTreeContainer));
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                FileName = "New chat tree",
                DefaultExt = ".xml",
                Filter = "xml files|*.xml"
            };
            bool? result = saveFile.ShowDialog();
            if (result == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFile.FileName))
                {
                    serializer.Serialize(writer, container);
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (displays.Count != 0)
            {
                MessageBoxResult res = MessageBox.Show("You have unsaved work.\n" +
                    " Importing a new file will clear your progress without saving." +
                    "\nDo you wish to proceed?","Warning!",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
                if(res == MessageBoxResult.Yes)
                {
                    displays.Clear();
                    MainCanvas.Children.Clear();
                    FirstChat = null;
                } else
                {
                    return;
                }
            }
            ChatTreeContainer chatTree;
            XmlSerializer serializer = new XmlSerializer(typeof(ChatTreeContainer));
            OpenFileDialog openFile = new OpenFileDialog()
            {
                DefaultExt = ".xml",
                Filter = "xml files|*.xml"
            };
            bool? result = openFile.ShowDialog();
            if (result == true)
            {
                using (var stream = new FileStream(openFile.FileName, FileMode.Open))
                {
                    chatTree = serializer.Deserialize(stream) as ChatTreeContainer;
                }
                foreach (Chat chat in chatTree.chatTree)
                {
                    displays.Add(new ChatTreeDisplay(MainCanvas, chat));
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
                    source.Tag = null;
                } else
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
                        option.Tag = null;
                    }
                } 
            }

            void DrawLine(Rectangle source, ChatTreeDisplay display, ChatTreeDisplay nextChat)
            {
                Rectangle stop = nextChat.Display.FindChild("MoveBar") as Rectangle;
                
                Point start = source.TranslatePoint(new Point(source.Width / 2, source.Height / 2), MainCanvas);
                
                Point end = stop.TranslatePoint(new Point(stop.Width / 2, stop.Height / 2), MainCanvas);
                Line newLine = PointLine(start, end);
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

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.Focusable = true;
            Keyboard.Focus(MainCanvas);
        }
    }
}
