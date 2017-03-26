using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    public class LocalWorkflowContext : ContextBase
    {

        #region Private Declarations

        private IWorkflowInternals Internals { get; set; }

        /// <summary>
        /// Gets an instance of the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        public CodeActivityContext ExecutionContext { get { return Internals.CodeActivityContext; } }

        /// <summary>
        /// Gets an instance of the workflow context.
        /// </summary>
        /// <value>
        /// The workflow context.
        /// </value>
        public IWorkflowContext WorkflowContext { get { return Internals.WorkflowContext; } }

        #endregion

        #region Public Properties

        #endregion


        #region Constructors

        public LocalWorkflowContext(IWorkflowInternals internals) : base(internals)
        {

            Internals = internals;
        }

        #endregion

        #region Public Methods

        #endregion

    }
}
