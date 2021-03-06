﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public interface IServiceConfiguration
    {
        IReadOnlyDictionary<string, object> Properties { get; }

        ILifetimeScope Container { get; }

        IEventLog EventLog { get; }
    }
}
