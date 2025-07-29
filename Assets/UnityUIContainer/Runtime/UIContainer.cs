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
        
        [HideInInspector, SerializeField] private RectTransform rectTransform;
        [HideInInspector, SerializeField] private CanvasGroup canvasGroup;

        public event Action OnShowStarted;
        public event Action OnShowCompleted;
        public event Action OnHideStarted;
        public event Action OnHideCompleted;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            rectTransform.SetAnchorPreset(anchorPreset);
        }

        public virtual void SetData(UIContainerData data)
        {
            if (data.name != null) gameObject.name = data.name;
            if (data.anchorPreset != AnchorPreset.UseDefault) rectTransform.SetAnchorPreset(data.anchorPreset);
            if (data.position != Vector2.zero) rectTransform.anchoredPosition = data.position;
            if (data.sizeDelta != Vector2.zero) rectTransform.sizeDelta = data.sizeDelta;
            if (data.offset != Vector2.zero) rectTransform.anchoredPosition = data.offset;
            if (data.showEase != Ease.Linear) showEase = data.showEase;
            if (data.showDuration != 0f) showDuration = data.showDuration;
            if (data.hideEase != Ease.Linear) hideEase = data.hideEase;
            if (data.hideDuration != 0f) hideDuration = data.hideDuration;
        }

        public virtual void SetActive(bool isActive, float canvasGroupAlpha)
        {
            canvasGroup.alpha = canvasGroupAlpha;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
            gameObject.SetActive(isActive);
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            OnShowStarted?.Invoke();
            onShowStarted?.Invoke();

            SetActive(true, 0f);

            await LMotion.Create(0f, 1f, showDuration).WithEase(showEase).BindToAlpha(canvasGroup)
                .ToUniTask(CancellationTokenSource
                    .CreateLinkedTokenSource(destroyCancellationToken, cancellationToken)
                    .Token);

            OnShowCompleted?.Invoke();
            onShowCompleted?.Invoke();
        }

        public virtual async UniTask HideAsync(bool isActive = false, CancellationToken cancellationToken = default)
        {
            if (!canvasGroup) return;
            if (canvasGroup.alpha == 0 && isActive) return;

            OnHideStarted?.Invoke();
            onHideStarted?.Invoke();

            await LMotion.Create(1f, 0f, hideDuration).WithEase(hideEase).BindToAlpha(canvasGroup)
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
            rectTransform.SetAnchorPreset(anchorPreset);
        }

        [Button, HorizontalGroup]
        private void Show() => ShowAsync().Forget();

        [Button, HorizontalGroup]
        private void Hide() => HideAsync().Forget();
    }
}