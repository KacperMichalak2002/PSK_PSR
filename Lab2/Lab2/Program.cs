
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System.Diagnostics;

namespace Lab2
{
    class Program
    {

        private static readonly Random random = new Random();
        private const string FILE_NAME = "Wyniki_RavenDB_Dokumenty.txt";
        private const string Endpoint = "http://localhost:8080";
        private const int amount = 1000;

        static int Main(string[] args)
        {


            using(IDocumentStore store = new DocumentStore
            {
                Urls = new[]
                {
                    Endpoint
                },
                Database = "Lab2HomeworkRavenDB",
                Conventions = { }
            })
            {
                store.Initialize();


                while (true)
                {
                    Console.WriteLine($"1. Zapisz {amount} pacjentów");
                    Console.WriteLine($"2. Pobierz {amount} pacjentów");
                    Console.WriteLine($"3. Aktualizuj {amount} pacjentów");
                    Console.WriteLine($"4. Usuń {amount} pacjentów");
                    Console.WriteLine($"0. Wyjdź");

                    string choice = Console.ReadLine();
                    if (string.Equals(choice, "0"))
                        break;

                    HandleOperation(choice, store);
                }


               
            }


            return 0;
        }

        private static void HandleOperation(string? choice, IDocumentStore store)
        {
            Stopwatch stopwatch = new();
            string operationName = "";


            switch (choice)
            {
                case "1":
                    operationName = "Zapisz";
                    stopwatch.Start();

                    stopwatch.Stop();
                    break;
                case "2":
                    operationName = "Pobierz";
                    stopwatch.Start();

                    stopwatch.Stop();
                    break;
                case "3":
                    operationName = "Aktualizuj";
                    stopwatch.Start();

                    stopwatch.Stop();
                    break;
                case "4":
                    operationName = "Usuń";
                    stopwatch.Start();

                    stopwatch.Stop();
                    break;
                default:
                    return;
            }

            long time = stopwatch.ElapsedMilliseconds;
            SaveToFile("RavenDB", operationName, time);
            Console.WriteLine($"Wykonano:\n{operationName} w czasie {time} ms");


        }

        private static void SaveToFile(string sklad, string operationName, long time)
        {
            try
            {
                using StreamWriter streamWriter= new StreamWriter(FILE_NAME, append: true);
                streamWriter.WriteLine($"{sklad};{operationName}:{time};");

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}