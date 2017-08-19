﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Autofac;
using Spinvoice.Common.Infrastructure.Logging;
using Spinvoice.Infrastructure.DataAccess;

namespace Spinvoice.Application.Services
{
    public static class Bootstrapper
    {
        public static IContainer Init()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Common.Infrastructure.AutofacModule>();
            containerBuilder.RegisterModule<Infrastructure.DataAccess.AutofacModule>();
            containerBuilder.RegisterModule<Infrastructure.Pdf.AutofacModule>();
            containerBuilder.RegisterModule<QuickBooks.AutofacModule>();
            containerBuilder.RegisterModule<Domain.AutofacModule>();
            containerBuilder.RegisterModule<AutofacModule>();

            var dataDirectoryProvider = new DataDirectoryProvider(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Spinvoice",
                "data"));
            containerBuilder.RegisterInstance(dataDirectoryProvider).AsImplementedInterfaces();

            var container = containerBuilder.Build();

            container.Resolve<ILogConfigurator>().Configure();
            container.Resolve<IServerManager>().Start();

            Server.Properties.ServerSatellite.Include();

            return container;
        }
    }
}