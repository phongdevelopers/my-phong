using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace CommerceBuilder.Utility
{
    public class SessionFacade
    {
        // IF NO HTTPCONTEXT IS PRESENT, WE NEED A PLACE TO REFER TO FOR VARIABLE STORAGE
        // THIS DATA WILL HAVE NO PERSISTENCE OUTSIDE OF THE CURRENT THREAD
        private Dictionary<string, object> _InternalStorage = new Dictionary<string,object>();

        /// <summary>
        /// Gets or sets a collection of order ids that have been selected by the user 
        /// for some operation.
        /// </summary>
        public List<int> SelectedOrderIds
        {
            get
            {
                List<int> selectedOrderIds = GetDataFromSession(SessionKeys.SelectedOrderIds) as List<int>;
                if (selectedOrderIds == null)
                {
                    selectedOrderIds = new List<int>();
                }
                return selectedOrderIds;
            }
            set
            {
                if (value == null)
                {
                    RemoveDataFromSession(SessionKeys.SelectedOrderIds);
                }
                else
                {
                    SetDataInSession(SessionKeys.SelectedOrderIds, value);
                }
            }
        }

        #region Access Session

        private void SetDataInSession(string key, object value)
        {
            HttpSessionState session = HttpContextHelper.SafeGetSession();
            if (session != null)
            {
                session[key] = value;
            }
            else
            {
                _InternalStorage[key] = value;
            }
        }

        private void RemoveDataFromSession(string key)
        {
            HttpSessionState session = HttpContextHelper.SafeGetSession();
            if (session != null)
            {
                session.Remove(key);
            }
            else if (_InternalStorage.ContainsKey(key))
            {
                _InternalStorage.Remove(key);
            }
        }

        private object GetDataFromSession(string key)
        {
            HttpSessionState session = HttpContextHelper.SafeGetSession();
            if (session != null)
            {
                return session[key];
            }
            else if (_InternalStorage.ContainsKey(key))
            {
                return _InternalStorage[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        private static class SessionKeys
        {
            public const string SelectedOrderIds = "1a28f4ee69394935a02d77319144cc71";
        }
    }
}
