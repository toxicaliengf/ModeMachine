using ModeMachine;
using UnityEditor;
using UnityEngine;

namespace ModeMachine_Editor
{
    //ok so this is because weirdly the mode stack property drawer throws an exception when it's not part of a custom editor. idk why but including this fixes it
    //the exception is "ArgumentException: Getting control 1's position in a group with only 1 controls when doing repaint"
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    class MonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying && (target as MonoBehaviour).gameObject.scene.isLoaded && target is IModeStack)
                Repaint();
            base.OnInspectorGUI();
        }
    }
}
