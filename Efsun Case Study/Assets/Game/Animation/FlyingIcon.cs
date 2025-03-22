using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingIcon : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void ChangeSprite(Sprite sprite) => icon.sprite = sprite;
}
