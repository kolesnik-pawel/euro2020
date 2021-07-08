using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Requests;
using GSheets.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using Google.Apis.Util.Store;

namespace GSheets
{
    public class SheetsHelper
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        static readonly string ApplicationName = "Legislators";

        static readonly string SpreadsheetId = "1o_DOHZdmXc8y2IUaPwBTJMFjWfSZ0M9xhRXFJxhg8dY";

        //static readonly string sheet = "Arkusz1";

        static SheetsService service;
        
        public SheetsHelper()
        {
            GoogleCredential credential ;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);

            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            
        }
        
        public ValueRange Read(SheetsEnum sheet, string startCell, string endCell)
        {
            var range = $"{sheet}!{startCell}:{endCell}";

            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            var values = response.Values;

            return response;

        }

        public void Write(SheetsEnum sheet, string startCell, IList<object> values)
        {
            var range = $"{sheet}!{startCell}";
            var valueRange = new ValueRange();
            var tmp = new List<IList<object>>();
            tmp.Add(values);

            valueRange.Values = tmp;

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = appendRequest.Execute();
        }

        public void Write2(SheetsEnum sheet, string startCell, ValueRange valueRange)
        {
            var range = $"{sheet}!{startCell}";
           // var valueRange = new ValueRange();
            // var tmp = new List<IList<object>>();
            // tmp.Add(values);

            //valueRange.Values = values;

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = appendRequest.Execute();
        }
        public void ClearSheet(SheetsEnum sheet)
        {
            var range = $"{sheet}!A:ZZ";
            var request = service.Spreadsheets.Values.Clear(new ClearValuesRequest(), SpreadsheetId, range);

            var appendResponse = request.Execute();
        }

        public void ClearSheet(SheetsEnum sheet, string startCell, string endCell)
        {
            var range = $"{sheet}!{startCell}:{endCell}";
            var request = service.Spreadsheets.Values.Clear(new ClearValuesRequest(), SpreadsheetId, range);

            var appendResponse = request.Execute();
        }

        public void UpdateCell(SheetsEnum sheet, string cell, ValueRange value)
        {
            var range = $"{sheet}!{cell}";

            var UpdateRequest = service.Spreadsheets.Values.Update(value, SpreadsheetId, range);
            
            UpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = UpdateRequest.Execute();
            
        }

        public List<ConditionValue> ConditionValidationList(List<string> conditionList)
        {
            List<ConditionValue> conditionValueList = new List<ConditionValue>();
            foreach (var condition in conditionList)
            {
                ConditionValue  conditionValue = new ConditionValue(){UserEnteredValue = condition};
                conditionValueList.Add(conditionValue);
            }

            return conditionValueList;

        }
        public void DataValidation(int sheetId, ColumnEnum StartColum, ColumnEnum EndColumn, int RowStart, int RowEnd)
        {
            List<Request> body = new List<Request>();
            Request dataValidation = new Request();
            GridRange range = new GridRange();

            range.SheetId = sheetId;
            range.StartColumnIndex = (int?)StartColum;
            range.EndColumnIndex = (int?)EndColumn;
            range.StartRowIndex = RowStart;
            range.EndRowIndex = RowEnd;

            DataValidationRule rule = new DataValidationRule();

            BooleanCondition condition = new BooleanCondition();
            condition.Type =  "ONE_OF_LIST";
            ConditionValue  conditionValue1 = new ConditionValue(){UserEnteredValue = "Tak"};
            ConditionValue  conditionValue2 = new ConditionValue(){UserEnteredValue = "Nie"};
            
            List<ConditionValue> conditionValueList = new List<ConditionValue>();
            conditionValueList.Add(conditionValue1);
            conditionValueList.Add(conditionValue2);
            condition.Values = conditionValueList;

            rule.Condition = condition;
            
            SetDataValidationRequest dataValidationRequest = new SetDataValidationRequest();

            dataValidationRequest.Range = range;
            dataValidationRequest.Rule = rule;

            dataValidation.SetDataValidation = dataValidationRequest;
            
            BatchUpdateSpreadsheetRequest request = new BatchUpdateSpreadsheetRequest();

            request.Requests = new List<Request>(){dataValidation};

            var set = service.Spreadsheets.BatchUpdate(request, SpreadsheetId);

            set.Execute();
        }
        public void DataValidation(int sheetId, ColumnEnum StartColum, ColumnEnum EndColumn, int RowStart, int RowEnd, List<ConditionValue> conditionValueList)
        {
            List<Request> body = new List<Request>();
            Request dataValidation = new Request();
            GridRange range = new GridRange();

            range.SheetId = sheetId;
            range.StartColumnIndex = (int?)StartColum;
            range.EndColumnIndex = (int?)EndColumn;
            range.StartRowIndex = RowStart;
            range.EndRowIndex = RowEnd;

            DataValidationRule rule = new DataValidationRule();

            BooleanCondition condition = new BooleanCondition();
            condition.Type =  "ONE_OF_LIST";
            // ConditionValue  conditionValue1 = new ConditionValue(){UserEnteredValue = "Tak"};
            // ConditionValue  conditionValue2 = new ConditionValue(){UserEnteredValue = "Nie"};
            
            // List<ConditionValue> conditionValueList = new List<ConditionValue>();
            // conditionValueList.Add(conditionValue1);
            // conditionValueList.Add(conditionValue2);
            condition.Values = conditionValueList;

            rule.Condition = condition;
            
            SetDataValidationRequest dataValidationRequest = new SetDataValidationRequest();

            dataValidationRequest.Range = range;
            dataValidationRequest.Rule = rule;

            dataValidation.SetDataValidation = dataValidationRequest;
            
            BatchUpdateSpreadsheetRequest request = new BatchUpdateSpreadsheetRequest();

            request.Requests = new List<Request>(){dataValidation};

            var set = service.Spreadsheets.BatchUpdate(request, SpreadsheetId);

            set.Execute();
        }
        public void ProtectedRange(int sheetId, ColumnEnum StartColum, ColumnEnum EndColumn, int RowStart, int RowEnd, int ProtectedRangeId)
        {
           //var tmp = service.Spreadsheets.Values.Get(SpreadsheetId, "A1:Z20").Service.;
            List<Request> body = new List<Request>();
            Request dataProtected = new Request();
            GridRange range = new GridRange();
            ProtectedRange protectedRange = new ProtectedRange();
            protectedRange.ProtectedRangeId = ProtectedRangeId;

            range.SheetId = sheetId;
            range.StartColumnIndex = (int?)StartColum;
            range.EndColumnIndex = (int?)EndColumn;
            range.StartRowIndex = RowStart;
            range.EndRowIndex = RowEnd;

            protectedRange.Range = range;

           // protectedRange.NamedRangeId = "Nie ma obstawiania";

            Editors usersCanEdit = new Editors();
            usersCanEdit.Users = new List<string>();
            usersCanEdit.Users.Add("sancho0510@gmail.com");
            usersCanEdit.Users.Add("sheets@fluid-isotope-311615.iam.gserviceaccount.com");
            protectedRange.Editors = usersCanEdit;
            
            // dataProtected.AddProtectedRange =  new AddProtectedRangeRequest();
            // dataProtected.AddProtectedRange.ProtectedRange = protectedRange;
            
            dataProtected.UpdateProtectedRange = new UpdateProtectedRangeRequest();
            dataProtected.UpdateProtectedRange.ProtectedRange = protectedRange;
            dataProtected.UpdateProtectedRange.Fields = "*";
            
            BatchUpdateSpreadsheetRequest request = new BatchUpdateSpreadsheetRequest();

            request.Requests = new List<Request>(){dataProtected};
            //request.Requests[0].

            var set = service.Spreadsheets.BatchUpdate(request, SpreadsheetId);
            //service.Spreadsheets.Get();

            set.Execute();
            
        }

        public void SetDataValidationPost()
        {
            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{SpreadsheetId}:batchUpdate";
            RestHelper restHelper = new RestHelper();

            
            
            var client = restHelper.SetRestClient(url);
            var request = restHelper.CreateGetRequest(RestSharp.Method.POST);
           // request.AddJsonBody(JsonConvert.SerializeObject(new FileStream("Json//SetDataValidation.json",FileMode.Open, FileAccess.Read)));
            request.AddJsonBody("Json//SetDataValidation.json");
            var response = restHelper.GetResponse(client, request);
            //var content = restHelper.Content2<Match>(response);
        }

    }
}