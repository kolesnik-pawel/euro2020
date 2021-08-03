# euro2020

With my friend deside to bets matches results at euro2020 tourments. To freandly manage bets I decide to use a Google Sheest. 
Google Sheets is easy to use, all of my frends has a google account and its best choise for us.

I'm lazy man and I do not want manually puts all matches and results, and guard the honesty of users.

To manage all of them I use a few frameworks, many taime of analize a inserts data, reads/watch many tutorials. But, starting from beginning.

The page of organiser the tournament - ufea.com. At this page I found a endpoints who returns necesary data. Every match at tournament have a unicaly ID. 
All of IDs was know before tournament.The endpoint v2/matches?matchId=2024452 returns json with full information about that match. Include teams, logo, 
match status, score, winner team and match more. This same endpoint taks list of match ids. 
Second endpoint https://standings.uefa.com/v1/standings?groupIds  allows a list of tourments groups. Returns list of the Groups with teams, teams points, 
qualification to second round, wins, lost, drows and more.

I use a RestSharp framework to setup connection, send request and get response.

Returnded json was parsed to the objects. 

Google share api to servising Google Sheets: Google.Apis.Sheets.v4.
This api allows Read, Write, Update cells at the specific sheet. 
Based on the apis above I wrote  simple aplication. 
My aplication 
1. gets data from uefa.com endpoints 
2. analize jsons and parsed to objects.
3. Inserts/updates analyzed data to the specific row
4. If the kick off time was exceeded the cells to insert match resalts, was blocked to entres 
5. Match results was updates every 5 min (setup at windows task scheduler)
6. For singe user was count points and update at Sheest


Below share link to Google Sheets used at that project.
https://docs.google.com/spreadsheets/d/1o_DOHZdmXc8y2IUaPwBTJMFjWfSZ0M9xhRXFJxhg8dY/edit?usp=sharing

Used tools
Visual Studio CODE
Visual Studio CODE Extensions: 
  
