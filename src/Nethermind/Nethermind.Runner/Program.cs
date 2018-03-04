/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using LightInject;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nethermind.Core;

namespace Nethermind.Runner
{
    class Program
    {
        private static ILogger _logger;

        static void Main(string[] args)
        {
            try
            {
                //var bootstraper = new Bootstraper();
                var webHost = BuildWebHost(args);

                _logger = webHost.Services.GetService<ILogger>();

                var ethereumRunner = webHost.Services.GetService<IEthereumRunner>();
                ethereumRunner.Start();

                var jsonRpcRunner = webHost.Services.GetService<IJsonRpcRunner>();
                jsonRpcRunner.Start(webHost);
            }
            catch (Exception e)
            {
                if (_logger == null)
                {
                    _logger = new ConsoleLogger();
                }
                _logger.Error("Error during Runner start", e);
            }      
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
