using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CardEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_TabbedMenuTree;
    private TabbedMenuController m_Controller;

    [MenuItem("Window/CardEditor")]
    public static void ShowCardEditr()
    {
        CardEditor wnd = GetWindow<CardEditor>();
        wnd.titleContent = new GUIContent("CardEditor");
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    public void CreateGUI()
    {
        ShowTabbedMenu();
    }

    private void ShowTabbedMenu()
    {
        rootVisualElement.Add(m_TabbedMenuTree.CloneTree());
        m_Controller = new(rootVisualElement.Q<VisualElement>("TabbedMenu"));
    }
}
