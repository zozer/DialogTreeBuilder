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
        private void Export_Click(object sender, RoutedEventArgs e)
        {
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
                    serializer.Serialize(writer, ExportData());
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChatTreeContainer));
            XmlSerializer saveData = new XmlSerializer(typeof(ChatTreeSave));
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                FileName = "New chat tree save",
                DefaultExt = ".gz",
                Filter = "gz files|*.gz"
            };
            bool? result = saveFile.ShowDialog();
            if (result == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFile.FileName))
                {
                    saveData.Serialize(writer, ExportSave());
                }
                Utils.Compress(new FileInfo(saveFile.FileName));
            }
        }

        private ChatTreeContainer ExportData()
        {
            return new ChatTreeContainer
            {
                chatTree = displays.Select(e => e.BuildChat()).ToList()
            };
        }

        private ChatTreeSave ExportSave()
        {
            return new ChatTreeSave()
            {
                points = displays.Select(e =>
                    new DataPoint()
                    {
                        X = Canvas.GetLeft(e.Display),
                        Y = Canvas.GetTop(e.Display),
                        name = e.Name
                    }
                ).ToList(),
                Container = ExportData()
            };
        }
    }
}
