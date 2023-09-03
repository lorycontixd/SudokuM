using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EmojiPanel : MonoBehaviour
{
    [SerializeField] private GameObject emojiPrefab;
    [SerializeField] private Transform emojiHolder;

    private List<GameObject> clones = new List<GameObject>();



    public void Open()
    {
        Debug.Log($"Opening emoji panel => Spawning");
        gameObject.SetActive(true);
        SpawnItems();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void ClearItems()
    {
        for (int i=0; i<emojiHolder.childCount; i++)
        {
            Destroy(emojiHolder.GetChild(i).gameObject);
        }
    }
    private void SpawnItems()
    {
        ClearItems();
        Debug.Log($"Emojicount: {EmojiManager.Instance.emojis.Count}");
        for (int i=0; i<EmojiManager.Instance.emojis.Count; i++)
        {
            EmojiManager.EmojiContainer emoji = EmojiManager.Instance.emojis[i];
            if (emoji.IsActive)
            {
                if (!emoji.IsAdmin || (emoji.IsAdmin && SessionManager.Instance.ActiveUser.RoleID == 0))
                {
                    GameObject clone = Instantiate(emojiPrefab, emojiHolder);
                    clones.Add(clone);
                    EmojiPanelItem item = clone.GetComponent<EmojiPanelItem>();
                    item.onClick += OnEmojiClick;
                    item.SetEmoji(emoji);
                    item.UpdateUI();
                }
            }
        }
        Debug.Log($"Spawnitems => Spawned {clones.Count} emoji items");
    }

    private void OnEmojiClick(EmojiManager.EmojiContainer container)
    {
        Close();
        EmojiManager.Instance.SendEmoji(container);
    }
}
