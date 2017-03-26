using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    /// <summary>
    /// /// Describes the pipeline stages supported by the plug-in. See <see cref="XrmUtils.Extensions.PipelineStage"/> enumeration for available pipeline stages.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class StageAttribute : Attribute
    {

        private PipelineStage[] _supportedStages;

        /// <summary>
        /// Gets the supported pipeline stages.
        /// </summary>
        /// <value>
        /// The supported pipeline stages.
        /// </value>
        public PipelineStage[] SupportedStages
        {
            get { return _supportedStages; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StageAttribute"/> class.
        /// </summary>
        /// <param name="supportedStages">The supported pipeline stages.</param>
        public StageAttribute(params PipelineStage[] supportedStages)
        {

            if (supportedStages == null || supportedStages.Length == 0)
                throw new ArgumentNullException(nameof(supportedStages));

            _supportedStages = supportedStages.ToArray();

        }


    }
}
