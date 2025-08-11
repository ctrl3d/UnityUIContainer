using System;
using UnityEngine;
#if USE_LITMOTION
using LitMotion;
#endif
#if USE_UNITY_EXTENSIONS
using AnchorPreset = work.ctrl3d.RectTransformExtensions.AnchorPreset;
#endif

namespace work.ctrl3d
{
    [Serializable]
    public class UIContainerData
    {
        public string name;
#if USE_UNITY_EXTENSIONS
        public AnchorPreset anchorPreset = AnchorPreset.UseDefault;
#endif
        public Vector2 position = Vector2.zero;
        public Vector2 sizeDelta = Vector2.zero;
        public Vector2 offset = Vector2.zero;
#if USE_LITMOTION
        public Ease showEase;
        public float showDuration;
        public Ease hideEase;
        public float hideDuration;
#endif
    }
}