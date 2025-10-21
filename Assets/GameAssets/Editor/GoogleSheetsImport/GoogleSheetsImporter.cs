using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

namespace GoogleSpreadsheets
{
    public class GoogleSheetsImporter
    {
        private readonly List<string> _headers = new();
        private readonly SheetsService _service;
        private readonly string _spreadsheetId;

        public GoogleSheetsImporter(string credentialsPath, string spreadsheetId)
        {
            _spreadsheetId = spreadsheetId;

            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }

            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }

        public static bool logParsedItems { get; set; } = false;

        private void TryLog(string log)
        {
            if (logParsedItems) Debug.Log(log);
        }

        public async Task DownloadAndParseSheet(string sheetName, ISheetParser parser)
        {
            TryLog($"Starting downloading sheet (${sheetName})...");

            var range = $"{sheetName}!A1:Z";
            var request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
            ValueRange response;
            try
            {
                response = await request.ExecuteAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error retrieving Google Sheets data: {e.Message}");
                return;
            }

            if (response != null && response.Values != null)
            {
                var tableArray = response.Values;
                TryLog($"Sheet downloaded successfully: {sheetName}. Parsing started.");

                _headers.Clear();
                var firstRow = tableArray[0];
                var x = tableArray[0].Count;
                Debug.LogWarning($"Count: {x}");
                for (var i = 0; i < x; i++)
                {
                    _headers.Add(tableArray[0][i].ToString());
                    if (logParsedItems) Debug.Log(tableArray[0][i]);
                }


                var rowsCount = tableArray.Count;
                for (var i = 1; i < rowsCount; i++)
                {
                    var row = tableArray[i];
                    var rowLength = row.Count;

                    for (var j = 0; j < _headers.Count; j++)
                    {
                        var header = _headers[j];
                        var cell = j < row.Count ? row[j]?.ToString() : "";
                        if (string.IsNullOrWhiteSpace(cell))
                            continue;

                        parser.Parse(header, cell);
                    }
                }

                TryLog($"Sheet {sheetName} parsed successfully.");
            }
            else
            {
                Debug.LogWarning("No data found in Google Sheets.");
            }
        }
    }
}