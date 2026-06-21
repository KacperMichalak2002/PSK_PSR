using NFalkorDB;
using System.Diagnostics;


namespace Lab4
{
    class Program
    {
        private const string FILE_NAME = "Wyniki_Baza_Grafowa.txt";
        private static LeageService _leagueService;
        private const int amount = 1000;
        static void Main(string[] args)
        {


            var client = new FalkorDB("localhost:6379");
            Graph graph = client.SelectGraph("Liga");

            _leagueService = new LeageService(graph);

            try
            {
                while (true)
                {
                    Console.WriteLine($"1. Zapisz {amount} lig");
                    Console.WriteLine($"2. Pobierz {amount} lig");
                    Console.WriteLine($"3. Aktualizuj {amount} lig");
                    Console.WriteLine($"4. Usuń {amount} lig");
                    Console.WriteLine($"0. Wyjdź");

                    string choice = Console.ReadLine();
                    if (string.Equals(choice, "0"))
                        break;

                    HandleOperation(choice);
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
        }

        private static void HandleOperation(string? choice)
        {
            Stopwatch stopwatch = new();
            string operationName = "";


            switch (choice)
            {
                case "1":
                    operationName = "Zapisz";
                    stopwatch.Start();

                    _leagueService.Insert1000();

                    stopwatch.Stop();
                    break;
                case "2":
                    operationName = "Pobierz";
                    stopwatch.Start();

                    _leagueService.Select1000();

                    stopwatch.Stop();
                    break;
                case "3":
                    operationName = "Aktualizuj";
                    stopwatch.Start();

                    _leagueService.Update1000();

                    stopwatch.Stop();
                    break;
                case "4":
                    operationName = "Usuń";
                    stopwatch.Start();

                    _leagueService.Delete1000();
                  
                    stopwatch.Stop();
                    break;
                default:
                    return;
            }

            long time = stopwatch.ElapsedMilliseconds;
            SaveToFile("FalkorDB", operationName, time);
            Console.WriteLine($"Wykonano:\n{operationName} w czasie {time} ms");


        }
        private static void SaveToFile(string sklad, string operationName, long time)
        {
            try
            {
                using StreamWriter streamWriter = new StreamWriter(FILE_NAME, append: true);
                streamWriter.WriteLine($"{sklad};{operationName};{time};");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    
}