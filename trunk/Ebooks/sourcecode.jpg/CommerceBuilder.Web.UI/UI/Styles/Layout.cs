using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.UI.Styles
{
    public class Layout
    {
        private string _Name;
        private string _MasterPageFile;

        public string Name
        {
            get { return _Name; }
        }

        public string MasterPageFile
        {
            get { return _MasterPageFile; }
        }

        public Layout(string name, string masterPageFile)
        {
            _Name = name;
            _MasterPageFile = masterPageFile;
        }
    }
}