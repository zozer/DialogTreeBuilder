using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DialogTreeBuilder
{
    public partial class ChatTreeDisplay
    {
        private double barMoveX;
        private double barMoveY;
        private static int count = 1;
        List<Line> inLines = new List<Line>();
        List<Line> outLines = new List<Line>();
        public Dictionary<TextBox, ChatTreeDisplay> paths = new Dictionary<TextBox, ChatTreeDisplay>();
        public ChatTreeDisplay nextChat = null;
        static bool isCreatingLine = false;
        static Line createdLine = null;
        public bool IsFirst { get; private set; }
        private Canvas mainCanvas;
        public Canvas Display { get; private set; }
        public TextBox Message { get; private set; }
        public bool IsDirect { get => paths.Count == 0; }
        public string Name => Message.Name;
        public bool IsExit { get; private set; }
        private Rectangle moveBar;
        public ChatTreeDisplay(Canvas canvas)
        {
            mainCanvas = canvas;
            IsFirst = MainWindow.displays.Count == 0;
            if (IsFirst)
            {
                MainWindow.FirstChat = this;
            }
            IsExit = false;
            CreateNewDialogStep();
        }

        public ChatTreeDisplay(Canvas canvas, Chat chat)
        {
            mainCanvas = canvas;
            IsFirst = chat.name == "enter";
            IsExit = chat.exit;
            if (!IsFirst)
            {
                count++;
            }
            else
            {
                count = 0;
            }
            CreateNewDialogStep(chat);
        }

        private void AddAtMouse(FrameworkElement item)
        {
            Point loc = Mouse.GetPosition(mainCanvas);
            Canvas.SetLeft(item, loc.X - item.Width / 2);
            Canvas.SetTop(item, loc.Y - item.Height / 2);

            mainCanvas.Children.Add(item);
        }

        private void AddAtNextLoc(FrameworkElement item)
        {
            Canvas.SetLeft(item, 100 + count * 110);
            Canvas.SetTop(item, 50);
            mainCanvas.Children.Add(item);
        }

        private void CreateNewDialogStep(Chat chat = null)
        {
            Canvas newArea = new Canvas()
            {
                Width = 100,
                Height = 45
            };
            TextBox newLabel = new TextBox
            {
                Name = (chat != null) ? chat.name : (IsFirst) ? "enter" : $"step{count++}",
                Text = (chat != null) ? chat.Message : "new dialog",
                Width = 100,
                Height = 30,
                Background = Brushes.LightSteelBlue,
                Tag = this
            };
            Ellipse AddButton = new Ellipse()
            {
                Height = 15,
                Width = 15,
                Fill = Brushes.DarkBlue
            };
            Rectangle MoveBar = new Rectangle()
            {
                Name = "MoveBar",
                Height = 10,
                Width = 100,
                Fill = (chat != null && chat.exit) ? Brushes.DarkRed : (IsFirst) ? Brushes.DarkGreen : Brushes.DarkBlue
            };
            Rectangle NextChatButton = new Rectangle()
            {
                Name = "NextStep",
                Height = 10,
                Width = 10,
                Fill = Brushes.Black
            };
            if (chat != null && !IsExit && IsDirect)
            {
                NextChatButton.Tag = chat;
            }
            CheckBox isExit = new CheckBox()
            {
                IsChecked = (chat != null) ? chat.exit : false,
                Height = 10,
                Width = 10
            };

            newLabel.GotKeyboardFocus += NewLabel_GotFocus;
            newLabel.LostKeyboardFocus += NewLabel_LostFocus;
            AddButton.MouseDown += AddButton_MouseDown;
            MoveBar.MouseMove += MoveBar_MouseMove;
            MoveBar.MouseLeftButtonDown += MoveBar_MouseLeftButtonDown;
            MoveBar.MouseRightButtonDown += MoveBar_MouseRightButtonDown;
            MoveBar.Cursor = Cursors.SizeAll;
            isExit.Checked += IsExit_Checked;
            isExit.Unchecked += IsExit_Unchecked;
            NextChatButton.MouseLeftButtonDown += NextChatButton_MouseLeftButtonDown;
            Canvas.SetLeft(MoveBar, 0);
            Canvas.SetTop(MoveBar, -5);
            Canvas.SetLeft(isExit, 89);
            Canvas.SetTop(isExit, -5);
            Canvas.SetLeft(newLabel, 0);
            Canvas.SetTop(newLabel, 0);
            Canvas.SetLeft(AddButton, -7.5);
            Canvas.SetTop(AddButton, 22.5);
            Canvas.SetLeft(NextChatButton, 40);
            Canvas.SetTop(NextChatButton, 40);

            newArea.Children.Add(newLabel);
            newArea.Children.Add(AddButton);
            newArea.Children.Add(MoveBar);
            newArea.Children.Add(NextChatButton);
            newArea.Children.Add(isExit);
            if (chat == null)
            {
                AddAtMouse(newArea);
            }
            else
            {
                AddAtNextLoc(newArea);
            }
            Display = newArea;
            Message = newLabel;
            moveBar = MoveBar;
            if (chat?.options != null)
            {
                foreach (Option option in chat.options)
                {
                    AddOption(AddButton, option);
                }
            }
        }

        private void IsExit_Unchecked(object sender, RoutedEventArgs e)
        {
            IsExit = false;
            moveBar.Fill = (IsFirst) ? Brushes.DarkGreen : Brushes.DarkBlue;
        }

        private void IsExit_Checked(object sender, RoutedEventArgs e)
        {
            IsExit = true;
            moveBar.Fill = Brushes.DarkRed;
        }

        private void NextChatButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.displays.Count == 1) return;

            Rectangle orgin = sender as Rectangle;
            Point centerPoint = orgin.TranslatePoint(new Point(orgin.ActualHeight / 2, orgin.ActualWidth / 2), mainCanvas);
            Point mousePoint = Mouse.GetPosition(mainCanvas);
            TextBox tag = IsDirect ? (orgin.Parent as Canvas).Children.OfType<TextBox>().First(): tag = orgin.Tag as TextBox; ;                
            if ((IsDirect && nextChat != null) || (!IsDirect && paths[tag] != null)) return;
            Line line = new Line()
            {
                X1 = centerPoint.X,
                Y1 = centerPoint.Y,
                X2 = mousePoint.X,
                Y2 = mousePoint.Y,
                StrokeThickness = 4,
                Stroke = Brushes.Black,
                Tag = new Tuple<TextBox, ChatTreeDisplay>(tag, this)
            };
            Panel.SetZIndex(line, -1);
            mainCanvas.Children.Add(line);
            outLines.Add(line);
            isCreatingLine = true;
            createdLine = line;
            mainCanvas.MouseMove += MainCanvas_MouseMoveLineUpdate;
        }

        private void MainCanvas_MouseMoveLineUpdate(object sender, MouseEventArgs e)
        {
            Point mousePoint = Mouse.GetPosition(mainCanvas);
            if (isCreatingLine)
            {
                createdLine.X2 = mousePoint.X;
                createdLine.Y2 = mousePoint.Y;
            }
        }

        private void AddButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse button = e.Source as Ellipse;
            AddOption(button);
        }

        private void CreatedLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Line line = sender as Line;
            Point mousePos = Mouse.GetPosition(mainCanvas);
            Menu menu = NewMenu();
            MenuItem test = DeleteMenuItem(line);
            test.Click += DeleteLine_Click;
            menu.Items.Add(test);
            AddAtMouse(menu);
        }

        private void DeleteLine_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            mainCanvas.Children.Remove(item.Parent as UIElement);
            DeleteLine(item.Tag as Line);
        }

        private void DeleteOption_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            mainCanvas.Children.Remove(item.Parent as UIElement);
            DeleteOption(item.Tag as TextBox);
        }

        private void Option_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textbox = (sender as Rectangle).Tag as TextBox;
            Point mousePos = Mouse.GetPosition(mainCanvas);
            Menu menu = NewMenu();
            MenuItem test = DeleteMenuItem(textbox);
            test.Click += DeleteOption_Click;
            menu.Items.Add(test);
            AddAtMouse(menu);
        }

        private void AddOption(Ellipse button, Option option = null)
        {
            if (IsDirect)
            {
                if (outLines.Count > 0)
                {
                    mainCanvas.Children.Remove(outLines[0]);
                    outLines.Clear();
                    nextChat.inLines.Clear();
                    nextChat = null;
                }
            }

            TextBox newLabel = new TextBox
            {
                Name = $"Option_{paths.Count+1}",
                Text = (option != null)?option.Message:"new response",
                Width = 100,
                Height = 30,
                Background = Brushes.DarkSlateBlue
            };
            if (option != null)
            {
                newLabel.Tag = option;
            } else
            {
                newLabel.Tag = this;
            }

            Rectangle nextChatButton = new Rectangle()
            {
                Name = $"NextStep{paths.Count+1}",
                Width = 10,
                Height = 10,
                Fill = Brushes.Black,
                Tag = newLabel
            };
            nextChatButton.MouseRightButtonDown += Option_MouseRightButtonDown;
            nextChatButton.MouseLeftButtonDown += NextChatButton_MouseLeftButtonDown;
            newLabel.GotKeyboardFocus += NewLabel_GotFocus;
            newLabel.LostKeyboardFocus += NewLabel_LostFocus;
            double height = Canvas.GetTop(button);
            Canvas.SetTop(button, height + 30);
            Canvas.SetLeft(nextChatButton, 110);
            Canvas.SetTop(nextChatButton, height + 17.5);

            Canvas.SetLeft(newLabel, 0);
            Canvas.SetTop(newLabel, height + 7.5);
            Display.Children.Add(nextChatButton);
            Panel.SetZIndex(button, Display.Children.Add(newLabel));

            paths.Add(newLabel, null);
        }

        public void AddInLine(Line line)
        {
            inLines.Add(line);
        }

        public void AddOutLine(Line line)
        {
            outLines.Add(line);
        }

        public void DeleteOption(TextBox option)
        {           
            ChatTreeDisplay end = paths[option];
            int index = int.Parse(option.Name.Split('_')[1]);
            Rectangle chatButton = Display.FindChild($"NextStep{index}") as Rectangle;
            List<TextBox> options = paths.Keys.OrderBy(e => int.Parse(e.Name.Split('_')[1])).ToList();
            for (int i = index; i < options.Count; i++)
            {
                TextBox textBox = options[i];
                textBox.Name = $"Option_{i}";
                if (paths[textBox] != null)
                {
                    outLines.First(e => (e.Tag as Tuple<TextBox, ChatTreeDisplay>).Item1 == textBox).Y1 -= 30;
                }
                Canvas.SetTop(textBox, Canvas.GetTop(textBox) - 30);
                Rectangle nextChat = Display.FindChild($"NextStep{i + 1}") as Rectangle;
                nextChat.Name = $"NextStep{i}";
                Canvas.SetTop(nextChat, Canvas.GetTop(nextChat) - 30);
            }
            Ellipse button = Display.Children.OfType<Ellipse>().First();
            Canvas.SetTop(button, Canvas.GetTop(button) - 30);
            if (end != null)
            {
                Line line = outLines.First(e => (e.Tag as Tuple<TextBox, ChatTreeDisplay>).Item1 == option);
                DeleteLine(line);
            } else
            {
                paths.Remove(option);
            }
            Display.Children.Remove(option);
            Display.Children.Remove(chatButton);
        }

        public void DeleteLine(Line line)
        {
            Tuple<TextBox, ChatTreeDisplay> pair = line.Tag as Tuple<TextBox, ChatTreeDisplay>;
            ChatTreeDisplay start = pair.Item1.Tag as ChatTreeDisplay;
            ChatTreeDisplay end = pair.Item2;
            if (start.IsDirect)
            {
                start.nextChat = null;
            } else
            {
                start.paths.Remove(pair.Item1);
            }
            start.outLines.Remove(line);
            end.inLines.Remove(line);
            mainCanvas.Children.Remove(line);
        }

        public void Destroy()
        {
            Tuple<TextBox, ChatTreeDisplay> pair;
            foreach (Line line in outLines)
            {
                pair = line.Tag as Tuple<TextBox, ChatTreeDisplay>;
                pair.Item2.inLines.Remove(line);
                mainCanvas.Children.Remove(line);
            }
            foreach(Line line in inLines)
            {
                pair = line.Tag as Tuple<TextBox, ChatTreeDisplay>;
                ChatTreeDisplay source = pair.Item1.Tag as ChatTreeDisplay;
                source.inLines.Remove(line);
                if (source.IsDirect)
                {
                    source.nextChat = null;
                }
                else
                {
                    source.paths.Remove(pair.Item1);
                }
                mainCanvas.Children.Remove(line);
            }
            mainCanvas.Children.Remove(Display);
            MainWindow.displays.Remove(this);
        }

        public Chat BuildChat()
        {
            Chat chat = new Chat()
            {
                name = Name,
                Message = Message.Text,
                exit = IsExit
            };
            if (!IsExit)
            {
                if (IsDirect)
                {
                    chat.nextStep = nextChat.Name;
                    chat.options = null;
                }
                else
                {
                    foreach (KeyValuePair<TextBox, ChatTreeDisplay> option in paths)
                    {
                        chat.options.Add(new Option()
                        {
                            Message = option.Key.Text,
                            nextStep = option.Value.Name
                        });
                    }
                }
            } else
            {
                chat.options = null;
            }
            return chat;
        }
    }
}
