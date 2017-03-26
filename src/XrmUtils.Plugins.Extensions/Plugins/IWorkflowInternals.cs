using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    public interface IWorkflowInternals : IInternalServices
    {

        IWorkflowContext WorkflowContext { get; }

        CodeActivityContext CodeActivityContext { get; }

    }
}
