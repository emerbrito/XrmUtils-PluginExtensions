using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{

    /// <summary>
    /// Base implementation for a plug-in. 
    /// </summary>
    public abstract partial class PluginBase : IPlugin
    {

        private ITracingService customTraceService;

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
        /// Executes plug-in code in response to an event.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public void Execute(IServiceProvider serviceProvider)
        {

            Stopwatch stopWatch = new Stopwatch();
            PluginInternals internals = null;
            LocalPluginContext localContext = null;

            internals = new PluginInternals(serviceProvider, this.GetType(), CreateTracingService);

            internals.TracingService.Trace("Instantiating local context.");
            localContext = new LocalPluginContext(internals);

            localContext.ValidatePluginRegistration();

            internals.TracingService.Trace("Calling execute method from derived class.");

            stopWatch.Start();
            internals.TracingService.Trace("Entering {0}.Execute()", this.GetType().Name);
            Execute(localContext);

            stopWatch.Stop();
            internals.TracingService.Trace("Done with derived class.");

        }

        /// <summary>
        /// Executes plug-in code in response to an event.
        /// </summary>
        /// <param name="localContext">The plug-in local context.</param>
        abstract protected void Execute(LocalPluginContext localContext);

    }
}
