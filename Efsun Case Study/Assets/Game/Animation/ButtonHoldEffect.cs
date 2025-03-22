using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoldEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform _rectTransform;
    private Tweener _scaleDownTween;
    private Tweener _scaleUpTween;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _scaleDownTween = _rectTransform.DOScale(Vector3.one * 0.9f, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(false)
            .Pause();

        _scaleUpTween = _rectTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(false).Pause();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _scaleDownTween.Restart();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _scaleUpTween.Restart();
    }
}