using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DialogTreeBuilder
{
    [XmlRoot("NPC")]
    public class ChatTreeContainer
    {
        [XmlArray("Chats")]
        [XmlArrayItem("Chat")]
        public List<Chat> chatTree = new List<Chat>();

        public Chat GetEnterChat()
        {
            return chatTree.First(e => e.name == "enter");
        }

        public Chat GetChatByOption(Option option)
        {
            return chatTree.FirstOrDefault(e => e.name == option.nextStep);
        }
    }
    public class Chat
    {
        private string message;
        [XmlAttribute("name")]
        public string name;
        [XmlText]
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value.Trim();
            }
        }
        [XmlAttribute("tag")]
        public string tag;
        [XmlAttribute("goto")]
        public string nextStep;
        [XmlAttribute("exit")]
        public bool exit;
        [XmlArray("Options")]
        [XmlArrayItem("Option")]
        public List<Option> options = new List<Option>();
    }

    public class Option
    {
        private string message;
        [XmlAttribute("goto")]
        public string nextStep;
        [XmlText]
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value.Trim();
            }
        }
    }
}
