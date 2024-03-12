using System;
using System.Collections.Generic;
using System.Linq;
using Blockcore.Base.Deployments;
using Blockcore.Consensus;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus.Checkpoints;
using Blockcore.Consensus.ScriptInfo;
using Blockcore.Consensus.TransactionInfo;
using Blockcore.Features.Consensus.Rules.CommonRules;
using Blockcore.Features.Consensus.Rules.UtxosetRules;
using Blockcore.Features.MemoryPool.Rules;
using Blockcore.NBitcoin;
using Blockcore.NBitcoin.DataEncoders;
using Blockcore.NBitcoin.Protocol;
using Blockcore.Networks.Bigcoin.Deployments;
using Blockcore.Networks.Bigcoin.Policies;
using Blockcore.Networks.Bigcoin.Rules;
using Blockcore.P2P;

namespace Blockcore.Networks.Bigcoin
{
    public class BigcoinMain : Network
    {
        public BigcoinMain()
        {
            this.Name = "Main";
            this.AdditionalNames = new List<string> { "Mainnet" };
            this.NetworkType = NetworkType.Mainnet;
            
            this.RootFolderName = BigcoinRootFolderName;
            this.DefaultConfigFilename = BigcoinDefaultConfigFilename;
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            this.Magic = 0xD9B4BEF9;
            this.DefaultPort = 6333;
            this.DefaultMaxOutboundConnections = 8;
            this.DefaultMaxInboundConnections = 117;
            this.DefaultRPCPort = 6332;
            this.DefaultAPIPort = 37220;
            this.MaxTimeOffsetSeconds = BigcoinMaxTimeOffsetSeconds;
            this.MaxTipAge = BigcoinDefaultMaxTipAgeInSeconds;
            this.MinTxFee = 1000;
            this.MaxTxFee = Money.Coins(0.1m).Satoshi;
            this.FallbackFee = 20000;
            this.MinRelayTxFee = 1000;
            this.CoinTicker = "BBTC";
            this.DefaultBanTimeSeconds = 60 * 60 * 24; // 24 Hours

            var consensusFactory = new ConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1709914688;
            this.GenesisNonce = 623989;
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
                [BigcoinBIP9Deployments.TestDummy] = new BIP9DeploymentsParameters("TestDummy", 28, 0, 1230767999, BIP9DeploymentsParameters.DefaultMainnetThreshold),
                [BigcoinBIP9Deployments.CSV] = new BIP9DeploymentsParameters("CSV", 0, 0, 1493596800, BIP9DeploymentsParameters.DefaultMainnetThreshold),
                [BigcoinBIP9Deployments.Segwit] = new BIP9DeploymentsParameters("Segwit", 1, 0, 1510704000, BIP9DeploymentsParameters.DefaultMainnetThreshold)
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
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256(),
                minerConfirmationWindow: 10080, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 0,
                defaultAssumeValid: uint256.Zero,
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
                minimumChainWork: uint256.One,
                isProofOfStake: false,
                lastPowBlock: default(int),
                proofOfStakeLimit: null,
                proofOfStakeLimitV2: null,
                proofOfStakeReward: Money.Zero,
                proofOfStakeTimestampMask: 0
            );

            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (0) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (5) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (128) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };

