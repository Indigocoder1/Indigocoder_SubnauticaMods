using HarmonyLib;
using TextureReplacerEditor.Monobehaviors.Windows;
using UnityEngine;

namespace TextureReplacerEditor.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Postfix(Player __instance)
        {
            GameObject editorCanvas = Main_Plugin.AssetBundle.LoadAsset<GameObject>("TextureReplacerEditorCanvas");
            GameObject editorCanvasInstance = GameObject.Instantiate(editorCanvas);
            editorCanvasInstance.SetActive(false);
            Main_Plugin.CurrentEditorWindowInstance = editorCanvasInstance;
        }

        [HarmonyPatch(nameof(Player.Update)), HarmonyPostfix]
        private static void Update_Postfix(Player __instance)
        {
            if (!Input.GetMouseButtonDown(2)) return;

            if(TextureReplacerEditorWindow.Instance && TextureReplacerEditorWindow.Instance.IsWindowActive)
            {
                return;
            }

            RaycastHit hitInfo;
            if (!Physics.Raycast(SNCameraRoot.main.transform.position, SNCameraRoot.main.transform.forward, out hitInfo)) return;

            var prefabIdentifier = hitInfo.collider.GetComponentInParent<PrefabIdentifier>();
            if (prefabIdentifier == null) return;

            TextureReplacerEditorWindow editorWindow = Main_Plugin.CurrentEditorWindowInstance.GetComponent<TextureReplacerEditorWindow>();
            editorWindow.prefabInfoWindow.CreateChildHierarchy(prefabIdentifier.transform);
            editorWindow.prefabInfoWindow.SetPrefabIdentifier(prefabIdentifier);

            TextureReplacerEditorWindow.Instance.prefabInfoWindow.OpenWindow();
        }
    }
}
