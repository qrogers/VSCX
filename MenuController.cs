using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    private List<Menu> menus;
    private Menu currentMenu;

    void Start() {
        menus = new List<Menu>();
        currentMenu = null;
    }

    public void CreateMenu(string menuText, List<Option> menuOptions, Vector2 startCorner) {
        Menu newMenu = ScriptableObject.CreateInstance<Menu>();
        newMenu.SetupMenu(menuText, menuOptions, startCorner);
        menus.Add(newMenu);
        currentMenu = newMenu;
    }

    public void MoveMenu(bool direction) {
        currentMenu.MoveOption(direction);
    }

    public void ClickMenu() {
        if(currentMenu != null) {
            currentMenu.ClickOption();
        }
    }

    public void DeleteMenu() {
        Menu menu = menus[menus.Count - 1];
        menus.Remove(menu);
        Destroy(menu);
        if(menus.Count > 0) {
            currentMenu = menus[menus.Count - 1];
        } else {
            currentMenu = null;
        }
    }

    public void RenderMenus() {
        foreach(Menu menu in menus) {
            menu.RenderMenu();
        }
    }
}
