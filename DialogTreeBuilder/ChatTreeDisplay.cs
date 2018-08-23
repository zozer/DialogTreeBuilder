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
    public class ChatTreeDisplay
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
        public bool IsDirect { get; private set; }
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
            IsDirect = true;
            IsExit = false;
            CreateNewDialogStep();
        }

        public ChatTreeDisplay(Canvas canvas, Chat chat)
        {
            mainCanvas = canvas;
            IsFirst = chat.name == "enter";
            IsDirect = chat?.options.Count == 0;
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
                Name = (chat!=null)?chat.name:(IsFirst) ? "enter" : $"step{count++}",
                Text = (chat != null)?chat.Message:"new dialog",
                Width = 100,
                Height = 30,
                Background = Brushes.LightSteelBlue
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
                Fill = (chat != null && chat.exit)?Brushes.DarkRed:(IsFirst)?Brushes.DarkGreen:Brushes.DarkBlue
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
                IsChecked = (chat != null)?chat.exit:false,
                Height = 10,
                Width = 10
            };

            newLabel.GotKeyboardFocus += NewLabel_GotFocus;
            newLabel.LostKeyboardFocus += NewLabel_LostFocus;
            AddButton.MouseDown += AddButton_MouseDown;
            MoveBar.MouseMove += MoveBar_MouseMove;
            MoveBar.MouseLeftButtonDown += MoveBar_MouseLeftButtonDown;
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
            } else
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

            Rectangle orgin = e.Source as Rectangle;
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
                } else
                {
                    pair.Item2.paths[pair.Item1] = this;
                }
                createdLine.Tag = null;
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

        private void AddButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse button = e.Source as Ellipse;
            AddOption(button);
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
                IsDirect = false;
            }

            Canvas area = button.Parent as Canvas;
            TextBox newLabel = new TextBox
            {
                Text = (option != null)?option.Message:"new response",
                Width = 100,
                Height = 30,
                Background = Brushes.DarkSlateBlue
            };
            if (option != null)
            {
                newLabel.Tag = option;
            }

            Rectangle nextChatButton = new Rectangle()
            {
                Name = $"NextStep{paths.Count+1}",
                Width = 10,
                Height = 10,
                Fill = Brushes.Black,
                Tag = newLabel
            };

            nextChatButton.MouseLeftButtonDown += NextChatButton_MouseLeftButtonDown;
            newLabel.GotKeyboardFocus += NewLabel_GotFocus;
            newLabel.LostKeyboardFocus += NewLabel_LostFocus;
            double height = Canvas.GetTop(button);
            Canvas.SetTop(button, height + 30);
            Canvas.SetLeft(nextChatButton, 110);
            Canvas.SetTop(nextChatButton, height + 17.5);

            Canvas.SetLeft(newLabel, 0);
            Canvas.SetTop(newLabel, height + 7.5);
            area.Children.Add(nextChatButton);
            Panel.SetZIndex(button, area.Children.Add(newLabel));

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
