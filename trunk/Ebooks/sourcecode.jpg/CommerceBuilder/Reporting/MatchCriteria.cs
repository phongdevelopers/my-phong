using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class representing a matching criteria
    /// </summary>
    [Serializable]
    public class MatchCriteria
    {        
        private string _FieldName;
        private string _FieldValue;

        /// <summary>
        /// Name of the field to match
        /// </summary>
        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                _FieldName = value;
            }
        }

        /// <summary>
        /// Value of the field to match
        /// </summary>
        public string FieldValue
        {
            get
            {
                return _FieldValue;
            }
            set
            {
                _FieldValue = value;
            }
        }

    }
}
