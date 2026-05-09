using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{ 
    public class Przychodnia
    {
        public string id { get; set; }
        public string nazwa { get; set; }
    }
    
    public class Pacjent
    {
        public string id { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }

        public List<Przychodnia> przychodnie { get; set; }

        public override string ToString()
        {
            return $"Pacjent{{id={id}, imie={imie}, nazwisko={nazwisko}}}"; ;
        }

    }
}
