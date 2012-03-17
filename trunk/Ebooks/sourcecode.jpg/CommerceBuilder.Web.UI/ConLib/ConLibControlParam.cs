using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Web.ConLib
{
    [Serializable]
    public class ConLibControlParam
    {
        private string _Name;
        private string _DefaultValue;
        private string _Summary;
        public string Name { get { return _Name; } }
        public string DefaultValue { get { return _DefaultValue; } }
        public string Summary { get { return _Summary; } }
        public ConLibControlParam(string name, string defaultValue, string summary)
        {
            _Name = name;
            _DefaultValue = defaultValue;
            _Summary = summary;
        }
    }
}
