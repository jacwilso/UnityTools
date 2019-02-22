using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorShortcuts {
    [MenuItem ("Edit/Pause %p")]
    public static void PauseEditorApplication () {
        EditorApplication.isPaused = true;
    }

    [MenuItem ("Edit/Pause %p", true)]
    public static bool ValidatePauseEditorApplication () {
        return EditorApplication.isPlaying;
    }

    // EditorApplication.ExecuteMenuItem()

    [MenuItem ("Edit/Clear %l")]
    public static void ClearConsole () {
        Type.GetType ("UnityEditor.LogEntries,UnityEditor.dll")
            .GetMethod ("Clear", BindingFlags.Static | BindingFlags.Public)
            .Invoke (null, null);
    }

    public static System.Type[] GetAllDerivedTypes (this System.AppDomain aAppDomain, System.Type aType) {
        var result = new List<System.Type> ();
        var assemblies = aAppDomain.GetAssemblies ();
        foreach (var assembly in assemblies) {
            var types = assembly.GetTypes ();
            foreach (var type in types) {
                if (type.IsSubclassOf (aType))
                    result.Add (type);
            }
        }
        return result.ToArray ();
    }

    private static Rect? ApplicationScreenRect = null;
    public static Rect GetApplicationScreenRect () {
        if (ApplicationScreenRect.HasValue) {
            return ApplicationScreenRect.Value;
        }
        var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes (typeof (ScriptableObject)).Where (t => t.Name == "ContainerWindow").FirstOrDefault ();
        if (containerWinType == null)
            throw new System.MissingMemberException ("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
        var showModeField = containerWinType.GetField ("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var positionProperty = containerWinType.GetProperty ("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (showModeField == null || positionProperty == null)
            throw new System.MissingFieldException ("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
        var windows = Resources.FindObjectsOfTypeAll (containerWinType);
        foreach (var win in windows) {
            var showmode = (int) showModeField.GetValue (win);
            if (showmode == 4) // main window
            {
                ApplicationScreenRect = (Rect) positionProperty.GetValue (win, null);
                return ApplicationScreenRect.Value;
            }
        }
        throw new System.NotSupportedException ("Can't find internal main window. Maybe something has changed inside Unity");
    }

    public static void CenterOnApplicationWindow (this UnityEditor.EditorWindow window, Vector2? size = null) {
        var main = GetApplicationScreenRect ();
        var pos = window.position;
        pos.size = size.GetValueOrDefault (pos.size);
        float w = (main.width - pos.width) * 0.5f;
        float h = (main.height - pos.height) * 0.5f;
        pos.x = main.x + w;
        pos.y = main.y + h;
        window.position = pos;
    }
}