using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;
using GSheets.Enums;
using GSheets.Models;

namespace GSheets
{
    public class GetMatchDay
    {
        static string path = $"https://match.uefa.com/v2/matches?matchId=";
        static string pathEvents = $"https://match.uefa.com/v2/matches/";
        static string matchday1 = $"2024452,2024453,2024454,2024441,2024442,2024447,2024448,2024449,2024450,2024451,2024455,2024456,";
        static string matchday2 = $"2024443,2024444,2024457,2024458,2024459,2024460,2024461,2024462,2024464,2024465,2024463,2024466,";
        static string matchday3 = $"2024445,2024446,2024467,2024468,2024469,2024470,2024471,2024472,2024473,2024474,2024475,2024476,";

        static string roundOf16 = $"2024477,2024478,2024479,2024480,2024481,2024482,2024483,2024484,";

        static string quaterFinals = $"2024485,2024486,2024487,2024488,";

        static string semiFinals = $"2024489,2024490,";

        static string final = $"2024491";

        static string eventsPath = "/events?filter=LINEUP&offset=0&limit=100";

        static string url = string.Concat(path, matchday1, matchday2, matchday3, roundOf16, quaterFinals, semiFinals, final);


        public List<MatchInfo> GetDataFromEndpoint()
        {
            RestHelper restHelper = new RestHelper();

            var client = restHelper.SetRestClient(url);
            var request = restHelper.CreateGetRequest(RestSharp.Method.GET);

            var response = restHelper.GetResponse(client, request);
            var content = restHelper.Content2<MatchInfo>(response);

            return content.OrderBy(x => x.kickOffTime.dateTime).ToList();

        }

        public MatchInfo GetSingleMatch(int MachId)
        {
            var url = path + MachId.ToString();

            RestHelper restHelper = new RestHelper();

            var client = restHelper.SetRestClient(url);
            var request = restHelper.CreateGetRequest(RestSharp.Method.GET);

            var response = restHelper.GetResponse(client, request);
            var content = restHelper.Content<MatchInfo>(response);

            return content;
        }

        public List<MatchEvents> GetSingleMatchEvents(int MachId)
        {
            var url = pathEvents + MachId.ToString() + eventsPath;



            RestHelper restHelper = new RestHelper();

            var client = restHelper.SetRestClient(url);
            var request = restHelper.CreateGetRequest(RestSharp.Method.GET);

            var response = restHelper.GetResponse(client, request);
            if (response.Content.Count() == 0)
            {
                return null;
            }
            var content = restHelper.Content2<MatchEvents>(response);

            return content;
        }

        public int CountEvents(List<MatchEvents> content, EventsEnum events)
        {
            int count = 0;
            foreach (var item in content)
            {
                if (item.type.ToString() == events.ToString())
                {
                    count++;
                }
            }
            return count;
        }

