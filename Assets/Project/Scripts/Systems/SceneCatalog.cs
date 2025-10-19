using System;
using System.Collection.Generic;
using System.Linq;
using UnityEngine;

namespace Memoria.Systems
{
    public class SceneCatalog : ScriptableObject
    {
        [SerializeField] public struct ContentEntry
        {
            public ContentScene key;
            public string sceneName;
        }
        [SerializeField] public struct OverlayEntry
        {
            public OverlayScene key;
            public string sceneName;
        }

        public List<ContentEntry> contents = new List<ContentEntry>();
        public List<OverlayEntry> overlays = new List<OverlayEntry>();

        Dictionary<ContentScene,string> _cMap;
        Dictionary<OverlayScene,string> _oMap;

        public string Resolve(ContentScene k)
        {
            _cMap ??= contents.Where(e=>!string.IsNullOrEmpty(e.sceneName))
                              .ToDictionary(e=>e.key, e=>e.sceneName);
            return _cMap.TryGetValue(k, out var v) ? v : null;
        }
        public string Resolve(OverlayScene k)
        {
            _oMap ??= overlays.Where(e=>!string.IsNullOrEmpty(e.sceneName))
                              .ToDictionary(e=>e.key, e=>e.sceneName);
            return _oMap.TryGetValue(k, out var v) ? v : null;
        }
    }
}