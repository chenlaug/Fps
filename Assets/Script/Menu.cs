using UnityEngine;

public enum MenuType
{
    None,
    MainMenu,
    CreateRoomMenu,
    FindRoomMenu,
    RoomMenu,
    ErrorMenu,
    LoadingScreen,
}

public class Menu : MonoBehaviour
{
    [HideInInspector] public bool isOpen = false;
    public MenuType menuType;

    public void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }
}