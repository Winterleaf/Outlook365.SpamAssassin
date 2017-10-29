using System;

namespace Outlook365.SpamAssassin.Spamassassin
{
    /// <summary>
    /// Ruleset Result Definitions
    /// </summary>
    public class RuleResult
    {
        /// <summary>
        /// Rule Description
        /// </summary>
        public string Description = "";
        /// <summary>
        /// Rule
        /// </summary>
        public string Rule = "";
        /// <summary>
        /// Score
        /// </summary>
        public double Score;

      /// <summary>
      /// Parses a line of the result into a rule result
      /// </summary>
      /// <param name="line"></param>
        public RuleResult(string line)
        {
            Score = double.Parse(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)).Trim());
            line = line.Substring(line.IndexOf(" ", StringComparison.Ordinal) + 1);
            Rule = line.Substring(0, 23).Trim();
            Description = line.Substring(23).Trim();
        }
    }
}
