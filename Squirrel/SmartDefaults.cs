﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SmartDefaults
    {
        static readonly SmartDefaults _instance = new SmartDefaults();
        public static Dictionary<string, List<string>> DefaultValues = new Dictionary<string, List<string>>();
        /// <summary>
        /// 
        /// </summary>
        public static SmartDefaults Instance => _instance;

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
        public Dictionary<string, List<string>> GetSmartDefaultValues(string defaultValueFilePath)
        {
            if (DefaultValues.Count == 0)
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                XDocument doc = XDocument.Load(defaultValueFilePath);
                List<XElement> nodes = doc.Root.Descendants().ToList();
                foreach (var node in nodes)
                    DefaultValues.Add(node.FirstAttribute.Value, node.LastAttribute.Value.Split(',').ToList());
            }
            return DefaultValues;
        }
    }
}
