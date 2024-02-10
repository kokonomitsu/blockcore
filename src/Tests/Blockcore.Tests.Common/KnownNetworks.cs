using Blockcore.Networks;
using Blockcore.Networks.Bitcoin;

namespace Blockcore.Tests.Common
{
    public static class KnownNetworks
    {
        public static Network Main => NetworkRegistration.GetNetwork("Main") ?? NetworkRegistration.Register(new BitcoinMain());

        public static Network TestNet => NetworkRegistration.GetNetwork("TestNet") ?? NetworkRegistration.Register(new BitcoinTest());

        public static Network RegTest => NetworkRegistration.GetNetwork("RegTest") ?? NetworkRegistration.Register(new BitcoinRegTest());

       
    }
}