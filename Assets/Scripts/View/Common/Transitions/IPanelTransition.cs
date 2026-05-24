using Cysharp.Threading.Tasks;

namespace View.Common.Transitions
{
    public interface IPanelTransition
    {
        UniTask FadeIn(float duration);
        UniTask FadeOut(float duration);
    }
}