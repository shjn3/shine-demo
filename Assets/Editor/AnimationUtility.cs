using UnityEditor;
using UnityEngine;


public class AnimationUtility : EditorWindow
{
    [MenuItem("Shine/AnimationUtility")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow<AnimationUtility>();

    }

    public AnimationClip animationClip;
    public AnimationClip removeAnimationClip;

    public RuntimeAnimatorController animationController;
    void OnGUI()
    {
        GUILayout.Label("Add animationClip to Controller", EditorStyles.boldLabel);

        GUILayout.Label("Controller");
        animationController = (RuntimeAnimatorController)EditorGUILayout.ObjectField(animationController, typeof(RuntimeAnimatorController), true);
        GUILayout.Label("Animation clip");
        animationClip = (AnimationClip)EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), true);

        if (GUILayout.Button("AddAsset"))
        {
            AddClipToAnimatorController(animationController, animationClip);
        }

        GUILayout.Label("Remove Animation clip");
        removeAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(removeAnimationClip, typeof(AnimationClip), true);

        if (GUILayout.Button("RemoveAsset"))
        {
            RemoveClipFromAnimatorController(removeAnimationClip);
        }


    }

    private void AddClipToAnimatorController(RuntimeAnimatorController animatorController, AnimationClip clip)
    {
        if (AssetDatabase.IsSubAsset(clip))
        {
            Debug.Log("clip is sub asset");
            return;
        }
        var clipCopy = Object.Instantiate(clip) as AnimationClip;
        clipCopy.name = animationClip.name;
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(clip));
        AssetDatabase.AddObjectToAsset(clipCopy, animatorController);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    private void RemoveClipFromAnimatorController(AnimationClip clip)
    {
        AssetDatabase.RemoveObjectFromAsset(clip);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}