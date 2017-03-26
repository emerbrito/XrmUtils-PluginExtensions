using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    /// <summary>
    /// Describes the messages supported by the plug-in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {

        private string[] _supportedMessages;

        /// <summary>
        /// Gets the supported messages.
        /// </summary>
        /// <value>
        /// The supported messages.
        /// </value>
        public string[] SupportedMessages
        {
            get { return _supportedMessages; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute"/> class.
        /// </summary>
        /// <param name="supportedMessages">The supported pipeline messages .</param>
        public MessageAttribute(params string[] supportedMessages)
        {

            if (supportedMessages == null || supportedMessages.Length == 0)
                throw new ArgumentNullException(nameof(supportedMessages));

            _supportedMessages = supportedMessages.ToArray();

        }

    }
}
