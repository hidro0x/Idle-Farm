using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResourceCollectAnimation : MonoBehaviour
{
    public static Action<Vector3, ResourceSO, int> OnResourceCollected;
    
    [SerializeField] private FlyingIcon flyingIconPrefab;
    [SerializeField] private int poolSize = 10;
    private Queue<FlyingIcon> _pool = new Queue<FlyingIcon>();

    private Camera _camera;
    private Canvas _canvas;
    private ResourceBarController _barController;
    
    private void Start()
    {
        _barController = GetComponent<ResourceBarController>();
        _canvas = GetComponent<Canvas>();
        _camera = Camera.main;
        
        OnResourceCollected += PlayResourceCollectAnimation;
        
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(flyingIconPrefab, transform);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    private void OnDestroy()
    {
        OnResourceCollected -= PlayResourceCollectAnimation;
    }
    
    private void PlayResourceCollectAnimation(Vector3 worldPos, ResourceSO resource, int amount)
    {
        Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPos,
            _canvas.worldCamera,
            out Vector2 localPos
        );

        var target = _barController.GetBarForResource(resource);
        if (target == null) return;

        int maxSpawnCount = 5;
        int spawnCount = Mathf.Min(amount, maxSpawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            var icon = Get();
            icon.ChangeSprite(resource.Icon);
            var rect = icon.GetComponent<RectTransform>();
            rect.SetParent(_canvas.transform, false);
            
             Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 100f;
             rect.anchoredPosition = localPos + randomOffset;
            
            icon.transform.DOMove(target.position, 0.6f).SetEase(Ease.InOutQuad).OnComplete(() => Return(icon));
            
        }
    }
    
    private FlyingIcon Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        var newObj = Instantiate(flyingIconPrefab, transform);
        return newObj;
    }

    private void Return(FlyingIcon obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
    
}
