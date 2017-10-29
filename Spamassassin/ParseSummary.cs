using System.Collections.Generic;

namespace Outlook365.SpamAssassin.Spamassassin
{
    /// <summary>
    /// Stores the Spamassassin results
    /// </summary>
    public class ParseSummary
    {
        /// <summary>
        /// SpamAssassin thinks it is spam
        /// </summary>
        public bool IsSpam { get; set; }

        /// <summary>
        /// Spam Score set in SpamAssassin
        /// </summary>
        public double SpamScore { get; set; }
        /// <summary>
        /// Score determined by SpamAssassin
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Rules Triggered
        /// </summary>
        public List<RuleResult> Report { get; set; }
    }
}
