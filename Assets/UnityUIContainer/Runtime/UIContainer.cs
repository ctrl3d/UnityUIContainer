//#if ALCHEMY_SUPPORT
using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Events;
using AnchorPreset = work.ctrl3d.RectTransformExtensions.AnchorPreset;

namespace work.ctrl3d
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContainer : MonoBehaviour, IUIContainer
    {
        [Title("Settings")]
        [SerializeField] private AnchorPreset anchorPreset = AnchorPreset.UseDefault;
        [SerializeField] private Ease showEase = Ease.Linear;
        [SerializeField] private float showDuration = 0.2f;
        [SerializeField] private Ease hideEase = Ease.Linear;
        [SerializeField] private float hideDuration = 0.2f;

        [Title("Events")] 
        public UnityEvent onShowStarted;
        public UnityEvent onShowCompleted;
        public UnityEvent onHideStarted;
        public UnityEvent onHideCompleted;

        public RectTransform RectTransform { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }

        public event Action OnShowStarted;
        public event Action OnShowCompleted;
        public event Action OnHideStarted;
        public event Action OnHideCompleted;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            RectTransform.SetAnchorPreset(anchorPreset);
        }

        public virtual void SetData(UIContainerData data)
        {
            if (data.name != null) gameObject.name = data.name;
            if (data.anchorPreset != AnchorPreset.UseDefault) RectTransform.SetAnchorPreset(data.anchorPreset);
            if (data.position != Vector2.zero) RectTransform.anchoredPosition = data.position;
            if (data.sizeDelta != Vector2.zero) RectTransform.sizeDelta = data.sizeDelta;
            if (data.offset != Vector2.zero) RectTransform.anchoredPosition = data.offset;
            if (data.showEase != Ease.Linear) showEase = data.showEase;
            if (data.showDuration != 0f) showDuration = data.showDuration;
            if (data.hideEase != Ease.Linear) hideEase = data.hideEase;
            if (data.hideDuration != 0f) hideDuration = data.hideDuration;
        }

        public virtual void SetActive(bool isActive, float canvasGroupAlpha)
        {
            CanvasGroup.alpha = canvasGroupAlpha;
            CanvasGroup.blocksRaycasts = isActive;
            CanvasGroup.interactable = isActive;
            gameObject.SetActive(isActive);
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            OnShowStarted?.Invoke();
            onShowStarted?.Invoke();

            SetActive(true, 0f);

            await LMotion.Create(0f, 1f, showDuration).WithEase(showEase).BindToAlpha(CanvasGroup)
                .ToUniTask(CancellationTokenSource
                    .CreateLinkedTokenSource(destroyCancellationToken, cancellationToken)
                    .Token);

            OnShowCompleted?.Invoke();
            onShowCompleted?.Invoke();
        }

        public void SetSizeDelta(float width, float height) => RectTransform.SetSizeDelta(width, height);
        public void SetSizeDelta(Vector2 sizeDelta) => RectTransform.SetSizeDelta(sizeDelta);
        public void SetAnchoredPosition(float x, float y) => RectTransform.SetAnchoredPosition(x, y);
        public void SetAnchoredPosition(Vector2 position) => RectTransform.SetAnchoredPosition(position);
        public void SetAnchoredPosition3D(float x, float y, float z) => RectTransform.SetAnchoredPosition3D(x, y, z);
        public void SetAnchoredPosition3D(Vector3 position) => RectTransform.SetAnchoredPosition3D(position);
        
        public virtual async UniTask HideAsync(bool isActive = false, CancellationToken cancellationToken = default)
        {
            if (!CanvasGroup) return;
            if (CanvasGroup.alpha == 0 && isActive) return;

            OnHideStarted?.Invoke();
            onHideStarted?.Invoke();

            await LMotion.Create(1f, 0f, hideDuration).WithEase(hideEase).BindToAlpha(CanvasGroup)
                .ToUniTask(CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellationToken)
                    .Token);
            SetActive(isActive, 0f);

            OnHideCompleted?.Invoke();
            onHideCompleted?.Invoke();
        }

        public virtual async UniTask DestroyAsync(CancellationToken cancellationToken = default)
        {
            await HideAsync(true, cancellationToken);
            Destroy(gameObject);
            await UniTask.Yield();
        }

        private void OnValidate()
        {
            if (!RectTransform) RectTransform = GetComponent<RectTransform>();
            if(!CanvasGroup) CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform.SetAnchorPreset(anchorPreset);
        }

        [Button, HorizontalGroup]
        private void Show() => ShowAsync().Forget();

        [Button, HorizontalGroup]
        private void Hide() => HideAsync().Forget();
    }
}
//#endif