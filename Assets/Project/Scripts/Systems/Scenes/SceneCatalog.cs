using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Memoria.Constants;

namespace Memoria.Systems
{
    [CreateAssetMenu(menuName = "Memoria/SceneCatalog")]
    public class SceneCatalog : ScriptableObject
    {
        [System.Serializable] public struct ContentEntry
        {
            public SceneStates.ContentScene key;
            public string sceneName;
        }
        [System.Serializable] public struct OverlayEntry
        {
            public SceneStates.OverlayScene key;
            public string sceneName;
        }

        [Header("Contents")]
        public List<ContentEntry> contents = new List<ContentEntry>();
        [Header("Overlays")]
        public List<OverlayEntry> overlays = new List<OverlayEntry>();

        Dictionary<SceneStates.ContentScene,string> _cMap;
        Dictionary<SceneStates.OverlayScene,string> _oMap;

        public string Resolve(SceneStates.ContentScene k)
        {
            _cMap ??= contents.Where(e=>!string.IsNullOrEmpty(e.sceneName))
                              .ToDictionary(e=>e.key, e=>e.sceneName);
            return _cMap.TryGetValue(k, out var v) ? v : null;
        }
        public string Resolve(SceneStates.OverlayScene k)
        {
            _oMap ??= overlays.Where(e=>!string.IsNullOrEmpty(e.sceneName))
                              .ToDictionary(e=>e.key, e=>e.sceneName);
            return _oMap.TryGetValue(k, out var v) ? v : null;
        }
    }
}