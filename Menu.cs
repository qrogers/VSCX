using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : ScriptableObject {

    private List<MenuOption> options;
    private int currentOption;
    private string menuText;
    private Vector2 startCorner;
    
	void Awake() {
        options = new List<MenuOption>();
    }

    public void SetupMenu(string menutext, List<Option> menuOptions, Vector2 startCorner) {
        this.menuText = menutext;
        this.startCorner = startCorner;
        foreach(Option option in menuOptions) {
            MenuOption newOption = new MenuOption();
            newOption.SetOption(option);
            options.Add(newOption);
        }
        currentOption = 0;
    }

    public void MoveOption(bool direction) {
        if(direction) {
            currentOption++;
        } else {
            currentOption--;
        }
        if(currentOption < 0) {
            currentOption = options.Count - 1;
        } else if(currentOption > options.Count - 1) {
            currentOption = 0;
        }
    }

    public void ClickOption() {
         options[currentOption].DoAction();
    }

    public void RenderMenu() {
        Rect menuBorder = new Rect(startCorner, new Vector2(Mathf.Max(10 * menuText.Length, 100), 20 * options.Count + 25));
        GUI.Box(menuBorder, menuText);
        int i = 0;
        foreach(MenuOption option in options) {
            i++;
            GUI.Label(new Rect(menuBorder.x + menuBorder.width / 4, menuBorder.y + 25 * i, menuBorder.width, menuBorder.height), option.option.name);
            if(option == options[currentOption]) {
                GUI.Label(new Rect(menuBorder.x + 10, menuBorder.y + 25 * i, menuBorder.width, menuBorder.height), "→");
            }
        }
        DrawRect(menuBorder, new Color(0.0f, 0.0f, 0.8f));
    }

    private void DrawRect(Rect rect, Color color) {
        Drawing.DrawLine(new Rect(rect.x, rect.y, 0, rect.height + 1), color);
        Drawing.DrawLine(new Rect(rect.x, rect.y, rect.width, 0), color);
        Drawing.DrawLine(new Rect(rect.x + rect.width, rect.y, 0, rect.height), color);
        Drawing.DrawLine(new Rect(rect.x, rect.y + rect.height, rect.width, 0), color);

        Drawing.DrawLine(new Rect(rect.x + 1, rect.y + 1, 0, rect.height - 1), new Color(8.0f, 0.4f, 0.4f));
        Drawing.DrawLine(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, 0), new Color(8.0f, 0.4f, 0.4f));

        Drawing.DrawLine(new Rect(rect.x + rect.width - 1, rect.y + 1, 0, rect.height - 1), new Color(8.0f, 0.4f, 0.4f));

        Drawing.DrawLine(new Rect(rect.x + 1, rect.y + rect.height - 1, rect.width - 2, 0), new Color(8.0f, 0.4f, 0.4f));

    }

}

public struct Option {
    public delegate void OptionAction(string actionName);
    public OptionAction optionAction;
    public string name;
}

public class MenuOption {

    public Option option;

    public void SetOption(Option option) {
        this.option = option;
    }

    public void DoAction() {
        option.optionAction(option.name);
    }

    public string GetName() {
        return option.name;
    }

}

public class Drawing {
    //****************************************************************************************************
    //  static function DrawLine(rect : Rect) : void
    //  static function DrawLine(rect : Rect, color : Color) : void
    //  static function DrawLine(rect : Rect, width : float) : void
    //  static function DrawLine(rect : Rect, color : Color, width : float) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, width : float) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color, width : float) : void
    //  
    //  Draws a GUI line on the screen.
    //  
    //  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
    //  This function works by drawing a 1x1 texture filled with a color, which is then scaled
    //   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
    //****************************************************************************************************

    public static Texture2D lineTex;

    public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
    public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
    public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
    public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width) {
        // Save the current GUI matrix, since we're going to make changes to it.
        Matrix4x4 matrix = GUI.matrix;

        // Generate a single pixel texture if it doesn't exist
        if(!lineTex) { lineTex = new Texture2D(1, 1); }

        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        Color savedColor = GUI.color;
        GUI.color = color;

        // Determine the angle of the line.
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);

        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if(pointA.y > pointB.y) { angle = -angle; }

        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));

        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, pointA);

        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);

        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }

}
