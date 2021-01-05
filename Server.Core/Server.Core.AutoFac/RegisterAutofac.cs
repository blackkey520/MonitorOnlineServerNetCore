﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.AutoFac
{
    public static class RegisterAutofac
    {
        public static IServiceProvider ForRegisterAutofac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule<AutofacModuleRegister>();
            var container = builder.Build(); 
            return new AutofacServiceProvider(container);
        }
    }
}
