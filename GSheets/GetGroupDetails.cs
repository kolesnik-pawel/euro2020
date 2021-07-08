
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Sheets.v4.Data;
using GSheets.Enums;
using GSheets.Models;
using Newtonsoft.Json;

namespace GSheets
{
    public class GetGroupDetails
    {
        static string path = $"https://standings.uefa.com/v1/standings?groupIds=";

        static string groupsId = $"2006438,2006439,2006440,2006441,2006442,2006443";
        static string url = string.Concat(path, groupsId);


        public List<Group> GetDataFromEndpoint()
        {
            RestHelper restHelper = new RestHelper();

            var client = restHelper.SetRestClient(url);
            var request = restHelper.CreateGetRequest(RestSharp.Method.GET);

            var response = restHelper.GetResponse(client, request);
            var content = restHelper.Content2<Group>(response);

            return content.OrderBy(x => x.group.metaData.groupShortName).ToList();

        }


        public ValueRange PrepareSheetsEntries(List<Group> content)
        {
            ValueRange valueRange = new ValueRange();
            var tmp = new List<IList<object>>();
            string groupName = "";
            int rowNumber = 0;

            foreach (var item in content)
            {
                rowNumber++;
                //IList<object> contentList2 = new List<object>();
                if (item.group.metaData.groupName != groupName)
                {
                    groupName = item.group.metaData.groupName;
                    //contentList.Add(groupTmp);
                    tmp.Add(new List<object>());
                    tmp.Add(new List<object>() { "", "", "", groupName });
                    tmp.Add(new List<object>() { "", "", "", "mecze", "wygrane", "Remisy", "Przegrane", "Gole", "Gole stracone", "Różnica", "Punkty", "Kwalifikacja", });
                    rowNumber = rowNumber + 2;
                }

                foreach (var itemTeam in item.items)
                {
                    IList<object> contentList = new List<object>();
                    contentList.Add(item.group.metaData.groupName);
                    string imageFlage = $@"=IMAGE(""{itemTeam.team.logoUrl}"")";

                    contentList.Add(imageFlage);
                    contentList.Add(itemTeam.team.internationalName);

                    contentList.Add(itemTeam.played.GetHashCode());
                    contentList.Add(itemTeam.won);
                    contentList.Add(itemTeam.drawn);
                    contentList.Add(itemTeam.lost);
                    contentList.Add(itemTeam.goalsFor);
                    contentList.Add(itemTeam.goalsAgainst);
                    contentList.Add(itemTeam.goalDifference);
                    contentList.Add(itemTeam.points);
                    contentList.Add(itemTeam.qualified);
                    contentList.Add(itemTeam.rank);
                    tmp.Add(contentList);
                }

            }
            valueRange.Values = tmp;

            return valueRange;
        }

        public ValueRange AddPlayers(string path)
        {
            var file = System.IO.File.ReadAllText(path);

            var players = JsonConvert.DeserializeObject<List<Player>>(file);
            ValueRange valueRange = new ValueRange();
            var tmp = new List<IList<object>>();
            IList<object> contentList = new List<object>();
            foreach (var player in players)
            {
                contentList.Add(player.Name);
            }
            tmp.Add(contentList);

            valueRange.Values = tmp;

            return valueRange;
        }

        public void addDataValidation(SheetsEnum sheet)
        {
            string startCell = "C1";
            string endCell = "C42";
            var sh = new SheetsHelper();
            int rownumber = 0;
            var readed = sh.Read(sheet, startCell, endCell);

            List<string> conditions = new List<string>() { "Tak", "Nie", "1st", "2nd" };
            sh.ConditionValidationList(conditions);

            foreach (var row in readed.Values)
            {
                if (row.Count != 0)
                {
                    System.Threading.Thread.Sleep(100);
                    sh.DataValidation(220905523, Enums.ColumnEnum.O, Enums.ColumnEnum.V, rownumber, rownumber + 1, sh.ConditionValidationList(conditions));
                }
                rownumber++;
            }

        }

        public void updateData()
        {
            List<Group> groups = GetDataFromEndpoint();
            var sh = new SheetsHelper();
            var readed = sh.Read(SheetsEnum.Awans_z_grupy, "A1", "N42");

            //   ValueRange valueRange = new ValueRange();
            // var tmp = new List<IList<object>>();

            int rowcount = 0;
            foreach (var row in readed.Values)
            {
                rowcount++;
                Console.WriteLine($"Wiersz nr:  {rowcount}");
                if (row.Count >= 13)
                {
                    ValueRange valueRange = new ValueRange();
                    var tmp = new List<IList<object>>();
                    TeamAtGroup team = null;
                    foreach (var group in groups)
                    {

                        var zep = row[2].ToString();
                        var selected = group.items.Where(x => x.team.internationalName == row[2].ToString());
                        Console.WriteLine($"Sprawdzanie dla {row[2].ToString()}");

                        try
                        {
                            team = (TeamAtGroup)group.items.Where(x => x.team.internationalName == row[2].ToString()).First();
                        }
                        catch (System.Exception)
                        {

                            continue;
                        }

                        if (selected != null)
                        {
                            team = (TeamAtGroup)selected.First();
                        }
                        if (team == null) { 
                            Console.WriteLine("pole team == null");
                            break; }

                    }
                    IList<object> contentList = new List<object>();
                    contentList.Add(team.played.GetHashCode());
                    contentList.Add(team.won);
                    contentList.Add(team.drawn);
                    contentList.Add(team.lost);
                    contentList.Add(team.goalsFor);
                    contentList.Add(team.goalsAgainst);
                    contentList.Add(team.goalDifference);
                    contentList.Add(team.points);
                    contentList.Add(team.qualified);
                    contentList.Add(team.rank);
                    tmp.Add(contentList);
                    valueRange.Values = tmp;

                    sh.UpdateCell(SheetsEnum.Awans_z_grupy, $"{ColumnEnum.D.ToString()}{rowcount}", valueRange);
                }
            }

        }

        public ValueRange AddPlayerToMistrz(string path)
        {
            var file = System.IO.File.ReadAllText(path);

            var players = JsonConvert.DeserializeObject<List<Player>>(file);
            ValueRange valueRange = new ValueRange();
            var tmp = new List<IList<object>>();
            IList<object> contentList = new List<object>();
            IList<object> gameList = new List<object>();
            // dodaje userów
            contentList.Add("");
            foreach (var player in players)
            {
                contentList.Add(player.Name);

            }
            tmp.Add(contentList);
            tmp.Add(new List<object>());
            tmp.Add(new List<object>() { Enums.TourmentClassificationEnum.Winner.ToString() });
            tmp.Add(new List<object>() { Enums.TourmentClassificationEnum.Second.ToString() });
            tmp.Add(new List<object>() { Enums.TourmentClassificationEnum.Third.ToString() });
            valueRange.Values = tmp;

            return valueRange;
        }

        public void addDataValidationToMistrz()
        {
            var sh = new SheetsHelper();


            List<Group> groups = GetDataFromEndpoint();
            List<string> conditions = new List<string>();

            // groups.OrderBy(x => x.items.OrderBy(y => y.team.internationalName).)

            foreach (var row in groups)
            {
                foreach (var teamAtRow in row.items)
                {
                    conditions.Add(teamAtRow.team.internationalName);

                }
            }
            conditions.Sort();
            sh.DataValidation(22330502, Enums.ColumnEnum.B, Enums.ColumnEnum.I, 2, 5, sh.ConditionValidationList(conditions));
        }
    }
}