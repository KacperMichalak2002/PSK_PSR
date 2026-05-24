using System;
using System.Collections.Generic;
using System.Linq;
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
        public string SerwisId { get; set; } = string.Empty;

    }


    public class Serwis
    {
        public string? Id { get; set; }
        public string Nazwa { get; set; } = string.Empty;
        public string Miasto { get; set; } = string.Empty;
        public List<string> SamochodyId { get; set; } = new();
    }
}
