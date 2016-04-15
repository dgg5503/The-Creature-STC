using UnityEditor;
using UnityEngine;

// TAKEN FROM http://answers.unity3d.com/questions/775869/editor-how-to-add-checkmarks-to-menuitems.html#answer-1146949

[InitializeOnLoad]
public static class CheckmarkMenuItem
{
    private const string MENU_NAME = "Threaded Editor/Auto Update";

    private static bool enabled_;
    /// Called on load thanks to the InitializeOnLoad attribute
    static CheckmarkMenuItem()
    {
        enabled_ = EditorPrefs.GetBool(MENU_NAME, false);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            PerformAction(enabled_);
        };
    }

    [MenuItem(MENU_NAME)]
    private static void ToggleAction()
    {

        /// Toggling action
        PerformAction(!enabled_);
    }

    public static void PerformAction(bool enabled)
    {

        /// Set checkmark on menu item
        Menu.SetChecked(MENU_NAME, enabled);
        /// Saving editor state
        EditorPrefs.SetBool(MENU_NAME, enabled);

        enabled_ = enabled;

        /// Perform your logic here...
        ContinuationManager.AutoUpdate = enabled;
    }
}