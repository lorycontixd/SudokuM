using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiPanelItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    private EmojiManager.EmojiContainer emoji;

    public Action<EmojiManager.EmojiContainer> onClick;


    private void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetEmoji(EmojiManager.EmojiContainer emoji)
    {
        this.emoji = emoji;
    }

    public void UpdateUI()
    {
        this.image.sprite = this.emoji.Emoji;
    }
    
    private void OnButtonClick()
    {
        onClick?.Invoke(this.emoji);
    }
}
