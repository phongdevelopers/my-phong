using System.Collections;
using System.IO;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Exception;

namespace CommerceBuilder.Messaging
{
    /// <summary>
    /// Utility class for supporting usage of NVelocity
    /// </summary>
    [System.CLSCompliant(false)]
    public sealed class NVelocityEngine : VelocityEngine
    {
        private static NVelocityEngine instance = new NVelocityEngine();

        /// <summary>
        /// Private constructor to initialize the NVelocityEngine instance
        /// </summary>
        private NVelocityEngine()
        {
            this.Init();
        }

        /// <summary>
        /// Returns the initialized instance of the NVelocityEngine
        /// </summary>
        public static NVelocityEngine Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Processes the given template using the given parameters
        /// </summary>
        /// <param name="parameters">The parameters to use when processing the template</param>
        /// <param name="template">The template to process</param>
        /// <returns>Result of the template processing</returns>
        public string Process(Hashtable parameters, string template)
        {
            string retVal = string.Empty;
            using (StringWriter writer = new StringWriter())
            {
                
                using (StringReader templateReader = new StringReader(template))
                {
                    this.Evaluate(new VelocityContext(parameters), writer, "AbleCommerce", templateReader);
                }
                
                retVal = writer.ToString();
            }
            return retVal;
        }
    }
}