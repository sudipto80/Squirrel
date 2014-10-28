using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    public sealed class SmartDefaults
    {
        static readonly SmartDefaults _instance = new SmartDefaults();
        public static Dictionary<string, List<string>> DefaultValues = new Dictionary<string, List<string>>();
        public static SmartDefaults Instance
        {
            get
            {
                return _instance;
            }
        }
        SmartDefaults()
        {
            // Initialize.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public KeyValuePair<bool,string> DoesMatchingEntryExist(List<string> values)
        {
            foreach (string key in DefaultValues.Keys)
            {
                if (DefaultValues[key].All(t => values.Contains(t)))
                    return new KeyValuePair<bool,string>(true,key);
            }
            return  new KeyValuePair<bool,string>(false,string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetSmartDefaultValues()
        {
            if (DefaultValues.Count == 0)
            {
                XDocument doc = XDocument.Load(@"..\..\..\TableAPI\Data\SmartDefaults.xml");
                List<XElement> nodes = doc.Root.Descendants().ToList();
                foreach (var node in nodes)
                    DefaultValues.Add(node.FirstAttribute.Value, node.LastAttribute.Value.Split(',').ToList());
            }
            return DefaultValues;
        }
    }
}
