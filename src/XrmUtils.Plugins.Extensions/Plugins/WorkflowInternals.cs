using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace XrmUtils.Extensions.Plugins
{
    public class WorkflowInternals : IWorkflowInternals
    {

        public IWorkflowContext WorkflowContext { get; internal set; }

        public CodeActivityContext CodeActivityContext { get; internal set; }

        public IOrganizationService OrganizationService { get; internal set; }

        public ITracingService TracingService { get; internal set; }

        public IExecutionContext InnerExecutionContext { get { return WorkflowContext; } }

        internal WorkflowInternals(CodeActivityContext executionContext, Type activityType, Func<ITracingService, ITracingService> customTrace)
        {

            ITracingService crmTracing;

            if (executionContext == null)
            {
                throw new ArgumentNullException(nameof(executionContext));
            }

            CodeActivityContext = executionContext;

            // Create the tracing service
            crmTracing = executionContext.GetExtension<ITracingService>();
            if (crmTracing == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            if (customTrace != null)
            {
                crmTracing.Trace("Attempting to create custom tracing service.");
                TracingService = customTrace(crmTracing);

                if (TracingService == null)
                {
                    throw new InvalidPluginExecutionException("Failed to instantiate custom tracing service.");
                }
            }

            TracingService.Trace("Entered {0}.Execute(), Activity Instance Id: {1}, Workflow Instance Id: {2}",
                activityType.FullName,
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            WorkflowContext = executionContext.GetExtension<IWorkflowContext>();
            if (WorkflowContext == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            TracingService.Trace("{0}.Execute(), Correlation Id: {1}, Initiating User: {2}",
                activityType.FullName,
                WorkflowContext.CorrelationId,
                WorkflowContext.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            OrganizationService = serviceFactory.CreateOrganizationService(WorkflowContext.UserId);

        }

    }
}
