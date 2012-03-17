using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using System.IO;

namespace CommerceBuilder.Web.ConLib
{
    public static class ConLibDataSource
    {
        public static ConLibControlCollection Load()
        {
            ConLibControlCollection controls = new ConLibControlCollection();
            InternalLoad(controls, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConLib"), string.Empty);
            controls.Sort(new ConLibComparer());
            return controls;
        }

        private static void InternalLoad(ConLibControlCollection controls, string folder, string prefix)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            if (di.Exists)
            {
                string filePrefix = prefix;
                if (filePrefix.Length > 0) filePrefix += "\\";
                DirectoryInfo[] subDirs = di.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    InternalLoad(controls, subDir.FullName, filePrefix + subDir.Name);
                }
                FileInfo[] files = di.GetFiles("*.ascx");
                foreach (FileInfo file in files)
                {
                    controls.Add(new ConLibControl(filePrefix + file.Name.Substring(0, file.Name.Length - 5), file.FullName));
                }
            }
        }

        private class ConLibComparer : System.Collections.IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                ConLibControl cX = (ConLibControl)x;
                ConLibControl cY = (ConLibControl)y;
                string nameX = cX.Name.ToLowerInvariant();
                string nameY = cY.Name.ToLowerInvariant();
                bool customX = nameX.StartsWith("custom\\");
                bool customY = nameY.StartsWith("custom\\");
                if (customX)
                {
                    if (customY) return nameX.CompareTo(nameY);
                    //X IS CUSTOM, Y IS NOT.  X MUST FOLLOW Y
                    return 1;
                }
                else if (customY)
                {
                    //X IS NOT CUSTOM, Y IS.  Y MUST FOLLOW X
                    return -1;
                }
                else
                {
                    return nameX.CompareTo(nameY);
                }
            }

            #endregion
        }

    }
}
