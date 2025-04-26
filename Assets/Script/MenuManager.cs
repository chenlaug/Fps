using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager MenuMangerInstance;

    public static MenuManager Instance
    {
        get
        {
            if (MenuMangerInstance == null)
            {
                MenuMangerInstance = FindObjectOfType(typeof(MenuManager)) as MenuManager;
            }

            return MenuMangerInstance;
        }
    }

    [SerializeField] private List<Menu> menus = new List<Menu>();

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
            else
            {
                menu.Open();
            }
        }
    }

    public void OpenMenu(Menu m)
    {
        foreach (var menu in menus)
        {
            if (menu.isOpen)
            {
                CloseMenu(menu.menuType);
            }
            menu.Open();
        }
    }
}