using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{

    /// <summary>
    /// Represents the attributes added to the plug-in class.
    /// </summary>
    /// <seealso cref="MessageAttribute"/>
    /// <seealso cref="ExecutionModeAttribute"/>
    /// <seealso cref="StageAttribute"/>
    /// <seealso cref="PrimaryEntityAttribute"/>
    public class PluginAttributesCollection
    {

        #region Private Declarations

        private List<Attribute> _allAttributes;

        #endregion

        #region Public Properties

        public IEnumerable<Attribute> AllAttributes { get { return _allAttributes; } }

        public string[] MessageAttributes { get; private set; }

        public ExecutionMode[] ExecutionModeAttributes { get; private set; }

        public PipelineStage[] StageAttributes { get; private set; }

        public string[] PrimaryEntityAttributes { get; private set; }

        #endregion

        #region Constructors

        internal PluginAttributesCollection(Type pluginType)
        {

            if(pluginType == null)
            {
                throw new ArgumentNullException(Resources.Messages.ArgumentNull, nameof(pluginType));
            }

            _allAttributes = new List<Attribute>();

            LoadAttributesFromType(pluginType);

        }

        #endregion

        #region Private Implementation

        /// <summary>
        /// Loads attributes from type.
        /// </summary>
        /// <param name="pluginType">Plug-in type.</param>
        private void LoadAttributesFromType(Type pluginType)
        {

            MessageAttribute pm = (MessageAttribute)Attribute.GetCustomAttribute(pluginType, typeof(MessageAttribute));
            if (pm != null)
            {
                _allAttributes.Add(pm);
                MessageAttributes = pm.SupportedMessages;
            }

            ExecutionModeAttribute psm = (ExecutionModeAttribute)Attribute.GetCustomAttribute(pluginType, typeof(ExecutionModeAttribute));
            if (psm != null)
            {
                _allAttributes.Add(psm);
                ExecutionModeAttributes = psm.SupportedModes;
            }

            StageAttribute pst = (StageAttribute)Attribute.GetCustomAttribute(pluginType, typeof(StageAttribute));
            if (pst != null)
            {
                _allAttributes.Add(pst);
                StageAttributes = pst.SupportedStages;
            }

            PrimaryEntityAttribute pe = (PrimaryEntityAttribute)Attribute.GetCustomAttribute(pluginType, typeof(PrimaryEntityAttribute));
            if (pe != null)
            {
                _allAttributes.Add(pe);
                PrimaryEntityAttributes = pe.SupportedEntities;
            }

        }

        #endregion

    }

}
