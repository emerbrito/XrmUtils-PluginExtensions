using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    public abstract class WorkflowActivityBase : CodeActivity
    {

        /// <summary>
        /// Override this method if you want to use a custom tracing service.
        /// </summary>
        /// <param name="crmTracingService">An instance of the inner CRM tracing service.</param>
        /// <returns></returns>
        protected virtual ITracingService CreateTracingService(ITracingService crmTracingService)
        {
            return crmTracingService;
        }

        /// <summary>
        /// Performs the execution of the custom activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {

            Stopwatch stopWatch = new Stopwatch();
            WorkflowInternals internals = null;
            LocalWorkflowContext localContext = null;

            internals = new WorkflowInternals(executionContext, this.GetType(), CreateTracingService);

            internals.TracingService.Trace("Instantiating local context.");
            localContext = new LocalWorkflowContext(internals);

            internals.TracingService.Trace("Calling Execute() method from derived class.");

            stopWatch.Start();
            Execute(localContext);
            stopWatch.Stop();
            internals.TracingService.Trace("Done with derived class.");

        }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the custom activity.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        abstract protected void Execute(LocalWorkflowContext localContext);

    }

}
