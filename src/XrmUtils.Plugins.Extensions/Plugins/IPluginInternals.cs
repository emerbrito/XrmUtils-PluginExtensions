using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{
    public interface IPluginInternals : IInternalServices
    {

        IPluginExecutionContext ExecutionContext { get; }

        Type PluginType { get; }

    }
}
