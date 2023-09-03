using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSelector : MonoBehaviour
{
    [SerializeField] private Sprite fixedBackground;
    [SerializeField] private List<Sprite> backgrounds = new List<Sprite>();
    [SerializeField] private Image backgroundImage;

    private int currentBackgroundIndex;

    [Header("Settings")]
    [SerializeField] private bool ChangeBackgroundOnMenuSwitch = false;
    [SerializeField] private bool ChangeBackgroundOnStart = true;


    private void Start()
    {
        if (LobbyMenuController.Instance != null)
        {
            LobbyMenuController.Instance.onMenuSwitch.AddListener(OnMenuSwitch);
        }
        if (ChangeBackgroundOnStart)
        {
            SetBackground(PickBackground());
        }
        else
        {
            if (fixedBackground != null)
            {
                SetBackground(fixedBackground);
            }
            else
            {
                SetBackground(PickBackground());
            }
        }
    }

    private void OnMenuSwitch(MenuType oldMenuType, MenuType newMenuType)
    {
        if (ChangeBackgroundOnMenuSwitch)
        {
            SetBackground(PickBackground());
        }
    }

    private Sprite PickBackground()
    {
        int index = Random.Range(0, backgrounds.Count);
        return backgrounds[index];
    }
    public void SetBackground(Sprite sprite)
    {
        if (backgroundImage != null)
        {
            backgroundImage.sprite = sprite;
        }
    }
}
