using DG.Tweening;
using Managers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EmojiManager : MonoBehaviour
{
    #region Emoji Container
    [Serializable]
    public struct EmojiContainer
    {
        public int Id;
        public string Name;
        public Sprite Emoji;
        public bool IsActive;
        public bool IsAdmin;
    }
    #endregion

    #region Singleton
    private static EmojiManager _instance;
    public static EmojiManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public int EmojiCooldownSeconds = 10;
    public int EmojiDurationSeconds = 4;
    public List<EmojiContainer> emojis = new List<EmojiContainer>();

    public bool IsOnCooldown { get; private set; }
    public bool IsRendering { get; private set; }
    private float cooldownTimestamp;

    [SerializeField] private Image emojiImage;

    public Action<EmojiContainer> onEmojiReceived;

    private void Start()
    {
        emojiImage.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (IsOnCooldown)
        {
            cooldownTimestamp -= Time.deltaTime;
            if (cooldownTimestamp <= 0f)
            {
                IsOnCooldown = false;
                cooldownTimestamp = 0f;
            }
        }
    }

    private void StartCooldown()
    {
        cooldownTimestamp = EmojiCooldownSeconds;
        IsOnCooldown = true;
    }
    public void SendEmoji(EmojiContainer emoji)
    {
        if (!IsOnCooldown && !IsRendering)
        {
            GamePunEventSender.SendEmoji(PhotonNetwork.LocalPlayer.ActorNumber, emoji.Id);
            RenderEmoji(emoji);
            StartCooldown();
        }
    }
    public void ReceiveEmoji(int emojiID)
    {
        EmojiContainer emoji = emojis.FirstOrDefault(e => e.Id == emojiID);
        AudioManager.Instance.PlayNotification("Emoji");
        RenderEmoji(emoji);
    }

    public void RenderEmoji(EmojiContainer emoji)
    {
        StartCoroutine(RenderEmojiCo(emoji));
    }
    private IEnumerator RenderEmojiCo(EmojiContainer container)
    {
        IsRendering = true;
        this.emojiImage.sprite = container.Emoji;
        this.emojiImage.gameObject.SetActive(true);
        this.emojiImage.transform.localScale = Vector3.zero;
        this.emojiImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).onComplete += () => { this.emojiImage.transform.DOShakePosition(2f, 10, 45, 90); };
        yield return new WaitForSeconds(EmojiDurationSeconds);
        this.emojiImage.transform.DOScale(new Vector3(0f,0f,0f), 0.3f).onComplete += () => { this.emojiImage.gameObject.SetActive(false); };
        IsRendering = false;
    }
}
