using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_domowa_kolumnowa
{
    public class Animal
    {
        public int ZooId { get; set; }
        public int AnimalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public int Age { get; set; }

        public override string ToString()
        {
            return $"  Zwierze ID: {AnimalId} Nazwa: {Name} Gatunek: {Species} Wiek: {Age}";
        }

    }


    public class Zoo
    {
        public int ZooId { get; set; }
        public string ZooName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Zoo: {ZooId} Nazwa:{ZooName}";
        }
    }
}
