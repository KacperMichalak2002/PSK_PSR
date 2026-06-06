
using Cassandra;
using Praca_domowa_kolumnowa;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

namespace Lab3
{
    class Program
    {
        private const string FILE_NAME = "Wyniki_Elassandra_Kolumnowy.txt";
        private static ZooManager zooManager;
        private const int amount = 1000;

        static void Main(string[] args)
        {
            Cluster cluster = Cluster.Builder()
                .AddContactPoint("127.0.0.1")
                .WithPort(9042)
                .Build();

            using var session = cluster.Connect();

            zooManager = new ZooManager(session);


            zooManager.Setup();

            Console.WriteLine("Połączono z Elassandra");

            while (true)
            {
                Console.WriteLine($"1. Zapisz {amount} zoo");
                Console.WriteLine($"2. Pobierz {amount} zoo");
                Console.WriteLine($"3. Aktualizuj {amount} zoo");
                Console.WriteLine($"4. Usuń {amount} zoo");
                Console.WriteLine($"0. Wyjdź");

                string choice = Console.ReadLine();
                if (string.Equals(choice, "0"))
                    break;

                HandleOperation(choice, session);
            }

        }

        private static void HandleOperation(string? choice, ISession session)
        {
            Stopwatch stopwatch = new();
            string operationName = "";


            switch (choice)
            {
                case "1":
                    operationName = "Zapisz";
                    stopwatch.Start();

                    zooManager.insert();
                    

                    stopwatch.Stop();
                    break;
                case "2":
                    operationName = "Pobierz";
                    stopwatch.Start();

                    zooManager.select();

                    stopwatch.Stop();
                    break;
                case "3":
                    operationName = "Aktualizuj";
                    stopwatch.Start();


                    zooManager.update();

                    stopwatch.Stop();
                    break;
                case "4":
                    operationName = "Usuń";
                    stopwatch.Start();

                    zooManager.delete();

                    stopwatch.Stop();
                    break;
                default:
                    return;
            }

            long time = stopwatch.ElapsedMilliseconds;
            SaveToFile("Elassandra", operationName, time);
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