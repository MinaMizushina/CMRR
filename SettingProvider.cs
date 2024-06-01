using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mina.Tools.CMRR
{
    internal class SettingProvider
    {
        internal struct SettingItem
        {
            public string Name;
            public string Match;
            public string replaceTo;
            public int notifySeconds;
            public bool goNext;
        }

        private List<SettingItem> settings;

        internal List<SettingItem> Settings
        { 
            get { return settings; }
        }

        internal SettingProvider()
        {
            XmlDocument settingDocument = new XmlDocument();
            settingDocument.Load(GetSettingFilename());

            settings = new List<SettingItem>();

            foreach(XmlElement el in settingDocument.SelectNodes(Resources.XPATH_SETTING))
            {
                var item = new SettingItem();

                item.Name = el.GetAttribute(Resources.XML_ATTR_NAME);
                item.Match = el.GetAttribute(Resources.XML_ATTR_MATCH);
                item.replaceTo = el.GetAttribute(Resources.XML_ATTR_REPLACE_TO);
                item.notifySeconds = int.Parse(el.GetAttribute(Resources.XML_ATTR_NOTIFYSECONDS));
                item.goNext = el.GetAttribute(Resources.XML_ATTR_GONEXT).ToLower().Trim().Equals("true");

                settings.Add(item);
            }
        }


        private string GetSettingFilename()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Resources.XML_SETTING_FILENAME);

        }
    }
}