        public ValueRange PrepareSheetsEntries(List<MatchInfo> content)
        {
            ValueRange valueRange = new ValueRange();
            var tmp = new List<IList<object>>();
            string matchday = "";
            int rowNumber = 1;

            tmp.Add(new List<object>{"Grupa","Gospodarz","","Godzina","Goście","","id meczu","Data meczu",
            "czas aktualny","róznica w czasie","przeliczona na dni", "Status meczu", "Typ rozgrywki"});

            foreach (var item in content)
            {
                rowNumber++;
                IList<object> contentList = new List<object>();
                if (item.kickOffTime.date != matchday)
                {
                    matchday = item.kickOffTime.date;
                    //contentList.Add(groupTmp);
                    tmp.Add(new List<object>());
                    tmp.Add(new List<object>() { "", "", "", matchday });
                    rowNumber = rowNumber + 2;
                }

                string imageHome = $@"=IMAGE(""{item.homeTeam.logoUrl}"")";
                string imageAway = $@"=IMAGE(""{item.awayTeam.logoUrl}"")";
                var matchHour = item.kickOffTime.dateTime.ToLocalTime().ToString("t");
                var ifGroupExist = item.group != null ? item.group.metaData.groupName : "";
                var groupLink = ifGroupExist != "" ? $@"=HYPERLINK(""https://www.uefa.com/uefaeuro-2020/match/{item.id}/standings"";""{ifGroupExist}"")" : "";

                contentList.Add(groupLink);
                //item.kickOffTime.dateTime.AddHours(item.kickOffTime.utcOffsetInHours).ToLocalTime();
                contentList.Add(item.homeTeam.internationalName);
                contentList.Add(imageHome);
                contentList.Add($@"=HYPERLINK(""https://www.uefa.com/uefaeuro-2020/match/{item.id}"";""{matchHour}"")");
                contentList.Add(item.awayTeam.internationalName);
                contentList.Add(imageAway);
                contentList.Add(item.id);
                // contentList.Add(item.kickOffTime.dateTime.AddHours(item.kickOffTime.utcOffsetInHours).ToString("yyyy-MM-dd HH:mm:ss"));
                contentList.Add(item.kickOffTime.dateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                contentList.Add("=Now()");
                contentList.Add($@"=if(I{rowNumber} > H{rowNumber}; ""00:00""; H{rowNumber}-I{rowNumber})");
                contentList.Add($@"=IFS(INT(J{rowNumber})<0; ""00:00""; INT(J{rowNumber})>=1;CONCATENATE(INT(J{rowNumber});"" dni ""; HOUR(J{rowNumber}); "" g "");  INT(J{rowNumber})=0; CONCATENATE(HOUR(J{rowNumber}); "" g: "";MINUTE(J{rowNumber}); "" min""))");
                contentList.Add(item.status);
                contentList.Add(item.type);
                if (item.score != null)
                {
                    contentList.Add(item.score.regular.home + ":" + item.score.regular.away);
                    //contentList.Add(item.score.penalty);
                    if (item.winner.match.reason == "DRAW")
                    {
                        contentList.Add(item.winner.match.reason);
                    }
                    else
                    {
                        contentList.Add(item.winner.match.team.internationalName);
                    }
                }
                tmp.Add(contentList);
            }
            valueRange.Values = tmp;

            return valueRange;
        }

        public void UpdateStatus()
        {
            var sheetName = SheetsEnum.Dane;
            var sh = new SheetsHelper();
            var readed = sh.Read(sheetName, "A1", "Q96");

            int rowCount = 0;

            foreach (var match in readed.Values)
            {
                rowCount++;

                if (match.Count < 5)
                {
                    continue;
                }
                if (match[1].ToString() != "" && match[1].ToString() != "Gospodarz")
                {
                    if (match[(int)DataColumnName.roznica_w_czasie].ToString() != "00:00")
                    {
                        double divInTime = double.Parse(match[(int)DataColumnName.roznica_w_czasie].ToString());
                        if (divInTime > 1)
                        {
                            continue;
                        }
                    }
                                         
                    int rok = int.Parse(match[(int)DataColumnName.Data_meczu].ToString().Split(' ')[0].Split('-')[0]);
                    int mc = int.Parse(match[(int)DataColumnName.Data_meczu].ToString().Split(' ')[0].Split('-')[1]);
                    int dzien = int.Parse(match[(int)DataColumnName.Data_meczu].ToString().Split(' ')[0].Split('-')[2]);

                    int godz = int.Parse(match[(int)DataColumnName.Data_meczu].ToString().Split(' ')[1].Split(':')[0]);
                    int min = int.Parse(match[(int)DataColumnName.Data_meczu].ToString().Split(' ')[1].Split(':')[1]);
                    int sec = int.Parse(match[(int)DataColumnName.Data_meczu].ToString().Split(' ')[1].Split(':')[2]);
                    DateTime MatchDate = new DateTime(rok,mc,dzien,godz,min,sec);
                    // if ((MatchDate - DateTime.Now).Days <-15)
                    // {
                    //         continue;
                    // }

                    if (match[11].ToString() == StatusEnum.UPCOMING.ToString() || match[11].ToString() == StatusEnum.LIVE.ToString()
                    || match[11].ToString() == StatusEnum.FINISHED.ToString())
                    {

                        MatchInfo matchRead = new MatchInfo();
                        //MatchEvents marchReadEvents = new MatchEvents();
                        List<MatchEvents> marchReadEvents = new List<MatchEvents>();
                        matchRead = GetSingleMatch(int.Parse(match[6].ToString()));
                        marchReadEvents = GetSingleMatchEvents(int.Parse(match[6].ToString()));

                        int yellowCards = CountEvents(marchReadEvents, EventsEnum.YELLOW_CARD);
                        int redCards = CountEvents(marchReadEvents, EventsEnum.RED_CARD);
                        int redYellowCards = CountEvents(marchReadEvents, EventsEnum.RED_YELLOW_CARD);

                        if (matchRead.status == StatusEnum.FINISHED.ToString())
                        {
                            ValueRange update = new ValueRange();
                            List<IList<object>> values = new List<IList<object>>();
                            List<object> rowValue = new List<object>();
                            rowValue.Add(matchRead.status);
                            rowValue.Add(matchRead.type);
                            rowValue.Add($"{matchRead.score.total.home}:{matchRead.score.total.away}");
                            if (matchRead.winner.match.reason == "WIN_REGULAR" || 
                            matchRead.winner.match.reason == "WIN_ON_EXTRA_TIME" || 
                            matchRead.winner.match.reason ==  "WIN_ON_PENALTIES")
                            {
                                rowValue.Add(matchRead.winner.match.team.internationalName);
                            }
                            else if (matchRead.winner.match.reason == "DRAW")
                            {
                                rowValue.Add(matchRead.winner.match.reason);
                            }
                            rowValue.Add(yellowCards);
                            rowValue.Add(redYellowCards + redCards);
                            if (matchRead.score.penalty != null)
                            {
                                rowValue.Add($"{matchRead.score.penalty.home}:{matchRead.score.penalty.away}");
                            }
                            else
                            {
                                rowValue.Add("brak");
                            }
                            rowValue.Add(DateTime.Now.ToString("dd-MM-yy hh:mm:ss"));
                            //rowValue.Add($"{matchRead.score.penalty.home}:{matchRead.score.penalty.home}");

                            values.Add(rowValue);
                            update.Values = values;
                            // parsuje enum na inny enum
                            string colStatusName = ((ColumnEnum)DataColumnName.Status_meczu).ToString();
                            string colScoreName = ((ColumnEnum)DataColumnName.Wynik).ToString();
                            string colPenetlyName = ((ColumnEnum)DataColumnName.Wynik_po_rzutach_karnych).ToString();

                            System.Threading.Thread.Sleep(1002);
                            Console.WriteLine($"update match {matchRead.homeTeam.internationalName} vs {matchRead.awayTeam.internationalName}");
                            sh.UpdateCell(sheetName, $"{colStatusName}{rowCount}", update);
                        }

                        if (matchRead.status == StatusEnum.LIVE.ToString())
                        {
                            ValueRange update = new ValueRange();
                            List<IList<object>> values = new List<IList<object>>();
                            List<object> rowValue = new List<object>();
                            rowValue.Add(matchRead.status);
                            rowValue.Add(matchRead.type);
                            rowValue.Add($"{matchRead.score.total.home}:{matchRead.score.total.away}");
                            rowValue.Add("Live");
                            rowValue.Add(yellowCards);
                            rowValue.Add(redYellowCards + redCards);
                            if (matchRead.score.penalty != null)
                            {
                                rowValue.Add($"{matchRead.score.penalty.home}:{matchRead.score.penalty.away}");
                            }
                            else
                            {
                                rowValue.Add("brak");
                            }
                            rowValue.Add(DateTime.Now.ToString("dd-MM-yy hh:mm:ss"));

                            values.Add(rowValue);

                            update.Values = values;
                            // parsuje enum na inny enum
                            string colStatusName = ((ColumnEnum)DataColumnName.Status_meczu).ToString();
                            string colScoreName = ((ColumnEnum)DataColumnName.Wynik).ToString();
                            string colPenetlyName = ((ColumnEnum)DataColumnName.Wynik_po_rzutach_karnych).ToString();
                            sh.UpdateCell(sheetName, $"{colStatusName}{rowCount}", update);
                        }


                    }
                }
            }

        }

        public void AddProtectedRange()
        {
            var sheetName = SheetsEnum.Dane;
            var sh = new SheetsHelper();
            var readed = sh.Read(sheetName, "A1", "Q63");

            int rowCount = 0;
            foreach (var match in readed.Values)
            {
                rowCount++;

                if (match.Count < 5)
                {
                    continue;
                }
                // if(match[1].ToString() != "" && match[1].ToString() != "Gospodarz")
                // {
                //       if (match[11].ToString() == StatusEnum.LIVE.ToString() || match[11].ToString() == StatusEnum.FINISHED.ToString())
                //       {

                //       }
                // }
                if (match[11].ToString() == StatusEnum.UPCOMING.ToString())
                {
                    break;
                }
            }
            sh.ProtectedRange(43786814, ColumnEnum.L, ColumnEnum.Z, 8, rowCount + 6, 790);
            if (rowCount > 65)
            {
                sh.ProtectedRange(48770365, ColumnEnum.L, ColumnEnum.Z, 8, rowCount + 6 - 66, 791);
            }
        }

    }
}