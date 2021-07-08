using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GSheets.Enums;
using GSheets.Models;

namespace GSheets
{
    class Program
    {


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SheetsHelper Sheets = new SheetsHelper();


            var getMatchDay = new GetMatchDay();
            var getGroupDetails = new GetGroupDetails();
            var addPlayers = new AddPlayers(); 

            //dodawnie graczy
            //var valueRangePlyers = addPlayers.AddPlayer("Json/players.json");
//var valueRangePlyersMistrz =  getGroupDetails.AddPlayerToMistrz("Json/players.json");

            //Sheets.Write2(SheetsEnum.Grupy, "L2", valueRangePlyers);
           // Sheets.Write2(SheetsEnum.FazaPucharowa, "L2", valueRangePlyers);
            
            /// dodawanie do tabeli Mistrz UEFA 2020
         //   Sheets.Write2(SheetsEnum.MistrzEuropy,"A1", valueRangePlyersMistrz);
          //  addPlayers.AddPoints(SheetsEnum.Grupy, SheetsEnum.Awans_z_grupy, SheetsEnum.MistrzEuropy);
            //getGroupDetails.addDataValidationToMistrz();

         //  addPlayers.addDataValidation(SheetsEnum.FazaPucharowa,48770365);
           //addPlayers.addDataValidation(SheetsEnum.Grup, 43786814);
            //getGroupDetails.addDataValidation(SheetsEnum.Awans_z_grupy);
//addPlayers.AddPoints(SheetsEnum.Grupy, SheetsEnum.Awans_z_grupy);

//Console.WriteLine("dodano graczy");
            
            //// Awans z grupy
            //var valueRangeGroup = getGroupDetails.PrepareSheetsEntries(getGroupDetails.GetDataFromEndpoint());
            //var valueRaneeGroupAddPlayers = getGroupDetails.AddPlayers("Json/Players.json");
            //getGroupDetails.GetDataFromEndpoint();
           ////// getGroupDetails.updateData();
            // wYNIKI W grupie
            


            //Sheets.SetDataValidationPost();

            // mecze 
             //var valueRange = getMatchDay.PrepareSheetsEntries(getMatchDay.GetDataFromEndpoint());
            // Sheets.Write2(SheetsEnum.Dane, "A1", valueRange);
            
            //Update meczu
           // Sheets.ProtectedRange(43786814,ColumnEnum.L,ColumnEnum.Z, 8, 11);
            Console.WriteLine("Update statusów start");
             var valueRange = getMatchDay.PrepareSheetsEntries(getMatchDay.GetDataFromEndpoint());
            Sheets.ClearSheet(SheetsEnum.Dane,"A", "M");
              Sheets.Write2(SheetsEnum.Dane, "A1", valueRange);
             getMatchDay.UpdateStatus();
            getMatchDay.AddProtectedRange();
            Console.WriteLine("Update statusów end");
            Console.WriteLine("AddPoints Start");
             addPlayers.AddPoints(SheetsEnum.Grupy, SheetsEnum.Awans_z_grupy, SheetsEnum.MistrzEuropy, SheetsEnum.FazaPucharowa);
            Console.WriteLine("AddPoints End");
            /// wypisuje grupy
            
            //getGroupDetails.addDataValidation(SheetsEnum.Arkusz2);
            
            //dodawnie danych do grupy
             var valueRangeGroup = getGroupDetails.PrepareSheetsEntries(getGroupDetails.GetDataFromEndpoint());
            // var valueRaneeGroupAddPlayers = getGroupDetails.AddPlayers("Json/Players.json");
            // Sheets.Write2(SheetsEnum.Awans_z_grupy, "A1", valueRangeGroup);
            // Sheets.Write2(SheetsEnum.Awans_z_grupy, "O2", valueRaneeGroupAddPlayers);
            //  getGroupDetails.addDataValidation(SheetsEnum.Awans_z_grupy);
            



            Console.WriteLine("End Program");

        }


    }
}
