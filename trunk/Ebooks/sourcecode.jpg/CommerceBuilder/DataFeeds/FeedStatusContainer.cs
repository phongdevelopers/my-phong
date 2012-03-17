using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Singleton class that holds the feed progress status information
    /// </summary>
    public sealed class FeedStatusContainer : System.Collections.Specialized.NameObjectCollectionBase 
    {
        static FeedStatusContainer instance=null;
        static readonly object padlock = new object();
        
        private ReaderWriterLock _Lock;

        FeedStatusContainer()
            : base()
        {
            _Lock = new ReaderWriterLock();
        }

        /// <summary>
        /// The only instance of FeedProgressStatus
        /// </summary>
        public static FeedStatusContainer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance==null)
                    {
                        instance = new FeedStatusContainer();                        
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void Add (string name, object value)
		{
			_Lock.AcquireWriterLock (-1); 
			try 
			{
				BaseAdd (name, value);
			} 
			finally 
			{
				_Lock.ReleaseWriterLock ();
			}
		}

        /// <summary>
        /// Clears the collection
        /// </summary>
		public void Clear ()
		{
			_Lock.AcquireWriterLock (-1); 
			try 
			{
				BaseClear ();
			} 
			finally 
			{
				_Lock.ReleaseWriterLock ();
			}
		} 

        /// <summary>
        /// Gets value for give key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public object Get (string name)
		{
			object ret = null;

			_Lock.AcquireReaderLock (-1); 
			try 
			{
				ret = BaseGet (name);
			} 
			finally 
			{
				_Lock.ReleaseReaderLock ();
			}

			return ret;
		}

        /// <summary>
        /// Gets value at given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public object Get (int index)
		{
			object ret = null;

			_Lock.AcquireReaderLock (-1); 
			try 
			{
				ret = BaseGet (index);
			} 
			finally 
			{
				_Lock.ReleaseReaderLock ();
			}

			return ret;
		}   

        /// <summary>
        /// Gets key at given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public string GetKey (int index)
		{
			string ret = null;

			_Lock.AcquireReaderLock (-1); 
			try 
			{
				ret = BaseGetKey (index);
			} 
			finally 
			{
				_Lock.ReleaseReaderLock ();
			}

			return ret;
		}      

        /// <summary>
        /// Obtains lock for synchronization
        /// </summary>
		public void Lock ()
		{
			_Lock.AcquireWriterLock (-1);
		}

        /// <summary>
        /// Removes the item with given key
        /// </summary>
        /// <param name="name"></param>
		public void Remove (string name)
		{
			_Lock.AcquireWriterLock (-1); 
			try 
			{
				BaseRemove (name);
			} 
			finally 
			{
				_Lock.ReleaseWriterLock ();
			}      
		}

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
		public void RemoveAll ()
		{
			Clear ();
		}

        /// <summary>
        /// Removes an item from the given index
        /// </summary>
        /// <param name="index"></param>
		public void RemoveAt (int index)
		{
			_Lock.AcquireWriterLock (-1); 
			try 
			{
				BaseRemoveAt (index);
			} 
			finally 
			{
				_Lock.ReleaseWriterLock ();
			}      
		}

        /// <summary>
        /// Sets given name value pair
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public void Set (string name, object value)
		{
			_Lock.AcquireWriterLock (-1); 
			try 
			{
				BaseSet (name, value);
			} 
			finally 
			{
				_Lock.ReleaseWriterLock ();
			}      
		}   

        /// <summary>
        /// Releases the lock
        /// </summary>
		public void UnLock ()
		{
			_Lock.ReleaseWriterLock ();
		}

        /// <summary>
        /// Gets all keys
        /// </summary>
		public string [] AllKeys 
		{
			get 
			{
				string [] ret = null;

				_Lock.AcquireReaderLock (-1); 
				try 
				{
					ret = BaseGetAllKeys ();
				} 
				finally 
				{
					_Lock.ReleaseReaderLock ();
				}     

				return ret;
			}
		}

        /// <summary>
        /// Returns number of items in the collection
        /// </summary>
		public override int Count 
		{
			get 
			{
				int ret = 0;

				_Lock.AcquireReaderLock (-1); 
				try 
				{
					ret = base.Count;
				} 
				finally 
				{
					_Lock.ReleaseReaderLock ();
				}     

				return ret;
			}
		}   

        /// <summary>
        /// Gets or sets value for given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public object this [string name] 
		{
			get { return Get (name); }
			set { Set (name, value); }
		}

        /// <summary>
        /// Gets or set value for given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public object this [int index] 
		{
			get { return Get (index); }
		}
    }
}
