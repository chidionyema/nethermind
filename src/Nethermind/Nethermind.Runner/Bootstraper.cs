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

using LightInject;
using Nethermind.Core;
using Nethermind.JsonRpc;
using Nethermind.JsonRpc.Module;
using Nethermind.Runner.Controllers;

namespace Nethermind.Runner
{
    public class Bootstraper
    {
        public static ServiceContainer Container { get; set; }

        public Bootstraper()
        {
            Container = new ServiceContainer();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            Container.Register<ILogger, ConsoleLogger>();
            Container.Register<IConfigurationProvider, ConfigurationProvider>(new PerContainerLifetime());
            Container.Register<IJsonSerializer, JsonSerializer>();
            Container.Register<INetModule, NetModule>();
            Container.Register<IWeb3Module, Web3Module>();
            Container.Register<IEthModule, EthModule>();
            Container.Register<IJsonRpcService, JsonRpcService>();

            Container.Register<IJsonRpcRunner, JsonRpcRunner>();
            Container.Register<IEthereumRunner, EthereumRunner>();

            Container.Register<MainController>();
        }
    }
}