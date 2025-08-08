using System;
using UnityEngine;
#if USE_LIT_MOTION
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
#if USE_LIT_MOTION
        public Ease showEase;
#endif
        public float showDuration;
#if USE_LIT_MOTION
        public Ease hideEase;
#endif
        public float hideDuration;
    }
}