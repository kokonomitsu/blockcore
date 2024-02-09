namespace Blockcore.Networks.Bigcoin
{
    public static class Networks
    {
        public static NetworksSelector Bigcoin
        {
            get
            {
                return new NetworksSelector(() => new BigcoinMain(), () => new BigcoinTest(), () => new BigcoinRegTest());
            }
        }
    }
}