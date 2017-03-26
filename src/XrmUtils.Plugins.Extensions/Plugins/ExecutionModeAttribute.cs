using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    /// <summary>
    /// Describes execution mode supported by the plug-in. See <see cref="XrmUtils.Extensions.ExecutionMode"/> enumeration for available execution modes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ExecutionModeAttribute : Attribute
    {

        private ExecutionMode[] _supportedModes;

        /// <summary>
        /// Gets the supported execution modes.
        /// </summary>
        /// <value>
        /// The supported execution modes.
        /// </value>
        public ExecutionMode[] SupportedModes
        {
            get { return _supportedModes; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionModeAttribute"/> class.
        /// </summary>
        /// <param name="supportedModes">The supported execution modes.</param>
        public ExecutionModeAttribute(params ExecutionMode[] supportedModes)
        {

            if (supportedModes == null || supportedModes.Length == 0)
                throw new ArgumentNullException(nameof(supportedModes));

            _supportedModes = supportedModes.ToArray();

        }


    }
}
