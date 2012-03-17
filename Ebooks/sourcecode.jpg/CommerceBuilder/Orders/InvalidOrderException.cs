using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    //TODO ??? why extend from System.Exception? 
    //Why not CommmerceBuilder.Exceptions.CommerceBuilderException
    /// <summary>
    /// Invalid order exception
    /// </summary>
    public class InvalidOrderException : System.Exception
    {
    }
}
