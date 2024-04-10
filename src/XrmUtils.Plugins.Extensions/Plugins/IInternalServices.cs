using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    public interface IInternalServices
    {

        IOrganizationService OrganizationService { get; }

        IOrganizationService SystemOrganizationService { get; }

        ITracingService TracingService { get; }

        IExecutionContext InnerExecutionContext { get; }
                

    }
}
