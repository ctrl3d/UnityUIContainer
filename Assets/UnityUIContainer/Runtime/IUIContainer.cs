#if USE_UNITASK
using System.Threading;
using Cysharp.Threading.Tasks;

namespace work.ctrl3d
{
    public interface IUIContainer
    {
        void SetData(UIContainerData data);
        void SetActive(bool isActive, float canvasGroupAlpha);

        UniTask ShowAsync(CancellationToken cancellationToken = default);
        UniTask HideAsync(bool isActive, CancellationToken cancellationToken = default);
        UniTask DestroyAsync(CancellationToken cancellationToken = default);
    }
}
#endif