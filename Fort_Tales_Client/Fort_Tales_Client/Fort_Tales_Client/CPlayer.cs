using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales_Client
{
    class CPlayer
    {
        public string Name;
        public int Food { get; set; }
        public int FoodIncome { get; set; }
        public int Resources { get; set; }
        public int ResourcesIncome { get; set; }
        public int Population { get; set; }
        public int MaxPopulation { get; set; }

        public CPlayer(string name)
        {
            Name = name;
        }
    }
}
