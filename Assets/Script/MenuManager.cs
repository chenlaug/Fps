using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager MenuManagerInstance;

    public static MenuManager Instance
    {
        get
        {
            if (MenuManagerInstance == null)
            {
                MenuManagerInstance = FindObjectOfType(typeof(MenuManager)) as MenuManager;
            }

            return MenuManagerInstance;
        }
    }

    [SerializeField] private List<Menu> menus = new List<Menu>();

    private void Awake()
    {
        OpenMenu(MenuType.MainMenu);
    }

    public void OpenMenu(MenuType menuType)
    {
        foreach (var menu in menus)
        {
            if (menu.menuType == menuType)
            {
                menu.Open();
            }
            else
            {
                menu.Close();
            }
        }
    }

    public void CloseMenu(MenuType menuType)
    {
        foreach (var menu in menus)
        {
            if (menu.menuType == menuType)
            {
                menu.Close();
            }
        }
    }

    public void OpenMenuChosen(Menu menuChosen)
    {
        foreach (var menu in menus)
        {
            if (menu.isOpen)
            {
                CloseMenu(menu.menuType);
            }
        }

        menuChosen.Open();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}