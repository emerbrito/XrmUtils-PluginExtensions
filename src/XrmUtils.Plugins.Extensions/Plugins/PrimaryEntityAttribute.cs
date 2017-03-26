using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{

    /// <summary>
    /// Represents the entity type supported by the plug-in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PrimaryEntityAttribute : Attribute
    {

        private string[] _supportedEntities;

        /// <summary>
        /// Gets the logical name of the supported entity.
        /// </summary>
        /// <value>
        /// The logical name of the supported entity.
        /// </value>
        public string[] SupportedEntities
        {
            get { return _supportedEntities; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryEntityAttribute"/> class.
        /// </summary>
        /// <param name="entityLogicalNames">Logical name of the supported entities.</param>
        public PrimaryEntityAttribute(params string[] entityLogicalNames)
        {

            if (entityLogicalNames == null || entityLogicalNames.Length == 0)
                throw new ArgumentNullException(nameof(entityLogicalNames));

            _supportedEntities = entityLogicalNames.ToArray();

        }

    }

}
