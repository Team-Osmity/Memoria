using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Memoria.Constants;

namespace Memoria.Systems
{
    public static class ParameterManager
    {
        public enum ObfMode { None, Simple, AES }
        private static readonly string DefaultFilePath = Path.Combine(Application.persistentDataPath, GoogleSheets.SAVE_PATH);
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "Assets/Resources/Data/Parameters.sav");

        private static ObfMode DefaultObf = ObfMode.Simple;
        private static ObfMode SaveObf = ObfMode.Simple;

        private static string DefaultAesPass = "";
        private static string SaveAesPass = "";

        private static JObject defaultsData;
        private static JObject userData;
        private static JObject mergedData;
        private static bool initialized;
        private static TaskCompletionSource<bool> initTcs = new TaskCompletionSource<bool>();
        public static bool IsInitialized => initialized;

        /// <summary>
        /// 初期化
        /// </summary>
        public static async Task InitializeAsync()
        {
            if (initialized) return;

            await Task.Run(() =>
            {
                defaultsData = LoadAndDecode(DefaultFilePath, DefaultObf, DefaultAesPass);
                userData = LoadAndDecode(SaveFilePath, SaveObf, SaveAesPass);
                Merge();
            });

            initialized = true;
            initTcs.TrySetResult(true);
            Debug.Log($"[ParameterManager] Initialized ({DefaultFilePath})");
        }

        public static async Task WaitUntilInitializedAsync()
        {
            if (initialized) return;
            await initTcs.Task;
        }

        private static JObject LoadAndDecode(string path, ObfMode mode, string pass)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ParameterManager] File not found: {path}");
                return new JObject();
            }

            try
            {
                string raw = File.ReadAllText(path, Encoding.UTF8);
                string json = Decode(raw, mode, pass);
                var root = JObject.Parse(json);
                return (JObject)(root["data"] ?? root);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ParameterManager] Failed to read {path}: {ex.Message}");
                return new JObject();
            }
        }

        private static string Decode(string raw, ObfMode mode, string pass)
        {
            switch (mode)
            {
                case ObfMode.None:
                    return raw;
                case ObfMode.Simple:
                    return JsonObfuscator.Decode(raw);
                case ObfMode.AES:
                    return JsonObfuscator.AesDecrypt(raw, pass);
                default:
                    return raw;
            }
        }

        private static void Merge()
        {
            mergedData = (JObject)defaultsData.DeepClone();
            mergedData.Merge(userData, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace,
                MergeNullValueHandling = MergeNullValueHandling.Ignore
            });
        }

        public static T GetParam<T>(string key, T fallback = default)
        {
            if (!initialized) throw new InvalidOperationException("ParameterManager not initialized");
            var token = mergedData.SelectToken(key);
            if (token == null) return fallback;
            try { return token.ToObject<T>(); }
            catch { return fallback; }
        }

        public static void SetParam(string key, object value)
        {
            if (!initialized) throw new InvalidOperationException("ParameterManager not initialized");
            var tok = JToken.FromObject(value);
            SetToken(userData, key, tok);
            SetToken(mergedData, key, tok.DeepClone());
        }

        public static void Save()
        {
            if (!initialized) throw new InvalidOperationException("ParameterManager not initialized");
            var root = new JObject { ["data"] = userData };
            string json = root.ToString(Formatting.None);

            string encoded = SaveObf switch
            {
                ObfMode.Simple => JsonObfuscator.Encode(json),
                ObfMode.AES => JsonObfuscator.AesEncrypt(json, SaveAesPass),
                _ => json
            };

            File.WriteAllText(SaveFilePath, encoded, new UTF8Encoding(false));
            Debug.Log($"[ParameterManager] Saved -> {SaveFilePath}");
        }

        private static void SetToken(JObject root, string path, JToken value)
        {
            var parts = path.Split('.');
            JObject cur = root;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var p = parts[i];
                if (!(cur[p] is JObject next))
                {
                    next = new JObject();
                    cur[p] = next;
                }
                cur = next;
            }
            cur[parts[^1]] = value;
        }
    }
}