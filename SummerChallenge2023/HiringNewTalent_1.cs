using System.Collections.Generic;
using System.Linq;

namespace SummerChallenge2023
{
    public class HiringNewTalent_1
    {
        /**
     * @param mutantScores The score corresponding to each mutant
     * @param threshold The score threshold above which mutants should be ignored
     * @return
     */
        public static string BestRemainingMutant(Dictionary<string, double> mutantScores, int threshold) {
            // Write your code here
            return mutantScores.Where(x=>x.Value < threshold).OrderByDescending(x=> x.Value).First().Key;
        }
    }
}