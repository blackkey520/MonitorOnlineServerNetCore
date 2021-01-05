using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Core.AutoFac;
using Server.Core.RunTime;
using Server.Core.Sockets;

namespace Server.DataReceive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();
            return RegisterAutofac.ForRegisterAutofac(services); 
        }
        //autofac 新增
        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    // 直接用Autofac注册我们自定义的 
        //    builder.RegisterType<StandardAgreementAnalyticer>().As<IAnalytice>().PropertiesAutowired();
        //    builder.RegisterType<Module_DataAnalytices>().Keyed<IMonitorOnlineModule>("Module_DataAnalytices").PropertiesAutowired().SingleInstance();
        //    builder.RegisterType<Module_MsgConsole>().Keyed<IMonitorOnlineModule>("Module_MsgConsole").PropertiesAutowired();
        //    builder.RegisterType<Module_RabbitMQ>().As<IMsgQueue>().PropertiesAutowired();
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var loggingOptions = this.Configuration.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
            loggerFactory.AddLog4Net(loggingOptions);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
