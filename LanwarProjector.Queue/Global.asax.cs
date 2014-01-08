using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using LanwarProjector.Queue;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using AutoMapper;
using log4net;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using LanwarProjector.Common;

namespace LanwarProjector.Queue
{
    public class Global : HttpApplication
    {
        //public static Config Config { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(Global));
        public static string AdminKey = ConfigurationManager.AppSettings["AdminKey"];
        

        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.MapSignalR("/signalr", new Microsoft.AspNet.SignalR.HubConfiguration());
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();

            log.Debug("Startup");
            // Code that runs on application startup
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            var serializer = JsonSerializer.Create(serializerSettings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer); 

            // Setup Automapper
            log.Debug("Setup AutoMapper");
            AutoMapperSetup.Setup();

            log.Debug("Testing NH Session Creation");
            using (NhFactory.OpenSession())
            {
                log.Debug("Testing NH Session Creation");
                // Just a test to ensure we connect successfully
            }

            
        }

        void Application_End(object sender, EventArgs e)
        {
           
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }
    }
}
