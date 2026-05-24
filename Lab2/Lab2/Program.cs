
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System.Diagnostics;

namespace Lab2
{
    class Program
    {

        private static readonly Random random = new Random();
        private const string FILE_NAME = "Wyniki_RavenDB_Dokumenty.txt";
        private const string Endpoint = "http://localhost:8080";
        private const string databaseName = "Lab2HomeworkRavenDB";
        private const int amount = 1000;

        static int Main(string[] args)
        {


            using(IDocumentStore store = new DocumentStore
            {
                Urls = new[]
                {
                    Endpoint
                },
                Database = databaseName,
                Conventions = { }
            })
            {
                store.Initialize();

                try
                {
                    DatabaseRecord databaseRecord = new DatabaseRecord(databaseName);
                    CreateDatabaseOperation createDatabaseOperation = new CreateDatabaseOperation(databaseRecord);

                    store.Maintenance.Server.Send(createDatabaseOperation);
                    Console.WriteLine("Baza utworzona");
                }catch(ConcurrencyException e)
                {
                    Console.WriteLine($"Baza danych już istnieje {e.Message}");
                }


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


                    using (IDocumentSession session = store.OpenSession())
                    {
                        for(int i = 0; i < amount; i++)
                        {
                            Samochod car1 = new Samochod
                            {
                                Marka = "Marka_" + random.Next(10_000),
                                Model = "Model_" + random.Next(1000),
                                RokProdukcji = 1980 + random.Next(46)
                            };

                            Samochod car2 = new Samochod
                            {
                                Marka = "Marka_" + random.Next(10_000),
                                Model = "Model_" + random.Next(1000),
                                RokProdukcji = 1980 + random.Next(46)
                            };

                            session.Store(car1);
                            session.Store(car2);


                            Serwis serwis = new Serwis
                            {
                                Nazwa = "Nazwa_" + random.Next(10_000),
                                Miasto = "Miasto_" + random.Next(1000),
                                SamochodyId = new List<string> { car1.Id!, car2.Id! }
                            };

                            session.Store(serwis);

                        }

                        session.SaveChanges();
                    }


                    stopwatch.Stop();
                    break;
                case "2":
                    operationName = "Pobierz";
                    stopwatch.Start();

                    using (IDocumentSession session = store.OpenSession())
                    {
                        var serwisy = session.Query<Serwis>()
                            .Include<Serwis>(x => x.SamochodyId)
                            .Take(amount)
                            .ToList();


                        foreach(var serwis in serwisy)
                        {
                            Console.WriteLine(serwis);
                            foreach(var carId in serwis.SamochodyId)
                            {
                                var car = session.Load<Samochod>(carId);
                                if(car != null)
                                    Console.WriteLine(car);
                            }
                        }
                    }

                    

                    stopwatch.Stop();
                    break;
                case "3":
                    operationName = "Aktualizuj";
                    stopwatch.Start();

                    using(IDocumentSession session = store.OpenSession())
                    {
                        var serwisy = session.Query<Serwis>()
                            .Take(amount)
                            .ToList();

                        foreach(var serwis in serwisy)
                        {
                            serwis.Nazwa = "Nowa_Nazwa_" + random.Next(10_000);
                        }

                        session.SaveChanges();
                    }


                    stopwatch.Stop();
                    break;
                case "4":
                    operationName = "Usuń";
                    stopwatch.Start();

                    using(IDocumentSession session = store.OpenSession())
                    {
                        var serwisy = session.Query<Serwis>()
                            .Include<Serwis>(x => x.SamochodyId)
                            .Take(amount)
                            .ToList();

                        foreach(var serwis in serwisy)
                        {
                            foreach(var carId in serwis.SamochodyId)
                            {
                                var car = session.Load<Samochod>(carId);
                                if (car != null)
                                    session.Delete(car);
                            }
                            session.Delete(serwis);
                        }
                        session.SaveChanges();
                    }
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
                streamWriter.WriteLine($"{sklad};{operationName};{time};");

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}