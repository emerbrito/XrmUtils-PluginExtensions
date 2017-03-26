using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmUtils.Extensions.Plugins
{

    [Flags]
    public enum ContextTargetType
    {
        None = 0,
        Entity = 1,
        EntityReference = 2,
        Unknown = 4
    }

}
