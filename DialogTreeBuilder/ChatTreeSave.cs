using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace DialogTreeBuilder
{
    [XmlRoot("Data")]
    public class ChatTreeSave
    {
        [XmlArray("Positions")]
        [XmlArrayItem("Position")]
        public List<DataPoint> points = new List<DataPoint>();
        public ChatTreeContainer Container { get; set; }
    }

    public class DataPoint
    {
        [XmlAttribute("X1")]
        public double X;
        [XmlAttribute("Y1")]
        public double Y;
        [XmlAttribute("X2")]
        public string name;
    }
}
