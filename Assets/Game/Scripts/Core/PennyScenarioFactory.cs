using System;
using System.Collections.Generic;

namespace PennyUniversity
{
    public static class PennyScenarioFactory
    {
        public static TruthScenario RollScenario(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            return (TruthScenario)random.Next(0, 4);
        }

        public static PennyGameState NewCampaign(int startingMoney, int randomSeed)
        {
            var random = new Random(randomSeed);
            var scenario = RollScenario(random);
            // Initial influence numbers are placeholders and NOT final game balance.
            var starting = new Dictionary<Faction, int>();
            foreach (Faction faction in Enum.GetValues(typeof(Faction)))
            {
                if (faction == Faction.Bystander) continue;
                starting[faction] = random.Next(8, 16);
            }
            return new PennyGameState(startingMoney, scenario, starting);
        }
    }
}