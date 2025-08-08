using System.Threading;
#if USE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace work.ctrl3d
{
    public interface IUIContainer
    {
        void SetData(UIContainerData data);
        void SetActive(bool isActive, float canvasGroupAlpha);
#if USE_UNITASK
        UniTask ShowAsync(CancellationToken cancellationToken = default);
        UniTask HideAsync(bool isActive, CancellationToken cancellationToken = default);
        UniTask DestroyAsync(CancellationToken cancellationToken = default);
#endif
    }
}