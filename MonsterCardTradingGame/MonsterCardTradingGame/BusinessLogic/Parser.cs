using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.BusinessLogic
{
    internal class Parser
    {
        public (string Element, string Creature) ParseCards(string cardName)
        {
            // Goblin
            if (cardName.Contains("Goblin") && cardName.Contains("Water")) return ("Water", "Goblin");
            if (cardName.Contains("Goblin") && cardName.Contains("Fire")) return ("Fire", "Goblin");
            if (cardName.Contains("Goblin"))  return ("Normal", "Goblin");

            // Dragon
            if (cardName.Contains("Dragon") && cardName.Contains("Water")) return ("Water", "Dragon");
            if (cardName.Contains("Dragon") && cardName.Contains("Fire")) return ("Fire", "Dragon");
            if (cardName.Contains("Dragon") ) return ("Normal", "Dragon");

            // Elf
            if (cardName.Contains("Elf") && cardName.Contains("Water")) return ("Water", "Elf");
            if (cardName.Contains("Elf") && cardName.Contains("Fire")) return ("Fire", "Elf");
            if (cardName.Contains("Elf") ) return ("Normal", "Elf");

            // Knight
            if (cardName.Contains("Knight") && cardName.Contains("Water")) return ("Water", "Knight");
            if (cardName.Contains("Knight") && cardName.Contains("Fire")) return ("Fire", "Knight");
            if (cardName.Contains("Knight") ) return ("Normal", "Knight");

            // Kraken
            if (cardName.Contains("Kraken") && cardName.Contains("Water")) return ("Water", "Kraken");
            if (cardName.Contains("Kraken") && cardName.Contains("Fire")) return ("Fire", "Kraken");
            if (cardName.Contains("Kraken") ) return ("Normal", "Kraken");

            // Orc
            if (cardName.Contains("Orc") && cardName.Contains("Water")) return ("Water", "Orc");
            if (cardName.Contains("Orc") && cardName.Contains("Fire")) return ("Fire", "Orc");
            if (cardName.Contains("Orc") ) return ("Normal", "Orc");

            // Wizard
            if (cardName.Contains("Wizard") && cardName.Contains("Water")) return ("Water", "Wizard");
            if (cardName.Contains("Wizard") && cardName.Contains("Fire")) return ("Fire", "Wizard");
            if (cardName.Contains("Wizard") ) return ("Normal", "Wizard");

            // Troll
            if (cardName.Contains("Troll") && cardName.Contains("Water")) return ("Water", "Troll");
            if (cardName.Contains("Troll") && cardName.Contains("Fire")) return ("Fire", "Troll");
            if (cardName.Contains("Troll") ) return ("Normal", "Troll");

            if (cardName.Contains("Spell") && cardName.Contains("Water")) return ("Water", "Spell");
            if (cardName.Contains("Spell") && cardName.Contains("Fire")) return ("Fire", "Spell");
            if (cardName.Contains("Spell")) return ("Normal", "Spell");

            // Default case
            return ("Unknown", "Unknown");
        }

        public string ParseUrl(string requestLine, int parseType)
        {
            string[] parts = requestLine.Split(' ');
            if (parts.Length > 1)
            {

                string[] urlParts = parts[1].Split('/');
                if (urlParts.Length > parseType)
                {
                    Console.WriteLine(urlParts[parseType]);
                    if (parseType == 1)
                    {
                        Console.WriteLine("/" + urlParts[parseType]);
                        return "/" + urlParts[parseType];
                    }
                    return urlParts[parseType];
                }
            }
            return "/";
        }

        public bool IsValidJson(string jsonString)
        {
            try
            {
                using (JsonDocument.Parse(jsonString))
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
        }

    }
}
