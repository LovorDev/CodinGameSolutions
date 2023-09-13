namespace SummerChallenge2023
{
    public class HiringNewTalent_2
    {
        /*
        SELECT agent.NAME, COUNT(agent.NAME) AS score
        FROM mutant, agent
        WHERE mutant.RECRUITERID = agent.AGENTID
        GROUP BY agent.NAME
        ORDER BY score DESC
        LIMIT 10
        */
    }
}