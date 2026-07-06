using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSO))]
public class AudioSOEditor : Editor
{
    private SerializedProperty audioData;
    private readonly Dictionary<string, bool> foldouts = new();

    void OnEnable()
    {
        audioData = serializedObject.FindProperty("audioData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawSyncButton();
        DrawGroupedAudio();
        serializedObject.ApplyModifiedProperties();
    }

    void DrawSyncButton()
    {
        EditorGUILayout.BeginHorizontal();

        // Отображаем текущее количество элементов
        EditorGUILayout.LabelField($"Audio entries: {audioData.arraySize}", EditorStyles.miniLabel);

        GUILayout.FlexibleSpace();

        // Кнопка синхронизации
        if (GUILayout.Button("Sync with AudioType Enum", GUILayout.Width(180)))
        {
            SyncAudioDataWithEnum();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
    }

    void SyncAudioDataWithEnum()
    {
        // Получаем все значения AudioType
        var allTypes = System.Enum.GetValues(typeof(AudioType))
            .Cast<AudioType>()
            .ToList();

        // Получаем существующие типы из audioData
        var existingTypes = new HashSet<AudioType>();
        for (int i = 0; i < audioData.arraySize; i++)
        {
            var element = audioData.GetArrayElementAtIndex(i);
            var type = (AudioType)element.FindPropertyRelative("type").intValue;
            existingTypes.Add(type);
        }

        // Добавляем отсутствующие типы
        bool changed = false;
        foreach (var type in allTypes)
        {
            if (!existingTypes.Contains(type))
            {
                int newIndex = audioData.arraySize;
                audioData.InsertArrayElementAtIndex(newIndex);
                var newElement = audioData.GetArrayElementAtIndex(newIndex);
                newElement.FindPropertyRelative("type").intValue = (int)type;
                changed = true;

                Debug.Log($"Added new AudioType: {type}");
            }
        }

        if (changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            Debug.Log("AudioData synchronized with AudioType enum");
        }
        else
        {
            Debug.Log("AudioData is already up to date with AudioType enum");
        }
    }

    void DrawGroupedAudio()
    {
        var groups = new Dictionary<string, List<int>>();

        for (int i = 0; i < audioData.arraySize; i++)
        {
            var element = audioData.GetArrayElementAtIndex(i);
            var type = (AudioType)element.FindPropertyRelative("type").intValue;

            string category = GetCategory(type);
            if (!groups.TryGetValue(category, out var list))
            {
                list = new List<int>();
                groups.Add(category, list);
            }

            list.Add(i);
        }

        foreach (var group in groups)
        {
            foldouts.TryAdd(group.Key, true);

            foldouts[group.Key] =
                EditorGUILayout.Foldout(foldouts[group.Key], group.Key, true);

            if (!foldouts[group.Key])
                continue;

            EditorGUI.indentLevel++;

            foreach (int index in group.Value)
            {
                var element = audioData.GetArrayElementAtIndex(index);
                DrawAudio(element);
                EditorGUILayout.Space(3);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }

    void DrawAudio(SerializedProperty property)
    {
        var type = property.FindPropertyRelative("type");
        var playMode = property.FindPropertyRelative("playMode");

        var clip = property.FindPropertyRelative("clip");
        var clips = property.FindPropertyRelative("clips");

        var loop = property.FindPropertyRelative("loop");
        var sfx3D = property.FindPropertyRelative("sfx3D");
        var mute = property.FindPropertyRelative("mute");

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        // Имя AudioType
        GUILayout.Label(type.enumDisplayNames[type.enumValueIndex],
            EditorStyles.boldLabel,
            GUILayout.ExpandWidth(true));

        // Группа кнопок с фиксированным размером, прижатая к правому краю
        GUILayout.FlexibleSpace();

        // Фиксированный размер для кнопок
        float buttonWidth = 60f;

        // Кнопки
        bool random = playMode.enumValueIndex == (int)AudioPlayMode.Random;
        random = GUILayout.Toggle(random, "Random", "Button", GUILayout.Width(buttonWidth));
        playMode.enumValueIndex = random ? (int)AudioPlayMode.Random : (int)AudioPlayMode.Single;

        DrawBoolButton(loop, "Loop", buttonWidth);
        DrawBoolButton(sfx3D, "3D", buttonWidth);
        DrawBoolButton(mute, "Mute", buttonWidth);

        EditorGUILayout.EndHorizontal();

        if (random)
            EditorGUILayout.PropertyField(clips, true);
        else
            EditorGUILayout.PropertyField(clip);

        EditorGUILayout.PropertyField(property.FindPropertyRelative("volume"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("pitch"));

        EditorGUILayout.EndVertical();
    }

    static void DrawBoolButton(SerializedProperty prop, string text, float width)
    {
        prop.boolValue = GUILayout.Toggle(prop.boolValue, text, "Button", GUILayout.Width(width));
    }

    static string GetCategory(AudioType type)
    {
        var field = typeof(AudioType).GetField(type.ToString());

        var attr = field?.GetCustomAttribute<AudioCategoryAttribute>();

        return attr?.Name ?? "Other";
    }
}