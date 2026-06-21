using NFalkorDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class LeageService
    {
        private readonly Graph _graph;
        private readonly Random _random = new Random();
        private const int Amount = 1000;

        public LeageService(Graph graph)
        {
            _graph = graph;
        }

        public void Insert1000()
        {
            for (int i = 1; i <= Amount; i++)
            {
                CreateLeague(i, "Liga_" + i);

                CreateTeam(
                    i * 2 - 1,
                    "Druzyna_" + _random.Next(10000),
                    "Miasto_" + _random.Next(1000),
                    1900 + _random.Next(125)
                );
                CreateTeam(
                    i * 2,
                    "Druzyna_" + _random.Next(10000),
                    "Miasto_" + _random.Next(1000),
                    1900 + _random.Next(125)
                );

                CreateHasTeamRelationship(i, i * 2 - 1);
                CreateHasTeamRelationship(i, i * 2);
            }
        }

        public void Select1000()
        {
            const string command =
                "MATCH (l:League)-[r:HAS_TEAM]->(t:Team) " +
                "RETURN l.name AS leagueName, t.name AS teamName, t.city AS city, t.founded AS founded";

            Console.WriteLine("Executing: " + command);

            ResultSet result = _graph.Query(command);

            int count = 0;
            foreach (Record record in result)
            {
                if (count < 10)
                {
                    Console.WriteLine(
                        "Liga: " + record.GetString("leagueName") +
                        " Druzyna: " + record.GetString("teamName") +
                        " Miasto: " + record.GetString("city") +
                        " Rok zalozenia: " + record.GetValue<long>("founded"));
                }
                count++;
            }
        }

        public void Update1000()
        {
            for (int i = 1; i <= Amount; i++)
            {
                UpdateLeague(i, "Nowa_Liga_" + _random.Next(10000));

                UpdateTeam(
                    i * 2 - 1,
                    "Nowa_Druzyna_" + _random.Next(10000),
                    "Nowe_Miasto_" + _random.Next(1000),
                    1900 + _random.Next(125)
                );
                UpdateTeam(
                    i * 2,
                    "Nowa_Druzyna_" + _random.Next(10000),
                    "Nowe_Miasto_" + _random.Next(1000),
                    1900 + _random.Next(125)
                );
            }
        }

        public void Delete1000()
        {
            DeleteAll();
        }


        private void CreateLeague(int leagueId, string name)
        {
            const string command = "CREATE (:League {league_id: $leagueId, name: $name})";
            var parameters = new Dictionary<string, object>
            {
                { "leagueId", leagueId },
                { "name", name }
            };
            _graph.Query(command, parameters);
        }

        private void CreateTeam(int teamId, string name, string city, int founded)
        {
            const string command =
                "CREATE (:Team {team_id: $teamId, name: $name, city: $city, founded: $founded})";
            var parameters = new Dictionary<string, object>
            {
                { "teamId", teamId },
                { "name", name },
                { "city", city },
                { "founded", founded }
            };
            _graph.Query(command, parameters);
        }

        private void CreateHasTeamRelationship(int leagueId, int teamId)
        {
            const string command =
                "MATCH (l:League {league_id: $leagueId}), (t:Team {team_id: $teamId}) " +
                "CREATE (l)-[:HAS_TEAM]->(t)";
            var parameters = new Dictionary<string, object>
            {
                { "leagueId", leagueId },
                { "teamId", teamId }
            };
            _graph.Query(command, parameters);
        }

        private void UpdateLeague(int leagueId, string newName)
        {
            const string command = "MATCH (l:League {league_id: $leagueId}) SET l.name = $newName";
            var parameters = new Dictionary<string, object>
            {
                { "leagueId", leagueId },
                { "newName", newName }
            };
            _graph.Query(command, parameters);
        }

        private void UpdateTeam(int teamId, string newName, string newCity, int newFounded)
        {
            const string command =
                "MATCH (t:Team {team_id: $teamId}) " +
                "SET t.name = $newName, t.city = $newCity, t.founded = $newFounded";
            var parameters = new Dictionary<string, object>
            {
                { "teamId", teamId },
                { "newName", newName },
                { "newCity", newCity },
                { "newFounded", newFounded }
            };
            _graph.Query(command, parameters);
        }

        private void DeleteAll()
        {
            const string command = "MATCH (n) DETACH DELETE n";
            Console.WriteLine("Executing: " + command);
            _graph.Query(command);
        }
    }
}