            var encoder = new Bech32Encoder("bgc");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            // Partially obtained from https://github.com/bitcoin/bitcoin/blob/b1973d6181eacfaaf45effb67e0c449ea3a436b8/src/chainparams.cpp#L146
            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                { 0, new CheckpointInfo(new uint256("0x00000e93f1a71e0f151b6c5f3ff375569ac5ec3b8b8d27aec1c3419c774c2a3c"))},
              
            };

            this.DNSSeeds = new List<DNSSeedData>
            {
             //   new DNSSeedData("bitcoin.sipa.be", "seed.bitcoin.sipa.be"), // Pieter Wuille
               // new DNSSeedData("bluematt.me", "dnsseed.bluematt.me"), // Matt Corallo
               // new DNSSeedData("dashjr.org", "dnsseed.bitcoin.dashjr.org"), // Luke Dashjr
               // new DNSSeedData("bitcoinstats.com", "seed.bitcoinstats.com"), // Christian Decker
               // new DNSSeedData("xf2.org", "bitseed.xf2.org"), // Jeff Garzik
               // new DNSSeedData("bitcoin.jonasschnelli.ch", "seed.bitcoin.jonasschnelli.ch") // Jonas Schnelli
            };

            string[] seedNodes = { "8.219.158.4","8.222.190.167","47.236.20.126","8.219.55.128" };
            this.SeedNodes = ConvertToNetworkAddresses(seedNodes, this.DefaultPort).ToList();

            this.StandardScriptsRegistry = new BigcoinStandardScriptsRegistry();

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x00000e93f1a71e0f151b6c5f3ff375569ac5ec3b8b8d27aec1c3419c774c2a3c"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("0x95b40e9bac3c952c97e04fe0f3b8b47590d3a3ecf4b1e113cdfa4991f104b474"));

            this.RegisterRules(this.Consensus);
            this.RegisterMempoolRules(this.Consensus);
        }

        protected void RegisterRules(IConsensus consensus)
        {
            consensus.ConsensusRules
                .Register<HeaderTimeChecksRule>()
                .Register<CheckDifficultyPowRule>()
                .Register<BigcoinActivationRule>()
                .Register<BigcoinHeaderVersionRule>();

            consensus.ConsensusRules
                .Register<BlockMerkleRootRule>();

            consensus.ConsensusRules
                .Register<SetActivationDeploymentsPartialValidationRule>()

                .Register<TransactionLocktimeActivationRule>() // implements BIP113
                .Register<CoinbaseHeightActivationRule>() // implements BIP34
                .Register<WitnessCommitmentsRule>() // BIP141, BIP144
                .Register<BlockSizeRule>()

                // rules that are inside the method CheckBlock
                .Register<EnsureCoinbaseRule>()
                .Register<CheckPowTransactionRule>()
                .Register<CheckSigOpsRule>();

            consensus.ConsensusRules
                .Register<SetActivationDeploymentsFullValidationRule>()

                // rules that require the store to be loaded (coinview)
                .Register<FetchUtxosetRule>()
                .Register<TransactionDuplicationActivationRule>() // implements BIP30
                .Register<CheckPowUtxosetPowRule>()// implements BIP68, MaxSigOps and BlockReward calculation
                .Register<PushUtxosetRule>()
                .Register<FlushUtxosetRule>();
        }

        protected void RegisterMempoolRules(IConsensus consensus)
        {
            consensus.MempoolRules = new List<Type>()
            {
                typeof(CheckConflictsMempoolRule),
                typeof(CheckCoinViewMempoolRule),
                typeof(CreateMempoolEntryMempoolRule),
                typeof(CheckSigOpsMempoolRule),
                typeof(CheckFeeMempoolRule),
                typeof(CheckRateLimitMempoolRule),
                typeof(CheckAncestorsMempoolRule),
                typeof(CheckReplacementMempoolRule),
                typeof(CheckAllInputsMempoolRule),
                typeof(CheckTxOutDustRule)
            };
        }

        /// <summary> Bigcoin maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int BigcoinMaxTimeOffsetSeconds = 70 * 60;

        /// <summary> Bigcoin default value for the maximum tip age in seconds to consider the node in initial block download (24 hours). </summary>
        public const int BigcoinDefaultMaxTipAgeInSeconds = 30 * 24 * 60 * 60;

        /// <summary> The name of the root folder containing the different Bigcoin blockchains (Main, TestNet, RegTest). </summary>
        public const string BigcoinRootFolderName = "bigcoin";

        /// <summary> The default name used for the Bigcoin configuration file. </summary>
        public const string BigcoinDefaultConfigFilename = "bigcoin.conf";

        public static Block CreateBigcoinGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "Bitcoin for the people";
            var genesisOutputScript = new Script(Op.GetPushOp(Encoders.Hex.DecodeData("0459faebe224e823c3656672c1cc4659cfc9a25f2a3d7b6b98108020983a2628bf6ef2616a20d52f80c38a6cdcfd3e583896ae3e6702c0a5a388966bd61905fd53")), OpcodeType.OP_CHECKSIG);

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(486604799), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)4 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
                ScriptPubKey = genesisOutputScript
            });

            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();
            return genesis;
        }
    }
}