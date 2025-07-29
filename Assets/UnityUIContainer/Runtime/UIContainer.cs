#if ALCHEMY_SUPPORT
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

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        public event Action OnShowStarted;
        public event Action OnShowCompleted;
        public event Action OnHideStarted;
        public event Action OnHideCompleted;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _rectTransform.SetAnchorPreset(anchorPreset);
        }

        public virtual void SetData(UIContainerData data)
        {
            if (data.name != null) gameObject.name = data.name;
            if (data.anchorPreset != AnchorPreset.UseDefault) _rectTransform.SetAnchorPreset(data.anchorPreset);
            if (data.position != Vector2.zero) _rectTransform.anchoredPosition = data.position;
            if (data.sizeDelta != Vector2.zero) _rectTransform.sizeDelta = data.sizeDelta;
            if (data.offset != Vector2.zero) _rectTransform.anchoredPosition = data.offset;
            if (data.showEase != Ease.Linear) showEase = data.showEase;
            if (data.showDuration != 0f) showDuration = data.showDuration;
            if (data.hideEase != Ease.Linear) hideEase = data.hideEase;
            if (data.hideDuration != 0f) hideDuration = data.hideDuration;
        }

        public virtual void SetActive(bool isActive, float canvasGroupAlpha)
        {
            _canvasGroup.alpha = canvasGroupAlpha;
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.interactable = isActive;
            gameObject.SetActive(isActive);
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            OnShowStarted?.Invoke();
            onShowStarted?.Invoke();

            SetActive(true, 0f);

            await LMotion.Create(0f, 1f, showDuration).WithEase(showEase).BindToAlpha(_canvasGroup)
                .ToUniTask(CancellationTokenSource
                    .CreateLinkedTokenSource(destroyCancellationToken, cancellationToken)
                    .Token);

            OnShowCompleted?.Invoke();
            onShowCompleted?.Invoke();
        }

        public virtual async UniTask HideAsync(bool isActive = false, CancellationToken cancellationToken = default)
        {
            if (!_canvasGroup) return;
            if (_canvasGroup.alpha == 0 && isActive) return;

            OnHideStarted?.Invoke();
            onHideStarted?.Invoke();

            await LMotion.Create(1f, 0f, hideDuration).WithEase(hideEase).BindToAlpha(_canvasGroup)
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
            if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
            if(!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform.SetAnchorPreset(anchorPreset);
        }

        [Button, HorizontalGroup]
        private void Show() => ShowAsync().Forget();

        [Button, HorizontalGroup]
        private void Hide() => HideAsync().Forget();
    }
}
#endif