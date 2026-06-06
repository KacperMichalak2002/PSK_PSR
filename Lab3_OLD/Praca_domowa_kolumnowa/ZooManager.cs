using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_domowa_kolumnowa
{
    public class ZooManager
    {
        private ISession session;
        private const string KEYSPACE = "zoo";
        private const int amount = 1000;
        private static readonly Random random = new Random();

        public ZooManager(ISession session)
        {
            this.session = session;
        }

        public void Setup()
        {
            session.Execute(
                $"CREATE KEYSPACE IF NOT EXISTS {KEYSPACE} " +
                $"WITH replication = {{'class':'SimpleStrategy','replication_factor':1}};"
            );

            session.Execute(
                $"CREATE TABLE IF NOT EXISTS {KEYSPACE}.zoos (" +
                $"  zoo_id   int PRIMARY KEY," +
                $"  zoo_name text" +
                $");"
            );

            session.Execute(
                $"CREATE TABLE IF NOT EXISTS {KEYSPACE}.animals (" +
                $"  zoo_id    int," +
                $"  animal_id int," +
                $"  name      text," +
                $"  species   text," +
                $"  age       int," +
                $"  PRIMARY KEY (zoo_id, animal_id)" +
                $");"
            );
        }

        public void insert()
        {
            session.Execute($"TRUNCATE {KEYSPACE}.zoos;");
            session.Execute($"TRUNCATE {KEYSPACE}.animals;");

            int animalId = 1;

            for(int i = 1; i <= amount; i++)
            {
                session.Execute($"INSERT INTO {KEYSPACE}.zoos (zoo_id, zoo_name) " + $"VALUES ({i}, 'Zoo_{i}');");

                for(int j = 0; j < 2; j++)
                {
                    session.Execute(
                                $"INSERT INTO {KEYSPACE}.animals (zoo_id, animal_id, name, species, age) " +
                                $"VALUES ({i}, {animalId++}, " +
                                $"'Zwierze_{random.Next(10000)}', " +
                                $"'Gatunek_{random.Next(1000)}', " +
                                $"{1 + random.Next(30)});"
                            );
                }
            }
        }

        public void select()
        {
            for(int i = 1; i <= amount; i++)
            {
                RowSet zoos = session.Execute($"SELECT * FROM {KEYSPACE}.zoos WHERE zoo_id = {i};");
                RowSet animals = session.Execute($"SELECT * FROM {KEYSPACE}.animals WHERE zoo_id = {i};");

                if(i <= 10)
                {
                    var zooRow = zoos.FirstOrDefault();
                    if(zooRow != null)
                        Console.WriteLine($"Zoo: {zooRow.GetValue<int>("zoo_id")} Nazwa: {zooRow.GetValue<string>("zoo_name")}");

                    foreach( Row row in animals)
                    {
                        Console.WriteLine($"  Zwierze ID: {row.GetValue<int>("animal_id")} " +
                                    $"Nazwa: {row.GetValue<string>("name")} " +
                                    $"Gatunek: {row.GetValue<string>("species")} " +
                                    $"Wiek: {row.GetValue<int>("age")}");
                    }
                }

            }
        }

        public void update()
        {
            for(int i = 1; i <= amount; i++)
            {
                session.Execute($"UPDATE {KEYSPACE}.zoos SET zoo_name = 'Nowe_Zoo_{random.Next(10000)}' " + $"WHERE zoo_id = {i};");

                var rs = session.Execute($"SELECT animal_id FROM {KEYSPACE}.animals WHERE zoo_id = {i};");

                foreach(Row row in rs)
                {
                    session.Execute(
                                $"UPDATE {KEYSPACE}.animals " +
                                $"SET name = 'Nowa_Nazwa_{random.Next(10000)}', " +
                                $"species = 'Nowy_Gatunek_{random.Next(1000)}', " +
                                $"age = {1 + random.Next(30)} " +
                                $"WHERE zoo_id = {i} AND animal_id = {row.GetValue<int>("animal_id")};"
                            );
                }
            }
        }

        public void delete()
        {
            for(int i = 1; i <= amount; i++)
            {
                session.Execute($"DELETE FROM {KEYSPACE}.animals WHERE zoo_id = {i};");
                session.Execute($"DELETE FROM {KEYSPACE}.zoos WHERE zoo_id = {i};");
            }
        }
    }
}
