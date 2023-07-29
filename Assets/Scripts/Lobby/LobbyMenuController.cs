using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LobbyMenuController : MonoBehaviour
{
    #region Singleton
    private static LobbyMenuController _instance;
    public static LobbyMenuController Instance { get { return _instance; } }

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


    [SerializeField] private List<BaseMenu> menus = new List<BaseMenu>();
    public BaseMenu activeMenu { get; private set; } = null;
    public UnityEvent<MenuType, MenuType> onMenuSwitch; // Params = (old menu, new menu)

    [Header("Settings")]
    [SerializeField] private bool DebugMode = false;
    [SerializeField] private bool AutoFindMenus = false;

    private void Start()
    {
        Init();
        SetInitialMenu();
        SetControllerToMenus();
    }

    private void Update()
    {
        
    }
    private void Init()
    {
        if (AutoFindMenus)
        {
            menus = transform.GetComponentsInChildren<BaseMenu>().ToList();
        }
        if (ValidateMenus())
        {

        }
        else
        {
        }
    }
    private void SetInitialMenu()
    {
        CloseAllMenus();
        activeMenu = GetMenuByType(MenuType.MAIN);
        activeMenu.gameObject.SetActive(true);
        activeMenu.Open();
    }
    private void SetControllerToMenus()
    {
        foreach(BaseMenu menu in menus)
        {
            menu.SetController(this);
        }
    }
    private bool ValidateMenus()
    {
        bool areNotNull = menus.Count > 0;
        if (!areNotNull)
        {
            Debug.LogWarning($"[LobbyMenuController->ValidateMenus] No menus found");
            return false;
        }
        bool areTypesUnique = menus.Select(m => m.Type).ToList().Distinct().Count() == menus.Count();
        if (!areTypesUnique)
        {
            Debug.LogWarning($"[LobbyMenuController->ValidateMenus] Menu types are not unique");
            return false;
        }
        return true;
    }


    public void CloseAllMenus()
    {
        foreach (BaseMenu menu in menus)
        {
            if (menu != null)
            {
                menu.Close();
                menu.gameObject.SetActive(false);
            }
        }
    }
    public BaseMenu GetMenuByType(MenuType type)
    {
        return menus.FirstOrDefault(m => m.Type == type);
    }

    public void SwitchMenu(MenuType type)
    {
        if (activeMenu != null)
        {
            if (activeMenu.CanClose())
            {
                BaseMenu newMenu = GetMenuByType(type);
                if (newMenu.CanOpen())
                {
                    MenuType oldMenuType = activeMenu.Type;
                    activeMenu.Close();
                    activeMenu.gameObject.SetActive(false);
                    newMenu.gameObject.SetActive(true);
                    newMenu.Open();
                    activeMenu = newMenu;
                    onMenuSwitch?.Invoke(oldMenuType, activeMenu.Type);
                }
            }
        }
        
    }
}
