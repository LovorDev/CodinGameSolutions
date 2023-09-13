using System.Collections.Generic;
using System.Linq;

namespace SummerChallenge2023
{
    public class HiringNewTalent_3
    {
        /**
         * @param fileContents A list of strings, where each string represents the contents of a file.
         * @return The contents of the merged file.
         */
        public static string MergeFiles(List<string> fileContents)
        {
            // Write your code here

            return string.Join('\n', fileContents.SelectMany(f => f.Split('\n').Select(x => x.Split(';').ToArray()))
                .GroupBy(x => x.First())
                .Select(x =>
                    new[] { x.Key }.Concat(x.Aggregate((x, y) => x.Union(y).ToArray()).Except(new[] { x.Key })
                        .OrderBy(x => x)))
                .OrderBy(x => x.First()).Select(x => string.Join(';', x)));
        }
    }
}