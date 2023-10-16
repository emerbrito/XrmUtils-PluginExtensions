using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    class PluginInternals : IPluginInternals
    {

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        public IPluginExecutionContext ExecutionContext { get; private set; }

        public IExecutionContext InnerExecutionContext { get { return ExecutionContext; } }

        /// <summary>
        /// Gets the ITracingService.
        /// </summary>
        /// <value>
        /// The tracing service.
        /// </value>
        public ITracingService TracingService { get; private set; }

        /// <summary>
        /// Gets the IOrganizationService.
        /// </summary>
        /// <value>
        /// The IOrganizationService.
        /// </value>
        public IOrganizationService OrganizationService { get; private set; }

        /// <summary>
        /// Gets an elevated IOrganizationService with SYSTEM user permissions.
        /// </summary>
        /// <value>
        /// The IOrganizationService.
        /// </value>
        public IOrganizationService SystemOrganizationService { get; private set; }

        /// <summary>
        /// Synchronous registered plug-ins can post the execution context to the Microsoft Azure Service Bus. <br/> 
        /// It is through this notification service that synchronous plug-ins can send brokered messages to the Microsoft Azure Service Bus.
        /// </summary>
        internal IServiceEndpointNotificationService NotificationService { get; private set; }

        public Type PluginType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInternals"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PluginInternals(IServiceProvider serviceProvider, Type pluginType, Func<ITracingService, ITracingService> customTrace)
        {

            ITracingService crmTracing;

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            
            PluginType = pluginType;

            // Extract the tracing service 
            crmTracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (crmTracing == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            if(customTrace != null)
            {
                crmTracing.Trace("Attempting to create custom tracing service.");
                TracingService = customTrace(crmTracing);

                if(TracingService == null)
                {
                    throw new InvalidPluginExecutionException("Failed to instantiate custom tracing service.");
                }
            }

            TracingService.Trace("Obtaining execution context from service provider.");
            ExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            TracingService.Trace("Correlation Id: {1}, User Id: {2}", this.GetType().Name, ExecutionContext.CorrelationId, ExecutionContext.UserId);
            
            TracingService.Trace("Obtaining service factory reference.");
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            TracingService.Trace("Obtaining organization service reference.");
            OrganizationService = serviceFactory.CreateOrganizationService(ExecutionContext.UserId);

            TracingService.Trace("Obtaining elevated organization service reference.");
            SystemOrganizationService = serviceFactory.CreateOrganizationService(null);

            NotificationService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));

        }

    }
}
