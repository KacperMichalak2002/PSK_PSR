using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class Samochod
    {
        public string? Id { get; set; }
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int RokProdukcji { get; set; }

        public override string ToString()
        {
            return $"{{id={Id}, marka={Marka} model={Model} rok_produkcji={RokProdukcji}}}";
        }

    }


    public class Serwis
    {
        public string? Id { get; set; }
        public string Nazwa { get; set; } = string.Empty;
        public string Miasto { get; set; } = string.Empty;
        public List<string> SamochodyId { get; set; } = new();

        public override string ToString()
        {
            return $"{{id={Id}, nazwa={Nazwa} miasto={Miasto} samochody={string.Join(", ", SamochodyId)}}}";
        }
    }
}
