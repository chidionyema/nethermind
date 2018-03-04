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

using Nethermind.Blockchain;
using Nethermind.Blockchain.Difficulty;
using Nethermind.Blockchain.Validators;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Encoding;
using Nethermind.Core.Extensions;
using Nethermind.Core.Potocol;
using Nethermind.Evm;
using Nethermind.JsonRpc.Module;
using Nethermind.KeyStore;
using Nethermind.Store;
using NUnit.Framework;

namespace Nethermind.JsonRpc.Test
{
    [TestFixture]
    public class EthModuleTests
    {
        private IEthModule _ethModule;

        [SetUp]
        public void Initialize()
        {
            var logger = new ConsoleLogger();

            var blockStore = new BlockStore();
            IEthereumRelease release = Frontier.Instance;
            var blockValidator = new BlockValidator(new TransactionValidator(release, new SignatureValidator(release, ChainId.DefaultGethPrivateChain)), new BlockHeaderValidator(blockStore), new OmmersValidator(blockStore, new BlockHeaderValidator(blockStore)), logger);
            var db = new InMemoryDb();
            var stateProvider = new StateProvider(new StateTree(db), release, logger );
            var multiDb = new MultiDb(logger);
            var storageProvider = new StorageProvider(multiDb, stateProvider, logger );
            var virtualMachine = new VirtualMachine(release, stateProvider, storageProvider, new BlockhashProvider(blockStore), logger);
            var signer = new EthereumSigner(release, ChainId.MainNet);
            var transactionProcessor = new TransactionProcessor(release, stateProvider, storageProvider, virtualMachine, signer, logger);

            var transactionStore = new TransactionStore();
            var blockProcessor = new BlockProcessor(release, blockStore , blockValidator , new ProtocolBasedDifficultyCalculator(release), new RewardCalculator(release), transactionProcessor,
                multiDb, stateProvider, storageProvider, transactionStore, logger );

            var keyStoreConfigurationProvider = new KeyStore.ConfigurationProvider();
            var blockchaninProcessor = new BlockchainProcessor(blockProcessor, blockStore, logger);
            var keyStore = new FileKeyStore(keyStoreConfigurationProvider, new JsonSerializer(logger), new AesEncrypter(keyStoreConfigurationProvider, logger), new CryptoRandom(), logger);

            _ethModule = new EthModule(logger, new JsonSerializer(logger), blockchaninProcessor, stateProvider, keyStore, new ConfigurationProvider(), blockStore, db, new JsonRpcModelMapper(signer), release, transactionStore);
        }
        
        [Test]
        public void GetBalanceSuccessTest()
        {
            var hex = new Hex(1024.ToBigEndianByteArray()).ToString(true, true);
        }
    }
}