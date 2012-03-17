using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// A collection of Scriptlet objects
    /// </summary>
    public class ScriptletCollection : PersistentCollection<Scriptlet>
    {
        /// <summary>
        /// Gets the index of the specified scriptlet in this collection
        /// </summary>
        /// <param name="identifier">Identifier of the scriptlet to find index of</param>
        /// <param name="scriptletType">Type of the scriptlet to find index of</param>
        /// <returns>Index of the specified scriptlet in this collection</returns>
        public int IndexOf(string identifier, ScriptletType scriptletType)
        {
            return IndexOf(identifier, scriptletType, BitFieldState.Any);
        }

        /// <summary>
        /// Gets the index of the specified scriptlet in this collection
        /// </summary>
        /// <param name="identifier">Identifier of the scriptlet to find index of</param>
        /// <param name="scriptletType">Type of the scriptlet to find index of</param>
        /// <param name="isCustom">Indicates whether this a custom scriptlet or default scriptlet or any of the two</param>
        /// <returns>Index of the specified scriptlet in this collection</returns>
        public int IndexOf(string identifier, ScriptletType scriptletType, BitFieldState isCustom)
        {
            bool custom = (isCustom == BitFieldState.True);
            for (int i = 0; i < this.Count; i++)
            {
                Scriptlet s = this[i];
                if ((identifier == s.Identifier) && (scriptletType == s.ScriptletType))
                {
                    if ((isCustom == BitFieldState.Any) || (custom == s.IsCustom)) return i;
                }
            }
            return -1;
        }
    }
}
