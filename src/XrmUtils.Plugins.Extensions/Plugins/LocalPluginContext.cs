using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{

    /// <summary>
    /// Defines the contextual information passed to a plug-in at run-time. Contains information that describes the run-time environment that the plug-in is executing in, information related to the execution pipeline, and entity business information. Also includes a series of wrappers for productivity.
    /// </summary>
    public class LocalPluginContext : ContextBase
    {

        #region Private Declarations

        PluginAttributesCollection _customAttributes;

        private IPluginInternals Internals { get; set; }

        private PluginAttributesCollection CustomAttributes
        {
            get
            {
                if(_customAttributes == null)
                {
                    _customAttributes = new PluginAttributesCollection(Internals.PluginType);
                }
                return _customAttributes;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the plug-in execution context. A shortcut to <see cref="Internals.ExecutionContext"/>.
        /// </summary>
        /// <value>
        /// The plug-in context.
        /// </value>
        public IPluginExecutionContext PluginExecutionContext { get; private set; }

        /// <summary>
        /// Gets the current pipeline stage.
        /// </summary>
        /// <value>
        /// The pipeline stage.
        /// </value>
        public PipelineStage Stage { get; private set; }

        #endregion

        #region Constructors

        public LocalPluginContext(IPluginInternals internals)
            : base(internals)
        {

            PluginExecutionContext = internals.ExecutionContext;
            Stage = (PipelineStage)internals.ExecutionContext.Stage;
            Internals = internals;

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validate current pipeline stage against a list of supported values and returns true if at least one match is found.
        /// </summary>
        /// <param name="supportedStages">Supported values</param>
        /// <returns>Boolean value indicating whether or not it passed the validation</returns>
        public bool IsSupportedPipelineStage(params PipelineStage[] supportedStages)
        {

            if (supportedStages == null || supportedStages.Length == 0)
            {
                throw new ArgumentNullException(nameof(supportedStages), string.Format(Resources.Messages.ArgumentNull, nameof(supportedStages)));
            }

            bool isValid = false;

            if (supportedStages.Contains(Stage))
            {
                isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Validate current pipleine stage against a list of supported values and throws an InvalidPluginExecutionException if it fails.
        /// </summary>
        /// <param name="supportedStages">Supported values</param>
        public void AssertSupportedPipelineStage(params PipelineStage[] supportedStages)
        {

            if (!IsSupportedPipelineStage(supportedStages))
            {

                var flatMsgs = string.Format(Resources.Messages.PipelineStageNotSupported,
                    ((ExecutionMode)InnerExecutionContext.Mode).ToString(),
                    string.Join(", ", supportedStages.Select(m => m.ToString()).ToArray())
                    );

                throw new InvalidPluginExecutionException(flatMsgs);

            }

        }

        /// <summary>
        /// Validates the plugin registration against expected values found on attributes added to the plugin class.
        /// Throws an <see cref="InvalidPluginExecutionException"/> if validation fails.
        /// </summary>
        public void ValidatePluginRegistration()
        {

            if(CustomAttributes.AllAttributes.Count() == 0)
            {
                return;
            }

            if(CustomAttributes.ExecutionModeAttributes.Any())
            {
                AssertSupportedExecutionMode(CustomAttributes.ExecutionModeAttributes);
            }

            if(CustomAttributes.MessageAttributes.Any())
            {
                AssertSupportedMessage(CustomAttributes.MessageAttributes);
            }

            if(CustomAttributes.PrimaryEntityAttributes.Any())
            {
                AssertSupportedEntities(CustomAttributes.PrimaryEntityAttributes);
            }

            if(CustomAttributes.StageAttributes.Any())
            {
                AssertSupportedPipelineStage(CustomAttributes.StageAttributes);
            }

        }

        #endregion

    }
}
