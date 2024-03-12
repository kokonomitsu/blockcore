using System;
using System.Collections.Generic;
using System.Linq;
using Blockcore.Base.Deployments;
using Blockcore.Consensus;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus.Checkpoints;
using Blockcore.NBitcoin;
using Blockcore.NBitcoin.DataEncoders;
using Blockcore.NBitcoin.Protocol;
using Blockcore.Networks.Bigcoin.Deployments;
using Blockcore.Networks.Bigcoin.Policies;
using Blockcore.P2P;

namespace Blockcore.Networks.Bigcoin
{
    public class BigcoinTest : BigcoinMain
    {
        public BigcoinTest()
        {
            this.Name = "TestNet";
            this.AdditionalNames = new List<string> { "test" };
            this.NetworkType = NetworkType.Testnet;
            this.Magic = 0x0709110B;
            this.DefaultPort = 16333;
            this.DefaultMaxOutboundConnections = 8;
            this.DefaultMaxInboundConnections = 117;
            this.DefaultRPCPort = 16332;
            this.DefaultAPIPort = 36220;
            this.CoinTicker = "TBBTC";
            this.DefaultBanTimeSeconds = 60 * 60 * 24; // 24 Hours

            var consensusFactory = new ConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1707014259;
            this.GenesisNonce = 2625242;
            this.GenesisBits = 0x1e0ff000;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Coins(50m);

            Block genesisBlock = CreateBigcoinGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            this.Genesis = genesisBlock;

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 1,
                [BuriedDeployments.BIP65] = 1,
                [BuriedDeployments.BIP66] = 1
            };

            var bip9Deployments = new BigcoinBIP9Deployments
            {
                [BigcoinBIP9Deployments.TestDummy] = new BIP9DeploymentsParameters("TestDummy", 28, 0, 1230767999, BIP9DeploymentsParameters.DefaultTestnetThreshold),
                [BigcoinBIP9Deployments.CSV] = new BIP9DeploymentsParameters("CSV", 0, 0, 1493596800, BIP9DeploymentsParameters.DefaultTestnetThreshold),
                [BigcoinBIP9Deployments.Segwit] = new BIP9DeploymentsParameters("Segwit", 1, 0, 1493596800, BIP9DeploymentsParameters.DefaultTestnetThreshold)
            };

            consensusFactory.Protocol = new ConsensusProtocol()
            {
                ProtocolVersion = ProtocolVersion.FEEFILTER_VERSION,
                MinProtocolVersion = ProtocolVersion.SENDHEADERS_VERSION,
            };

            this.Consensus = new Consensus.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: new ConsensusOptions(), // Default - set to Bigcoin params.
                coinType: 0,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 2100000,
                majorityEnforceBlockUpgrade: 51,
                majorityRejectBlockOutdated: 75,
                majorityWindow: 100,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256(),
                minerConfirmationWindow: 20160,
                maxReorgLength: 0,
                defaultAssumeValid:null,// uint256.Zero, // 1354312
                maxMoney: 210000000 * Money.COIN,
                coinbaseMaturity: 100,
                premineHeight: 0,
                premineReward: Money.Zero,
                proofOfWorkReward: Money.Coins(50),
                targetTimespan: TimeSpan.FromSeconds(7 * 24 * 60 * 60), // one weeks
                targetSpacing: TimeSpan.FromSeconds(1 * 60),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: false,
                powNoRetargeting: false,
                powLimit: new Target(new uint256("00ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                minimumChainWork: uint256.Zero,
                isProofOfStake: false,
                lastPowBlock: default(int),
                proofOfStakeLimit: null,
                proofOfStakeLimitV2: null,
                proofOfStakeReward: Money.Zero,
                proofOfStakeTimestampMask: 0
            );

            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (111) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (196) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (239) };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 115 };

            var encoder = new Bech32Encoder("tbg");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            // Partially obtained from https://github.com/bitcoin/bitcoin/blob/b1973d6181eacfaaf45effb67e0c449ea3a436b8/src/chainparams.cpp#L246
            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                { 0, new CheckpointInfo(new uint256("0x000006cbccc32aa92048c497d070537e215a1d43073f62459b3577da69c6f69e")) },
                //{ 3_000, new CheckpointInfo(new uint256("000000007f0eaec313e119f8ba4ad2df1d9a617771058f25d65c1263bec75589")) },
                //{ 10_000, new CheckpointInfo(new uint256("000000000058b74204bb9d59128e7975b683ac73910660b6531e59523fb4a102")) },
                //{ 20_000, new CheckpointInfo(new uint256("0000000008ca11392fa91c4786e59823a002f4868bdb0c1385b12a2844cbc11f")) },
                //{ 50_000, new CheckpointInfo(new uint256("00000000077eacdd2c803a742195ba430a6d9545e43128ba55ec3c80beea6c0c")) },
                //{ 100_000, new CheckpointInfo(new uint256("00000000009e2958c15ff9290d571bf9459e93b19765c6801ddeccadbb160a1e")) },
                //{ 200_000, new CheckpointInfo(new uint256("0000000000287bffd321963ef05feab753ebe274e1d78b2fd4e2bfe9ad3aa6f2")) },
                //{ 300_000, new CheckpointInfo(new uint256("000000000000226f7618566e70a2b5e020e29579b46743f05348427239bf41a1")) },
                //{ 500_000, new CheckpointInfo(new uint256("000000000001a7c0aaa2630fbb2c0e476aafffc60f82177375b2aaa22209f606")) },
                //{ 800_000, new CheckpointInfo(new uint256("0000000000209b091d6519187be7c2ee205293f25f9f503f90027e25abf8b503")) },
                //{ 1_000_000, new CheckpointInfo(new uint256("0000000000478e259a3eda2fafbeeb0106626f946347955e99278fe6cc848414")) },
                //{ 1_210_000, new CheckpointInfo(new uint256("00000000461201277cf8c635fc10d042d6f0a7eaa57f6c9e8c099b9e0dbc46dc")) },
                //{ 1_400_000, new CheckpointInfo(new uint256("000000000000fce208da3e3b8afcc369835926caa44044e9c2f0caa48c8eba0f")) } // 22-08-2018
            };

            this.DNSSeeds = new List<DNSSeedData>
            {
               // new DNSSeedData("bitcoin.petertodd.org", "testnet-seed.bitcoin.petertodd.org"),
              //  new DNSSeedData("bluematt.me", "testnet-seed.bluematt.me"),
              //  new DNSSeedData("bitcoin.schildbach.de", "testnet-seed.bitcoin.schildbach.de")
            };

            string[] seedNodes = { "192.168.2.116", "121.41.129.118", "121.41.32.63", "121.40.247.124" };
            this.SeedNodes = ConvertToNetworkAddresses(seedNodes, this.DefaultPort).ToList();


           // this.SeedNodes = new List<NetworkAddress>();

            this.StandardScriptsRegistry = new BigcoinStandardScriptsRegistry();

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x000006cbccc32aa92048c497d070537e215a1d43073f62459b3577da69c6f69e"));

            this.RegisterRules(this.Consensus);
            this.RegisterMempoolRules(this.Consensus);
        }
    }
}