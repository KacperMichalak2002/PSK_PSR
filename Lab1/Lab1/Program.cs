
using Microsoft.Azure.Cosmos;
using System.Diagnostics;
using System.Net;

namespace Lab1
{
    //https://localhost:8081/_explorer/index.html
    class Program
    {
        private const string EmulatorKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string Endpoint = "https://localhost:8081";
        private const string FileName = "Wyniki_Hazelcast.txt";
        private const int amount = 1000;


        static async Task Main(string [] args)
        {
            Console.WriteLine("Łączenie z bazą...");

            using CosmosClient client = new CosmosClient(Endpoint, EmulatorKey, new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true,
                HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
            });

            try
            {
                Database database = await client.CreateDatabaseIfNotExistsAsync("Lab1Db");
                Container patientMap = await database.CreateContainerIfNotExistsAsync("Pacjenci", "/id");
                Container przychodnieMap = await database.CreateContainerIfNotExistsAsync("Przychonie", "/id");

                for(int i = 1; i <= 10; i++)
                {
                    await przychodnieMap.UpsertItemAsync(new Przychodnia { id = i.ToString() , nazwa = $"Przychodnia_{i}"});
                }


                Console.WriteLine($"Połączono {database.Id}");

                while (true)
                {
                    Console.WriteLine($"1. Zapisz {amount} pacjentów");
                    Console.WriteLine($"2. Pobierz {amount} pacjentów");
                    Console.WriteLine($"3. Aktualizuj {amount} pacjentów");
                    Console.WriteLine($"4. Usuń {amount} pacjentów");
                    Console.WriteLine($"0. Wyjdź");

                    string choice  = Console.ReadLine();
                    if (string.Equals(choice, "0"))
                        break;

                    await HandleOperation(choice, patientMap, przychodnieMap);
                }

                

            }catch(Exception e)
            {
                Console.WriteLine($"Błąd CosmosDB: {e.Message}");
            }
            
        }
        
        private static async Task HandleOperation(string choice, Container pacjentMap, Container przychodnieMap)
        {
            Stopwatch stopwatch = new Stopwatch();
            string operationName = "";

            switch (choice)
            {
                case "1":
                    operationName = "Zapisz";
                    stopwatch.Start();

                    for(int i = 1; i <= amount; i++)
                    {
                        string przychodniaId = Random.Shared.Next(1, 11).ToString();
                        Przychodnia przychodnia = await przychodnieMap.ReadItemAsync<Przychodnia>(przychodniaId, new PartitionKey(przychodniaId));

                        Pacjent newPacjent = new Pacjent { id = i.ToString(), imie = $"Imie_{i}", nazwisko = $"Nazwisko_{i}", przychodnia = przychodnia };
                        await pacjentMap.UpsertItemAsync(newPacjent);
                    }
                    stopwatch.Stop();

                    break;
                case "2":
                    operationName = "Pobierz";
                    stopwatch.Start();
                    for(int i = 1; i <= amount; i++)
                    {
                        string id = i.ToString();
                        try
                        {
                            Pacjent pacjet = await pacjentMap.ReadItemAsync<Pacjent>(id, new PartitionKey(id));
                            if (i <= 10)
                            {
                                Console.WriteLine(pacjet);
                            }
                        }catch(CosmosException e)
                        {
                            Console.WriteLine(e.StatusCode);
                            break;
                        }
                       
                    }
                    stopwatch.Stop();
                    break;
                case "3":
                    operationName = "Aktualizuj";
                    stopwatch.Start();
                    
                    for(int i = 1; i <= amount; i++)
                    {
                        string przychodniaId = Random.Shared.Next(1, 11).ToString();
                        Przychodnia przychodnia = await przychodnieMap.ReadItemAsync<Przychodnia>(przychodniaId, new PartitionKey(przychodniaId));

                        Pacjent newPacjent = new Pacjent { id = i.ToString(), imie = $"Imie_{i}", nazwisko = $"Nazwisko_{i}", przychodnia = przychodnia };
                        await pacjentMap.UpsertItemAsync(newPacjent);
                    }
                    stopwatch.Stop();
                    break;
                case "4":
                    operationName = "Usuń";
                    stopwatch.Start();

                    for(int i =1; i <= amount; i++)
                    {
                        string id = i.ToString();
                        await pacjentMap.DeleteItemAsync<Pacjent>(id, new PartitionKey(id));
                    }

                    stopwatch.Stop();
                    break;
                default:
                    return;
            }

            long time = stopwatch.ElapsedMilliseconds;
            SaveToFile("CosmosDB", operationName, time);
            Console.WriteLine($"Wykonano:\n{operationName} w czasie {time} ms");
        }


        private static void SaveToFile(string sklad, string operationName, long time)
        {
            try
            {
                string line = $"{sklad};{operationName};{time}\n";
                File.AppendAllText(FileName, line);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}