using Blockcore.Consensus.Rules;
using Blockcore.Features.Consensus.Rules.CommonRules;

namespace Blockcore.Networks.Bigcoin.Rules
{
    /// <summary>
    /// Checks if <see cref="BigcoinMain"/> network block's header has a valid block version.
    /// <seealso cref="BigcoinActivationRule" />
    /// </summary>
    public class BigcoinHeaderVersionRule : HeaderVersionRule
    {
        /// <inheritdoc />
        public override void Run(RuleContext context)
        {
            // This is a stub rule - all version numbers are valid except those rejected by BigcoinActivationRule based
            // on the combination of their block height and version number.

            // All networks need a HeaderVersionRule implementation, as ComputeBlockVersion is used for block creation.
        }
    }
}