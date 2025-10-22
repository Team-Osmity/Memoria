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
using Memoria.Systems;

public class SheetsDataSyncWindow : EditorWindow
{
    [MenuItem("Tools/Google Sheets")]
    static void Open() => GetWindow<SheetsDataSyncWindow>("Google Sheets");

    enum ObfMode { None, Simple, AES }
    ObfMode _mode = ObfMode.Simple;
    string _aesPassphrase = "";

    string _sheetName;
    string _savePathAbs;
    string _savePathRelDisplay;
    string _lastLog;
    Vector2 _scroll;

    void OnEnable()
    {
        _savePathAbs = ResolveSavePath(GoogleSheets.SAVE_PATH);
        _savePathRelDisplay = MakeNiceRelative(_savePathAbs, Application.persistentDataPath);
    }

    static string ResolveSavePath(string relative)
    {
        if (string.IsNullOrEmpty(relative)) relative = "Save/Parameters.sav";
        string dir = Path.Combine(Application.persistentDataPath, Path.GetDirectoryName(relative) ?? "");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, Path.GetFileName(relative));
    }

    static string MakeNiceRelative(string abs, string root)
    {
        try
        {
            var a = Path.GetFullPath(abs).Replace('\\','/');
            var r = Path.GetFullPath(root).Replace('\\','/');
            if (a.StartsWith(r)) return a.Substring(r.Length).TrimStart('/');
            return abs;
        }
        catch { return abs; }
    }

    async void Pull(string sheetName = null)
    {
        sheetName ??= GoogleSheets.DEFAULT_SHEET_NAME;

        try
        {
            EditorUtility.DisplayProgressBar("Google Sheets", "Fetching...", 0.2f);
            Debug.Log(GoogleSheets.START_MESSAGE);

            string url = $"{GoogleSheets.API_URL}?sheet={UnityWebRequest.EscapeURL(sheetName)}";
            using var req = UnityWebRequest.Get(url);
            req.timeout = 15;

            var op = req.SendWebRequest();
            while (!op.isDone) await Task.Yield();

#if UNITY_2020_2_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                string msg = $"Error fetching data: {req.error}";
                Debug.LogError(msg);
                _lastLog = msg;
                return;
            }

            string raw = req.downloadHandler.text;
            Debug.Log(GoogleSheets.SUCCESS_MESSAGE);

            JToken payload;
            try { payload = JToken.Parse(raw); }
            catch (Exception ex)
            {
                Debug.LogWarning($"Invalid JSON, saving as string. ({ex.Message})");
                payload = JValue.CreateString(raw);
            }

            var envelope = new JObject
            {
                ["sheetName"] = sheetName,
                ["fetchedAt"] = DateTime.UtcNow.ToString("o"),
                ["data"]      = payload
            };

            string pretty = envelope.ToString(Formatting.Indented);

            string toSave;
            string obfInfo;
            switch (_mode)
            {
                case ObfMode.None:
                    toSave = pretty;
                    obfInfo = "None";
                    break;
                case ObfMode.Simple:
                    toSave = JsonObfuscator.Encode(pretty);
                    obfInfo = "Simple(XOR+Base64)";
                    break;
                case ObfMode.AES:
                    if (string.IsNullOrEmpty(_aesPassphrase))
                    {
                        _lastLog = "AES mode selected but passphrase is empty. Aborted.";
                        Debug.LogError(_lastLog);
                        return;
                    }
                    toSave = JsonObfuscator.AesEncrypt(pretty, _aesPassphrase);
                    obfInfo = "AES(PBKDF2+CBC)";
                    break;
                default:
                    toSave = pretty;
                    obfInfo = "Unknown";
                    break;
            }

            EditorUtility.DisplayProgressBar("Google Sheets", "Saving...", 0.6f);
            AtomicWriteUtf8NoBom(_savePathAbs, toSave);

            var bytes = Encoding.UTF8.GetByteCount(pretty);
            _lastLog = $"Saved to: {_savePathAbs}\n({Application.persistentDataPath}/ {_savePathRelDisplay})\nBytes: {bytes}\nMode: {obfInfo}\nUTC: {DateTime.UtcNow:o}";
            Debug.Log(_lastLog);
        }
        catch (Exception ex)
        {
            string msg = $"Save failed: {ex.Message}";
            Debug.LogError(msg);
            _lastLog = msg;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            Repaint();
        }
    }

    static void AtomicWriteUtf8NoBom(string absPath, string content)
    {
        var tmp = absPath + ".tmp";
        File.WriteAllText(tmp, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        if (File.Exists(absPath)) File.Replace(tmp, absPath, null);
        else File.Move(tmp, absPath);
    }

    void OnGUI()
    {
        GUILayout.Label("Google Sheets Data Sync", EditorStyles.boldLabel);
        GUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Sheet Name", GUILayout.Width(90));
            _sheetName = EditorGUILayout.TextField(_sheetName);
            if (GUILayout.Button("Pull", GUILayout.Width(80)))
                Pull(_sheetName);
        }

        GUILayout.Space(6);
        _mode = (ObfMode)EditorGUILayout.EnumPopup("Obfuscation", _mode);
        if (_mode == ObfMode.AES)
        {
            EditorGUI.indentLevel++;
            _aesPassphrase = EditorGUILayout.PasswordField("AES Passphrase", _aesPassphrase);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(6);
        EditorGUILayout.LabelField("Save Path (persistentDataPath):");
        EditorGUILayout.SelectableLabel($"{Application.persistentDataPath}/{_savePathRelDisplay}", GUILayout.Height(18));

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Open Save Folder", GUILayout.Width(150)))
                EditorUtility.RevealInFinder(Path.GetDirectoryName(_savePathAbs));
            if (GUILayout.Button("Copy Full Path", GUILayout.Width(150)))
                EditorGUIUtility.systemCopyBuffer = _savePathAbs;
        }

        GUILayout.Space(8);
        _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(140));
        GUILayout.Label(_lastLog ?? "(log will appear here)");
        GUILayout.EndScrollView();
    }
}
#endif
