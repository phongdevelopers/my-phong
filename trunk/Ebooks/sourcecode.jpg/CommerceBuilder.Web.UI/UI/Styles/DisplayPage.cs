using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Catalog;

namespace CommerceBuilder.UI.Styles
{
    public class DisplayPage
    {
        private string _DisplayPageFile;
        private string _Name;
        private CatalogNodeType _NodeType;
        private string _Description;

        public string DisplayPageFile
        {
            get { return _DisplayPageFile; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public CatalogNodeType NodeType
        {
            get { return _NodeType; }
        }

        public string Description
        {
            get { return _Description; }
        }

        public DisplayPage(string displayPageFile, string name, CatalogNodeType nodeType, string description)
        {
            _DisplayPageFile = displayPageFile;
            _Name = name;
            _NodeType = nodeType;
            _Description = description;
        }
    }
}