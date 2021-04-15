using UnityEngine;
using UnityEditor;
using ModeMachine;

namespace ModeMachine_Editor
{
    [CustomPropertyDrawer(typeof(ModeStack))]
    public class ModeStackEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object p = SerializeableExtensions.GetParent(property);
            if (p is IModeStack)
            { 
                ModeStack stack = fieldInfo.GetValue(property.serializedObject.targetObject) as ModeStack;
                ModeStack m = (p as IModeStack).ModeStack;

                if(stack == m)
                    DrawStackInspector(stack, position, property, label);
            }

        }


        //editor state 
        static bool expandModeList = true;
        static bool expandStackDrawer = true;

        void DrawStackInspector(ModeStack targetStack, Rect position, SerializedProperty property, GUIContent label)
        {
            int startingIndent = EditorGUI.indentLevel;

            //draw a foldout to contain a list of all the modes on the stack
            EditorGUI.indentLevel++;
            expandStackDrawer = EditorGUI.Foldout(position, expandStackDrawer, "IModeStack " + label.text, true);
            if (expandStackDrawer)
            {
                EditorGUI.indentLevel++;
                if (!Application.isPlaying)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                    style.fontSize += 2;
                    EditorGUILayout.LabelField("[not available outside of play mode]", style);

                    EditorGUI.indentLevel = startingIndent;
                }
                else
                {
                    //draw list of modes
                    expandModeList = EditorGUILayout.Foldout(expandModeList, "Modes count: " + targetStack.Modes.Count, true);
                    if (expandModeList)
                    {
                        for (int i = 0; i < targetStack.Modes.Count; i++)
                        {
                            Rect clickArea = EditorGUILayout.BeginHorizontal();

                            //mode index / depth label
                            EditorGUILayout.PrefixLabel(i.ToString());

                            //check click event (use this instead of button to detect double clicks)
                            Event current = Event.current;
                            if (clickArea.Contains(current.mousePosition))
                            {
                                if (current.type == EventType.MouseDown)
                                {
                                    if (current.clickCount == 1)
                                    {
                                        EditorGUIUtility.PingObject(targetStack.Modes[i]);
                                        current.Use();
                                    }
                                    else if (current.clickCount == 2)
                                    {
                                        Selection.activeGameObject = targetStack.Modes[i].gameObject;
                                        current.Use();
                                    }
                                }
                            }

                            //draw the mode
                            GUILayout.Button(targetStack.Modes[i].name, EditorStyles.objectField);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
            EditorGUI.indentLevel = startingIndent;
        }

    }
}