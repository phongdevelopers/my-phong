using System;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// Class representing BannedIP object in the database
    /// </summary>
    public partial class BannedIP
    {
        /// <summary>
        /// Gets a dotted IP range start
        /// </summary>
        public string DottedIPRangeStart
        {
            get { return BannedIP.ConvertToDotted(this.IPRangeStart); }
        }

        /// <summary>
        /// Gets a a dotted IP range end
        /// </summary>
        public string DottedIPRangeEnd
        {
            get { return BannedIP.ConvertToDotted(this.IPRangeEnd); }
        }

        /// <summary>
        /// Determines whether an ip address falls in the range.
        /// </summary>
        /// <param name="ip">The IP address to test.</param>
        /// <returns>True if the IP address is within the range, false otherwise.</returns>
        public bool IsInRange(string ip)
        {
            long ipNumber = ConvertToNumber(ip);
            return (ipNumber >= this.IPRangeStart && ipNumber <= this.IPRangeEnd);
        }

        /// <summary>
        /// Converts a dot-decimal ip string into the equivalent numeric value.
        /// </summary>
        /// <param name="ip">The ip in dot-decimal format (x.x.x.x)</param>
        /// <returns>The numeric value of the ip</returns>
        public static long ConvertToNumber(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return 0;
            string[] segments = ip.Split('.');
            if (segments.Length != 4) return 0;
            double total = 0;
            for (int i = segments.Length - 1; i >= 0; i--)
            {
                total += ((AlwaysConvert.ToInt(segments[i]) % 256) * Math.Pow(256, (3 - i)));
            }
            return (long)total;
        }

        /// <summary>
        /// Converts the long IP number to dotted IP number
        /// </summary>
        /// <param name="ipNumber"></param>
        /// <returns></returns>
        public static string ConvertToDotted(long ipNumber)
        {
            int s1 = (int)((ipNumber / 16777216) % 256);
            int s2 = (int)((ipNumber / 65536) % 256);
            int s3 = (int)((ipNumber / 256) % 256);
            int s4 = (int)(ipNumber % 256);
            return string.Format("{0}.{1}.{2}.{3}", s1, s2, s3, s4);
        }
    }
}
