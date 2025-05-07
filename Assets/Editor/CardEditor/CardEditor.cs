using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using System.IO;
using System.Linq;

public class CardEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_TabbedMenuTree;
    [SerializeField] private VisualTreeAsset m_ContentTabTree;
    [SerializeField] private VisualTreeAsset m_LevelUpTree;
    [SerializeField] private VisualTreeAsset m_EntryTemplate;
    [SerializeField] private Sprite m_DefaultIcon;

    private static List<LevelUps> m_LevelUps = new List<LevelUps>();
    private LevelUps m_ActiveLevelUp;
    private ListView m_LevelsListView;
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
        CreateGUIForLevels();
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
        if (!Directory.Exists(m_LevelUpsPath))
        {
            Directory.CreateDirectory(m_LevelUpsPath);
            AssetDatabase.Refresh();
        }
    }

    #region GenericMethods

    private void CreateGUIs<T>(out List<T> container, VisualElement root, out ListView lv, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem, Action<IEnumerable<object>> selectionChanged, out VisualElement rightPane, Action add, Action remove) where T : UnityEngine.Object
    {
        var allObjectsGuids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        container = new List<T>();
        foreach (var guid in allObjectsGuids)
        {
            container.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        root.Add(splitView);

        splitView.Add(m_ContentTabTree.CloneTree());

        var listView = new ListView(container, 60);
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.selectionType = SelectionType.Single;
        listView.selectionChanged += selectionChanged;
        listView.style.height = container.Count * 65;
        root.Q<VisualElement>("ItemsTab").Add(listView);
        lv = listView;

        rightPane = new VisualElement();
        splitView.Add(rightPane);

        splitView.Q<Button>("AddBtn").clicked += add;
        splitView.Q<Button>("RemoveBtn").clicked += remove;
    }

    private void Remove<T>(T itemToRemove, List<T> itemsSource, VisualElement ve, ListView listView, ref T active) where T : UnityEngine.Object
    {
        string path = AssetDatabase.GetAssetPath(itemToRemove);
        AssetDatabase.DeleteAsset(path);
        itemsSource.Remove(itemToRemove);
        if (itemsSource.Count > 0)
        {
            active = itemsSource[itemsSource.Count - 1];
            SerializedObject so = new SerializedObject(active);
            ve.Bind(so);
        }
        listView.style.height = itemsSource.Count * 65;
        listView.Rebuild();
        listView.SetSelection(itemsSource.Count - 1);
    }

    private void Add<T>(string itemName, List<T> itemsSource, string createPath, ListView listView, ref T active) where T : ScriptableObject
    {
        if (itemsSource.Exists(s => s.name == itemName))
        {
            Debug.LogError("There is another asset with the default name. Please rename it and continue!");
            return;
        }

        T newOne = CreateInstance<T>();
        if (newOne is LevelUps clu)
        {
            clu.Name = itemName;
        }

        AssetDatabase.CreateAsset(newOne, createPath + $"/{itemName}.asset");
        itemsSource.Add(newOne);
        listView.style.height = itemsSource.Count * 65;
        active = itemsSource[itemsSource.Count - 1];
        listView.Rebuild();
        listView.SetSelection(itemsSource.Count - 1);
    }

    #endregion

    #region LevelUps

    private void CreateGUIForLevels()
    {
        var rightPane = new VisualElement();
        var t = m_LevelUpTree.CloneTree();
        m_LevelsListView = t.Q<ListView>();
        //m_LevelsListView.makeItem = m_CardsLevelUpTree.CloneTree;
        var root = rootVisualElement.Q<VisualElement>("LevelUpsContent");
        var listView = new ListView();

        CreateGUIs(out m_LevelUps, root, out listView, m_EntryTemplate.CloneTree,
        (item, index) =>
        {
            item.Q<VisualElement>("Icon").style.backgroundImage = m_DefaultIcon.texture;
            item.Q<Label>("Name").text = m_LevelUps[index].name;
        },
        (item) =>
        {
            m_ActiveLevelUp = (LevelUps)item.FirstOrDefault();
            // root.Q<Button>("RemoveBtn").SetEnabled(!(m_ActiveLevelUp == null));
            if (m_ActiveLevelUp == null)
            {
                rightPane.style.visibility = Visibility.Hidden;
                return;
            }
            rightPane.style.visibility = Visibility.Visible;
            SerializedObject so = new SerializedObject(m_ActiveLevelUp);
            t.Bind(so);
            m_LevelsListView.Rebuild();
        }, out rightPane,
        () => Add("NewType", m_LevelUps, m_LevelUpsPath, listView, ref m_ActiveLevelUp),
        () => Remove(m_ActiveLevelUp, m_LevelUps, rightPane, listView, ref m_ActiveLevelUp));

        rightPane.Add(t);
        root.Query<Button>().ForEach(t => t.SetEnabled(false));
        root.Q<ListView>().SetSelection(0);
    }

    #endregion
}
