using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image userIcon;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void UpdateUI()
    {
        User user = SessionManager.Instance.ActiveUser;
        if (SessionManager.Instance.ActiveUser != null)
        {
            text.text = "1";
            usernameText.text = user.Username;
            levelText.text = $"LV 1";
            scoreText.text = $"{user.Scores.ScoreTimerace}";
        }
    }
}
