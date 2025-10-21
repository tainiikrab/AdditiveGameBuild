using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GoogleSpreadsheets;
using UnityEditor;
using UnityEngine;

public class SheetImporterMenu : MonoBehaviour
{
    private const string SPREADSHEET_ID = "1y3Q0j6O3PrVkYaaviVYe-8MDoKUqblHNaomzy6hRSt4";
    private const string TEST_SHEETS_NAME = "Test1";
    private const string customersSheetName = "Customers";
    private const string CREDENTIALS_PATH = "printer3dgame-881862586dbf.json";
    private const string savedConfig = "savedConfig";

    private static bool isEnabled = true;

    // public static GlobalConfig globalConfig { get; private set; }

    private void Awake()
    {
        GoogleSheetsImporter.logParsedItems = isEnabled;
        Menu.SetChecked("SheetImporter/Toggle console output", isEnabled);
        Debug.Log($"Google Sheets importer enabled: {isEnabled}");
    }


    // public static void ImportMenu()
    // {
    //     Debug.Log("Import menu called");
    //     Import();
    // }
    [MenuItem("SheetImporter/Import sheet")]
    public static async Task Import()
    {
        // Debug.Log("Import called");
        GoogleSheetsImporter.logParsedItems = isEnabled;
        var sheetsImporter = new GoogleSheetsImporter(CREDENTIALS_PATH, SPREADSHEET_ID);
        var globalConfig = new GlobalConfig();

        // Берём все публичные поля GlobalConfig типа List<>, где элемент реализует IConfig
        var fields = typeof(GlobalConfig).GetFields(BindingFlags.Public | BindingFlags.Instance);
        // Debug.Log($"FIELDS LENGTH: {fields.Length}");
        // Debug.Log($"field 1: {fields[1].Name}");
        foreach (var field in fields)
        {
            if (!field.FieldType.IsGenericType) continue;
            if (field.FieldType.GetGenericTypeDefinition() != typeof(List<>)) continue;

            var elementType = field.FieldType.GetGenericArguments()[0];
            if (!typeof(IConfig).IsAssignableFrom(elementType)) continue;

            var sheetName = field.Name; // имя листа = имя поля
            try
            {
                // Создаём ConfigParser<T> и приводим к ISheetParser
                var parserType = typeof(ConfigParser<>).MakeGenericType(elementType);
                var parserInstance = (ISheetParser)Activator.CreateInstance(parserType, globalConfig);

                Debug.Log($"[Import] Parsing sheet '{sheetName}' into {elementType.Name}...");
                await sheetsImporter.DownloadAndParseSheet(sheetName, parserInstance);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Import] Failed to parse sheet '{sheetName}' ({elementType.Name}): {ex}");
            }
        }

        var json = JsonUtility.ToJson(globalConfig, true);
        PlayerPrefs.SetString(savedConfig, json);
        Debug.Log(json);
    }

    [MenuItem("SheetImporter/Toggle console output")]
    private static void ToggleConsoleOutput()
    {
        isEnabled = !isEnabled;
        Menu.SetChecked("SheetImporter/Toggle console output", isEnabled);
    }

    [MenuItem("SheetImporter/Delete saves")]
    private static void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
    }

    public static GlobalConfig LoadConfig()
    {
        var jsonLoaded = PlayerPrefs.GetString(savedConfig);
        var GlobalConfig = !string.IsNullOrEmpty(jsonLoaded)
            ? JsonUtility.FromJson<GlobalConfig>(jsonLoaded)
            : new GlobalConfig();
        return GlobalConfig;
    }
}