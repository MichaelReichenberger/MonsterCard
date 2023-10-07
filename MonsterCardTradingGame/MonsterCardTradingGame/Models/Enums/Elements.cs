using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models.Enums
{
    public enum ElementNames
    {
        Water,
        Fire,
        Normal
    }
    internal class Elements
    {
        
        public Dictionary<ElementNames, double> ElementProbabilties;

        public Elements()
        {
            ElementProbabilties = new Dictionary<ElementNames, double>
            {
                {ElementNames.Fire, 0.2},
                {ElementNames.Water, 0.3},
                {ElementNames.Normal, 0.5}
            };
        }

        public ElementNames ChooseWithProbability()
        {
            double randomValue = new Random().NextDouble();
            double sum = 0;

            foreach (var entry in ElementProbabilties)
            {
                sum += entry.Value;
                if (randomValue <= sum)
                {
                    return entry.Key;
                }
            }
            throw new Exception("Probabilities don't match up to 1");
        }
    }
}