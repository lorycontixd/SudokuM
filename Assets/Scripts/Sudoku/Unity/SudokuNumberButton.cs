using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SudokuNumberButton : MonoBehaviour
{
    public int ID;
    public int Value;
    private Button button;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image selectionMask;

    public Action<SudokuNumberButton> onButtonClick;


    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        if (selectionMask == null)
        {
            selectionMask = transform.Find("Mask").GetComponent<Image>();
        }
        Reset();
    }

    public void SetID(int id)
    {
        this.ID = id;
    }

    private void OnClick()
    {
        onButtonClick?.Invoke(this);
    }

    public void Deactivate()
    {
        valueText.gameObject.SetActive(false);
        selectionMask.gameObject.SetActive(false);
    }
    public void Activate()
    {
        valueText.gameObject.SetActive(true);
        selectionMask.gameObject.SetActive(true);
    }

    public void Reset()
    {
        valueText.gameObject.SetActive(true);
        selectionMask.gameObject.SetActive(false);
    }
}
