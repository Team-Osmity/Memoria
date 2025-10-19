#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Memoria.Constants;

public class SheetsDataSyncWindow : EditorWindow
{
    [MenuItem("Tools/Google Sheets")]
    static void Open() => GetWindow<SheetsDataSyncWindow>("Google Sheets");

    string _sheetName;
    string _savePath;
    string _lastLog;
    Vector2 _scroll;

    void OnEnable() {
        _savePath = GoogleSheets.SAVE_PATH;
    }

    async void Pull(string sheetName = GoogleSheets.DEFAULT_SHEET_NAME)
    {
        Debug.Log(GoogleSheets.START_MESSAGE);
        string url = $"{GoogleSheets.API_URL}?sheet={sheetName}";
        using UnityWebRequest req = UnityWebRequest.Get(url);
        var operation = req.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching data: {req.error}");
            _lastLog = $"Fetch Error: {req.error}";
            return;
        }

        string raw = req.downloadHandler.text;
        Debug.Log(GoogleSheets.SUCCESS_MESSAGE);

        JToken payload;
        try
        {
            payload = JToken.Parse(raw);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Received non-JSON or invalid JSON. Saving as string. ({ex.Message})");
            payload = JValue.CreateString(raw);
        }

        var envelope = new JObject
        {
            ["sheetName"] = sheetName,
            ["fetchedAt"] = DateTime.UtcNow.ToString("o"),
            ["data"]      = payload
        };

        string pretty = envelope.ToString(Formatting.Indented);

        try
        {
            var dir = Path.GetDirectoryName(_savePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            File.WriteAllText(_savePath, pretty, new UTF8Encoding(encoderShouldEmitUTF8Identifier:false));

            AssetDatabase.ImportAsset(_savePath);

            _lastLog = $"Saved: {_savePath}\nBytes: {pretty.Length}";
        }
        catch (Exception ex)
        {
            Debug.LogError($"Save failed: {ex.Message}");
            _lastLog = $"Save Error: {ex.Message}";
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Google Sheets Data Sync", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.LabelField("Sheet Name:");
        _sheetName = EditorGUILayout.TextField(_sheetName);
        GUILayout.Space(15);

        if (GUILayout.Button("Pull from Google Sheets")) Pull(_sheetName);
        GUILayout.Space(10);


        _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(120));
        GUILayout.Label(_lastLog ?? "(log will appear here)");
        GUILayout.EndScrollView();
    }
}
#endif