using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHalfBoard : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float moveDuration = 2f;
    public float translation = 200f;

    private float defaultY;


    private void Start()
    {
        defaultY = transform.localPosition.y;
    }

    public void Show()
    {
        transform.DOBlendableLocalMoveBy(new Vector3(0f, -translation, 0f), moveDuration);
    }
    public void Hide()
    {
        transform.DOBlendableLocalMoveBy(new Vector3(0f, translation, 0f), moveDuration);
    }
}
