using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using System.IO;

public class CardEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_TabbedMenuTree;

    private TabbedMenuController m_Controller;

    protected string m_LevelUpsPath = "Assets/ScriptableObjects/Cards/LevelUps";

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
        CreateTypes();
        ShowTabbedMenu();
    }

    private void ShowTabbedMenu()
    {
        rootVisualElement.Add(m_TabbedMenuTree.CloneTree());
        m_Controller = new(rootVisualElement.Q<VisualElement>("TabbedMenu"));
    }

    private void CreateTypes()
    {
        CreateLevelUpsDirectory();
        string[] typeNames = Enum.GetNames(typeof(CardTypeEnum));

        foreach (var name in typeNames)
        {
            var asset = AssetDatabase.LoadAssetAtPath<LevelUps>(m_LevelUpsPath + $"/{name}.asset");
            
            if (asset == null)
            {
                var newOne = CreateInstance<LevelUps>();
                newOne.name = name;
                newOne.Init();
                AssetDatabase.CreateAsset(newOne, m_LevelUpsPath + $"/{name}.asset");
            }
        }
    }

    private void CreateLevelUpsDirectory()
    {
        if(!Directory.Exists(m_LevelUpsPath))
        {
            Directory.CreateDirectory(m_LevelUpsPath);
            AssetDatabase.Refresh();
        }
    }
}
