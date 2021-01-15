using UnityEditor;
using ModeMachine;
using UnityEngine;
using System.Reflection;

namespace ModeMachine_Editor
{
    [CustomEditor(typeof(Mode), true)]
    [CanEditMultipleObjects]
    class ModeEditor : Editor
    {
        string result = "";
        IModeStack parentStack;
        Object parentObject;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //draw field for parent stack
            Mode targetMode = target as Mode;

            //use reflection to find the actual stack objects
            parentStack = null;
            parentObject = null;
            FieldInfo[] fieldInfos = typeof(Mode).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (fieldInfos[i].Name == "ParentStack")
                {
                    parentStack = fieldInfos[i].GetValue(targetMode) as IModeStack;
                    parentObject = parentStack as Object;
                }

            }

            //set label string
            if (parentStack == null)
            {
                result = " None";
            }
            else if (parentObject == null)
            {
                result = " " + parentStack.GetType().ToString();
            }
            else
            {
                result = " " + parentObject.name;
            }

            //draw a divider
            Rect r = EditorGUILayout.GetControlRect();
            r.y += r.height / 1.75f;
            ModeStackEditor.DrawDivider(r);

            //draw prefix
            Rect clickArea = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Parent Stack:");

            //handle click event
            Event current = Event.current;
            if (parentObject != null && clickArea.Contains(current.mousePosition))
            {
                if (current.type == EventType.MouseDown)
                {
                    if (current.clickCount == 1)
                    {
                        EditorGUIUtility.PingObject(parentObject);
                        current.Use();
                    }
                    else if (current.clickCount == 2)
                    {
                        Selection.activeObject = parentObject;
                        current.Use();
                    }
                }
            }

            //draw the field
            GUILayout.Button(result, EditorStyles.objectField);
            EditorGUILayout.EndHorizontal();
        }
    }
}
