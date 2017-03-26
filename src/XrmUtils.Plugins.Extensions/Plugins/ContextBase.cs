using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    public abstract class ContextBase
    {

        #region Private Declarations    
        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the inner execution context.
        /// </summary>
        /// <value>
        /// The inner execution context.
        /// </value>
        internal IExecutionContext InnerExecutionContext { get; private set; }

        #endregion

        #region Public Declarations

        /// <summary>
        /// Gets the inner organization service.
        /// </summary>
        /// <value>
        /// The organization service.
        /// </value>
        public IOrganizationService OrganizationService { get; internal set; }

        /// <summary>
        /// Gets the inner tracing service.
        /// </summary>
        /// <value>
        /// The local tracing service.
        /// </value>
        public ITracingService TracingService { get; internal set; }

        /// <summary>
        /// Gets the current pipeline message.
        /// </summary>
        /// <value>
        /// The name of thecurrent pipeline message.
        /// </value>
        public string Message
        {
            get
            {
                var value = InnerExecutionContext.MessageName;
                return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
            private set { }
        }

        /// <summary>
        /// Gets the current execution mode.
        /// </summary>
        /// <remarks>
        /// Allowable values are defined in the <see cref="XrmUtils.Extensions.ExecutionMode"/> enumeration.
        /// </remarks>
        public ExecutionMode PipelineExecutionMode
        {
            get
            {
                return (ExecutionMode)InnerExecutionContext.Mode;
            }
            private set { }
        }

        /// <summary>
        /// The target type.
        /// </summary>
        public ContextTargetType TargetType
        {
            get
            {

                var value = ContextTargetType.Unknown;
                var key = "Target";

                if (!InnerExecutionContext.InputParameters.ContainsKey(key))
                {
                    value = ContextTargetType.None;
                }
                else
                {

                    if (InnerExecutionContext.InputParameters["Target"] is Entity)
                    {
                        value = ContextTargetType.Entity;
                    }
                    else if (InnerExecutionContext.InputParameters["Target"] is EntityReference)
                    {
                        value = ContextTargetType.EntityReference;
                    }

                }

                return value;

            }
        }

        /// <summary>
        /// Gets the name of the target entity for which the pipeline is processing events.
        /// </summary>
        public string PrimaryEntityName
        {
            get
            {
                return InnerExecutionContext.PrimaryEntityName;
            }
        }

        /// <summary>
        /// Gets the GUID of the target entity for which the pipeline is processing events.
        /// </summary>
        public Guid PrimaryEntityId
        {
            get
            {
                return InnerExecutionContext.PrimaryEntityId;
            }
        }

        #endregion

        #region Constructors

        public ContextBase(IInternalServices internals)
        {

            internals.TracingService.Trace($"Entering {nameof(ContextBase)}");

            InnerExecutionContext = internals.InnerExecutionContext;
            OrganizationService = internals.OrganizationService;

            TracingService = internals.TracingService;

            internals.TracingService.Trace($"Initialization complete {nameof(ContextBase)}");

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validate the primary entity against a list of supported values and returns true if at least one match is found. Case insensitive.
        /// </summary>
        /// <param name="supportedEntities">Supported entities.</param>
        public bool IsSupportedEntity(params string[] supportedEntities)
        {

            if (supportedEntities == null || supportedEntities.Length == 0)
            {
                throw new ArgumentNullException(nameof(supportedEntities), string.Format(Resources.Messages.ArgumentNull, nameof(supportedEntities)));
            }

            var isValid = false;
            var curEntity = InnerExecutionContext.PrimaryEntityName;

            if (supportedEntities.Any(m => string.Equals(curEntity, m, StringComparison.CurrentCultureIgnoreCase)))
            {
                isValid = true;
            }

            return isValid;

        }

        /// <summary>
        /// Validate primary entity againts a list of supported values and throws an InvalidPluginExecutionException if it fails.
        /// </summary>
        /// <param name="supportedEntities">Supported entities.</param>
        public void AssertSupportedEntities(params string[] supportedEntities)
        {

            if (!IsSupportedEntity(supportedEntities))
            {

                var flatMsgs = string.Format(Resources.Messages.PrimaryEntityNotSupported,
                    InnerExecutionContext.PrimaryEntityName,
                    string.Join(", ", supportedEntities));

                throw new InvalidPluginExecutionException(flatMsgs);
            }

        }

        /// <summary>
        /// Validate current pipeline message against a list of supported values and returns true if at least one match is found. Case insensitive.
        /// </summary>
        /// <param name="supportedMessages">Supported messages</param>
        public bool IsSupportedMessage(params string[] supportedMessages)
        {

            if(supportedMessages == null || supportedMessages.Length == 0)
            {
                throw new ArgumentNullException(nameof(supportedMessages), string.Format(Resources.Messages.ArgumentNull, nameof(supportedMessages)));
            }

            var isValid = false;
            var curMsg = InnerExecutionContext.MessageName;

            if (supportedMessages.Any(m => string.Equals(curMsg, m, StringComparison.CurrentCultureIgnoreCase)))
            {
                isValid = true;
            }

            return isValid;

        }

        /// <summary>
        /// Validate current message against a list of supported values and throws an InvalidPluginExecutionException if it fails.
        /// </summary>
        /// <param name="supportedEntities">Supported messages</param>
        public void AssertSupportedMessage(params string[] supportedMessages)
        {

            if (!IsSupportedMessage(supportedMessages))
            {

                var flatMsgs = string.Format(Resources.Messages.MessageNotSupported,
                    InnerExecutionContext.MessageName,
                    string.Join(", ", supportedMessages));

                throw new InvalidPluginExecutionException(flatMsgs);
            }

        }

        /// <summary>
        /// Validate current execution mode against a list of supported values and returns true if at least one match is found.
        /// </summary>
        /// <param name="supportedModes">Supported values</param>
        /// <returns>Boolean value indicating whether or not it passed the validation.</returns>
        public bool IsSupportedExecutionMode(params ExecutionMode[] supportedModes)
        {

            if (supportedModes == null || supportedModes.Length == 0)
            {
                throw new ArgumentNullException(nameof(supportedModes), string.Format(Resources.Messages.ArgumentNull, nameof(supportedModes)));
            }

            bool isValid = false;

            if (supportedModes.Contains(PipelineExecutionMode))
            {
                isValid = true;
            }

            return isValid;

        }

        /// <summary>
        /// Validate current execution mode against a list of supported values and throws an InvalidPluginExecutionException if it fails.
        /// </summary>
        /// <param name="supportedModes">Supported values</param>
        public void AssertSupportedExecutionMode(params ExecutionMode[] supportedModes)
        {

            if(!IsSupportedExecutionMode(supportedModes))
            {

                var flatMsgs = string.Format(Resources.Messages.ExecutionModeNotSupported,
                    ((ExecutionMode)InnerExecutionContext.Mode).ToString(),
                    string.Join(", ", supportedModes.Select(m => m.ToString()).ToArray())
                    );

                throw new InvalidPluginExecutionException(flatMsgs);

            }

        }

        /// <summary>
        /// Gets the target entity from the inner execution contex. Returns null if input paramters collection doesn't have a 'Target' entry or if entry is an entity reference.
        /// </summary>
        /// <returns></returns>
        public Entity GetTargetEntity()
        {

            Entity value = null;

            if(TargetType == ContextTargetType.Entity)
            {
                value = (Entity)InnerExecutionContext.InputParameters["Target"];
            }

            return value;

        }

        /// <summary>
        /// Gets the target <see cref="EntityReference"/>. 
        /// An attempt to return it from the input parameters collection will be made. If value is null an <see cref="EntityReference"/> instance will be created based in the primary entity ID and name from context.
        /// </summary>
        /// <returns></returns>
        public EntityReference GetTargetReference()
        {

            EntityReference value = null;

            if (TargetType == ContextTargetType.EntityReference)
            {
                value = (EntityReference)InnerExecutionContext.InputParameters["Target"];
            }
            else if (TargetType == ContextTargetType.Entity)
            {
                value = ((Entity)InnerExecutionContext.InputParameters["Target"]).ToEntityReference();
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(PrimaryEntityName))
                {
                    value = new EntityReference(PrimaryEntityName, PrimaryEntityId);
                }
            }

            return value;

        }

        /// <summary>
        /// Gets an entity pre image.
        /// </summary>
        /// <param name="name">The pre image name.</param>
        /// <param name="throwIfNull">Whether to throw an <see cref="InvalidPluginExecutionException"/> when image cannot be found.</param>
        /// <returns></returns>
        public Entity GetPreImage(string name, bool throwIfNull = false)
        {
            return GetEntityImage(name, ImageType.PreImage, throwIfNull);
        }

        /// <summary>
        /// Gets an entity post image.
        /// </summary>
        /// <param name="name">The post image name.</param>
        /// <param name="throwIfNull">Whether to throw an <see cref="InvalidPluginExecutionException"/> when image cannot be found.</param>
        /// <returns></returns>
        public Entity GetPostImage(string name, bool throwIfNull = false)
        {
            return GetEntityImage(name, ImageType.PostImage, throwIfNull);
        }

        /// <summary>
        /// Writes to the trace service.
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string format, params Object[] args)
        {

            string value = format;

            if (string.IsNullOrWhiteSpace(format) || this.TracingService == null)
            {
                return;
            }
                
            TracingService.Trace(value, args);

        }

        /// <summary>
        /// Determines whether the system user for whom the plug-in invokes web service methods on behalf of, is a system or non interactive user.
        /// </summary>
        /// <returns></returns>
        public bool IsSystemOrNonInteractiveUser()
        {

            /*
             * Known access mode:
             * 
             * 0 : Read-Write
             * 1 : Administrative
             * 2 : Read
             * 3 : Support User
             * 4 : Non-interactive
             * 5 : Delegated Admin 
             * 
             */

            /*
             * SYSTEM and INTEGRATION users
             * these users are disabled as they are reserved so by allowing disabled users 
             */

            Guid uid = InnerExecutionContext.UserId;
            int accessMode = 0;
            bool isdisabled = false;
            bool flag = false;

            if (InnerExecutionContext.SharedVariables.ContainsKey(uid.ToString()))
            {
                flag = (bool)InnerExecutionContext.SharedVariables[uid.ToString()];
            }
            else
            {
                Entity user = OrganizationService.Retrieve("systemuser", uid, new ColumnSet("accessmode", "isdisabled"));
                accessMode = user.GetAttributeValue<OptionSetValue>("accessmode").Value;
                isdisabled = user.GetAttributeValue<bool>("isdisabled");

                if (accessMode == 4 || accessMode == 5 || isdisabled) // non interactive, delegate admin, system, integration
                {
                    flag = true;
                }

                this.InnerExecutionContext.SharedVariables.Add(uid.ToString(), flag);

            }

            return flag;

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets an entity image.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <param name="throwIfNull">Whether to throw an <see cref="InvalidPluginExecutionException"/> when image cannot be found.</param>
        /// <returns></returns>
        /// <exception cref="Microsoft.Xrm.Sdk.InvalidPluginExecutionException"></exception>
        private Entity GetEntityImage(string imageName, ImageType imageType, bool throwIfNull = false)
        {

            if (string.IsNullOrWhiteSpace(imageName))
            {
                throw new ArgumentNullException(nameof(imageName), string.Format(Resources.Messages.ArgumentNull, nameof(imageName)));
            }

            Entity image = null;
            EntityImageCollection images;

            if (imageType == ImageType.PreImage)
            {
                images = InnerExecutionContext.PreEntityImages;
            }
            else
            {
                images = InnerExecutionContext.PostEntityImages;
            }

            if (images.Contains(imageName))
            {
                image = images[imageName];
            }
            else if (throwIfNull)
            {
                throw new InvalidPluginExecutionException(string.Format(Resources.Messages.EntityImageNotFound, imageType.ToString(), imageName));
            }

            return image;

        }

        /// <summary>
        /// Search and retrieves for an specific context in the parent context stack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="primaryEntityName">Name of the primary entity to search for.</param>
        /// <param name="recursive">if set to <c>true</c> search is recursive.</param>
        /// <returns></returns>
        private IExecutionContext RetrieveParentContext(IExecutionContext context, string primaryEntityName, bool recursive)
        {

            IExecutionContext retValue = null;
            IExecutionContext parent = null;

            var interfaces = context.GetType().GetInterfaces();
            if (interfaces.Any(i => i.Equals(typeof(IPluginExecutionContext))))
            {
                parent = ((IPluginExecutionContext)context).ParentContext;
            }
            else if (interfaces.Any(i => i.Equals(typeof(IWorkflowContext))))
            {
                parent = ((IWorkflowContext)context).ParentContext;
            }

            if (parent == null)
                return retValue;

            if (parent.PrimaryEntityName == primaryEntityName)
                return parent;

            if (recursive)
                retValue = RetrieveParentContext(parent, primaryEntityName, true);

            return retValue;

        }


        #endregion

    }
}
