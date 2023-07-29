using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
using UnityEngine.UI;
using TMPro;

namespace Managers
{
    public class NotificationManager : MonoBehaviour
    {
        #region Singleton
        private static NotificationManager _instance;
        public static NotificationManager Instance { get { return _instance; } }

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

        #region Level
        public enum Level
        {
            INFO,
            WARNING,
            ERROR
        }
        #endregion

        #region Notification
        public struct Notification
        {
            public string title;
            public string description;
            public Level level;

            public Notification(string title, string description, Level level)
            {
                this.title = title;
                this.description = description;
                this.level = level;
            }
        }
        #endregion

        [Header("UI")]
        [SerializeField] private Michsky.MUIP.NotificationManager notificationManager;
        [SerializeField] private Image notificationBackground;
        [SerializeField] private TextMeshProUGUI notificationTitle;
        [SerializeField] private TextMeshProUGUI notificationDescription;
        [SerializeField] private Image notificationIcon;

        [Header("Settings")]
        [SerializeField] private bool debugMode;
        [SerializeField] private float messageDuration;
        [SerializeField] private Color infoColor = new Color(45f, 65f, 85f);
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private Sprite infoIcon = null;
        [SerializeField] private Sprite warningIcon = null;
        [SerializeField] private Sprite errorIcon = null;

        private Queue<Notification> messages = new Queue<Notification>();
        private bool isRunning = false;


        private void Start()
        {
            notificationManager.gameObject.SetActive(true);
            notificationManager.enabled = true;
            notificationManager.enableTimer = false;
        }

        public void Info(string title, string message)
        {
            AddNotification(title, message, Level.INFO);
        }
        public void Warning(string title, string message)
        {
            AddNotification(title, message, Level.WARNING);
        }
        public void Error(string title, string message)
        {
            AddNotification(title, message, Level.ERROR);
        }

        private void AddNotification(string title, string message, Level level)
        {
            Notification notification = new Notification(title, message, level);
            messages.Enqueue(notification);
            if (!isRunning)
            {
                StartCoroutine(PrintNextMessage());
            }
        }

        private void ShowMessage(Notification notification)
        {
            notificationManager.gameObject.SetActive(true);
            notificationManager.title = notification.title;
            notificationManager.description = notification.description;
            Debug.Log($"notification level: {notification.level},  {notificationManager.title}");
            if (notification.level == Level.INFO)
            {
                notificationBackground.color = infoColor;
                notificationTitle.color = Color.white;
                notificationDescription.color = Color.white;
                if (infoIcon != null && notificationIcon != null)
                {
                    notificationIcon.sprite = infoIcon;
                }
            }else if (notification.level == Level.WARNING)
            {
                notificationBackground.color = warningColor;
                notificationTitle.color = Color.black;
                notificationDescription.color = Color.black;
                if (warningIcon != null && notificationIcon != null)
                {
                    notificationIcon.sprite = warningIcon;
                }
            }else if (notification.level == Level.ERROR)
            {
                notificationBackground.color = errorColor;
                notificationTitle.color = Color.black;
                notificationDescription.color = Color.black;
                if (errorIcon != null && notificationIcon != null)
                {
                    notificationIcon.sprite = errorIcon;
                }
            }
            else
            {
                // Nono
            }
            notificationManager.UpdateUI();
            notificationManager.Open();
        }

        private IEnumerator PrintNextMessage()
        {
            Debug.Log($"Printing next message!");
            isRunning = true;
            Notification msg = messages.Dequeue();
            ShowMessage(msg);
            yield return new WaitForSeconds(messageDuration);
            notificationManager.Close();
            if (messages.Count > 0 )
            {
                StartCoroutine(PrintNextMessage());
            }
            else
            {
                isRunning = false;
            }
        }
    }

}
