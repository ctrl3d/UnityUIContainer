#if USE_LITMOTION
using LitMotion;
#endif
using System;
using UnityEngine;
#if USE_UNITY_EXTENSIONS
using AnchorPreset = work.ctrl3d.RectTransformExtensions.AnchorPreset;
#endif

namespace work.ctrl3d
{
    [Serializable]
    public class UIContainerData
    {
        public string name;
        public AnchorPreset anchorPreset = AnchorPreset.UseDefault;
        public Vector2 position = Vector2.zero;
        public Vector2 sizeDelta = Vector2.zero;
        public Vector2 offset = Vector2.zero;
        public Ease showEase;
        public float showDuration;
        public Ease hideEase;
        public float hideDuration;
    }
}