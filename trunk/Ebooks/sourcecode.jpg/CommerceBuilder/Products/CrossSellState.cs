using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Products
{
    public enum CrossSellState:byte
    {
        Linked,
        CrossLinked,
        LinksTo,
        LinkedFrom,
        Unlinked,
        None
    }
}
