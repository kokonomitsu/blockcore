using Blockcore.Configuration;

namespace Blockcore.Node
{
    public static class NetworkSelector
    {
        public static NodeSettings Create(string chain, string[] args)
        {
            chain = chain.ToUpperInvariant();

            NodeSettings nodeSettings = null;

            switch (chain)
            {

                //case "BTC":
                //    nodeSettings = new NodeSettings(networksSelector: Blockcore.Networks.Bitcoin.Networks.Bitcoin, args: args);
                //    break;
                case "BBTC":
                    nodeSettings = new NodeSettings(networksSelector: Blockcore.Networks.Bigcoin.Networks.Bigcoin, args: args);
                    break;
            }

            return nodeSettings;
        }
    }
}