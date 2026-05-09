
using Microsoft.Azure.Cosmos;
using System.Net;

namespace Lab1
{
    class Program
    {
        private const string EmulatorKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string Endpoint = "https://localhost:8081";
        private const string FileName = "Wyniki_Hazelcast.txt";


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

                Console.WriteLine($"Połączono {database.Id}");

            }catch(Exception e)
            {
                Console.WriteLine($"Błąd CosmosDB: {e.Message}");
            }
            
        }

    }
}