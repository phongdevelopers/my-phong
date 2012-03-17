using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// Enum that defines the type of scriptlets
    /// </summary>
    public enum ScriptletType : int
    {
        /// <summary>
        /// The scriptlet type is unspecified
        /// </summary>
        Unspecified = 0, 
        
        /// <summary>
        /// This is a Layout scriptlet
        /// </summary>
        Layout, 
        
        /// <summary>
        /// This is a header scriptlet
        /// </summary>
        Header, 
        
        /// <summary>
        /// This is a footer scriptlet
        /// </summary>
        Footer, 
        
        /// <summary>
        /// This is a sidebar scriptlet
        /// </summary>
        Sidebar, 
        
        /// <summary>
        /// This is a content scriptlet
        /// </summary>
        Content
    }
}
