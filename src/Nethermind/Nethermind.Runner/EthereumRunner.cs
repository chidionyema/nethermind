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

using System.IO;
using Nethermind.Blockchain;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Encoding;
using Nethermind.Core.Extensions;

namespace Nethermind.Runner
{
    public class EthereumRunner : IEthereumRunner
    {
        private readonly IJsonRpcRunner _jsonRpcRunner;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IBlockchainProcessor _blockchainProcessor;

        public EthereumRunner(IJsonRpcRunner jsonRpcRunner, IJsonSerializer jsonSerializer, IBlockchainProcessor blockchainProcessor)
        {
            _jsonRpcRunner = jsonRpcRunner;
            _jsonSerializer = jsonSerializer;
            _blockchainProcessor = blockchainProcessor;
        }

        public void Start(string bootNodeValue, int discoveryPort, string genesisFile)
        {
            var genesisBlockRaw = File.ReadAllText(genesisFile);
            var blockJson = _jsonSerializer.Deserialize<TestBlockHeaderJson>(genesisBlockRaw);
            var block = Convert(blockJson);
            _blockchainProcessor.Initialize(Rlp.Encode(block));
        }

        public void Stop()
        {
            
        }

        private static Block Convert(TestBlockHeaderJson headerJson)
        {
            if (headerJson == null)
            {
                return null;
            }

            var header = new BlockHeader(
                new Keccak(headerJson.ParentHash),
                Keccak.OfAnEmptySequenceRlp,
                new Address(headerJson.Coinbase),
                Hex.ToBytes(headerJson.Difficulty).ToUnsignedBigInteger(),
                0,
                (long) Hex.ToBytes(headerJson.GasLimit).ToUnsignedBigInteger(),
                Hex.ToBytes(headerJson.Timestamp).ToUnsignedBigInteger(),
                Hex.ToBytes(headerJson.ExtraData)
            )
            {
                //Bloom = new Bloom(Hex.ToBytes(headerJson.Bloom).ToBigEndianBitArray2048()),
                //GasUsed = (long) Hex.ToBytes(headerJson.GasUsed).ToUnsignedBigInteger(),
                //Hash = new Keccak(headerJson.Hash),
                MixHash = new Keccak(headerJson.MixHash),
                Nonce = (ulong) Hex.ToBytes(headerJson.Nonce).ToUnsignedBigInteger()
                
                //ReceiptsRoot = new Keccak(headerJson.ReceiptTrie),
                //StateRoot = new Keccak(headerJson.StateRoot),
                //TransactionsRoot = new Keccak(headerJson.TransactionsTrie)
            };
            //header.RecomputeHash();

            var encodedHeader = Rlp.Encode(header);

            var hash = Keccak.Compute(Rlp.Encode(header));
            header.Hash = hash;

            var block = new Block(header);
            return block;
        }
    }

    public class TestBlockHeaderJson
    {
        public string Bloom { get; set; }
        public string Coinbase { get; set; }
        public string Difficulty { get; set; }
        public string ExtraData { get; set; }
        public string GasLimit { get; set; }
        public string GasUsed { get; set; }
        public string Hash { get; set; }
        public string MixHash { get; set; }
        public string Nonce { get; set; }
        public string Number { get; set; }
        public string ParentHash { get; set; }
        public string ReceiptTrie { get; set; }
        public string StateRoot { get; set; }
        public string Timestamp { get; set; }
        public string TransactionsTrie { get; set; }
        public string UncleHash { get; set; }
    }
}