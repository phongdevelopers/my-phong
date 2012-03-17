using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Common
{
    public class IntegerList : Collection<int>
    {
        public void AddRange(string list)
        {
            int[] items = AlwaysConvert.ToIntArray(list);
            if (items != null)
            {
                this.AddRange(items);
            }
        }

        public void AddRange(IEnumerable<int> items)
        {
            foreach (int item in items)
            {
                this.Add(item);
            }
        }

        public IntegerList() { }

        public IntegerList(string list)
        {
            this.AddRange(list);
        }

        public override string ToString()
        {
            return AlwaysConvert.ToList(",", this);
        }
    }
}