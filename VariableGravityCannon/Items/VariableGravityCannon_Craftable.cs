using Nautilus.Assets;
using IndigocoderLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using System.IO;
using Nautilus.Extensions;
using VariableGravityCannon.Monos;
using UnityEngine;

namespace VariableGravityCannon.Items
{
    public static class VariableGravityCannon_Craftable
    {
        public static TechType techType { get; private set; }

        public static void Patch(RepulsionCannon repulsionCannonPrefab)
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("variableGravityCannon.png", Main_Plugin.AssetsFolderPath);

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("VariableGravityCannon",
                "Variable Gravity Cannon", "A propulsion and repulsion cannon in one small package!")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.PropulsionCannon);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                gameObject.SetActive(false);

                RepulsionCannon repulsionCannon = gameObject.AddComponent<RepulsionCannon>();
                SetPropCannonValues(ref repulsionCannon, gameObject, repulsionCannonPrefab);

                PropulsionCannonWeapon propulsionCannonWeapon = gameObject.GetComponent<PropulsionCannonWeapon>();
                propulsionCannonWeapon.drawTime = .2f;

                Renderer renderer = gameObject.transform.Find("1st person model/Propulsion_Cannon_anim/Propulsion_Cannon_geo").GetComponent<Renderer>();

                VariableGravityCannon_Mono variableGrav = gameObject.AddComponent<VariableGravityCannon_Mono>();
                variableGrav.Initialize(repulsionCannon, propulsionCannonWeapon, renderer);
                variableGrav.ToggleCannonType(); //Set to propulsion cannon as default

                gameObject.SetActive(true);
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.RepulsionCannon).WithAnalysisTech(null);
            prefab.SetEquipment(EquipmentType.Hand);

            RecipeData recipe = Main_Plugin.GetRecipeFromJson(Path.Combine(Main_Plugin.RecipesFolderPath, "VariableGravityCannonRecipe.json"));

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(6f);

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }

        private static void SetPropCannonValues(ref RepulsionCannon repulsionCannon, GameObject gameObject, RepulsionCannon repulsionCannonPrefab)
        {
            repulsionCannon.enabled = false;
            repulsionCannon.pickupable = gameObject.GetComponent<Pickupable>();
            repulsionCannon.fxControl = gameObject.GetComponent<VFXController>();
            repulsionCannon.mainCollider = gameObject.GetComponent<CapsuleCollider>();
            repulsionCannon.drawSound = repulsionCannonPrefab.drawSound;
            repulsionCannon.shootSound = repulsionCannonPrefab.shootSound;
            repulsionCannon.bubblesFX = repulsionCannonPrefab.bubblesFX;
            repulsionCannon.muzzle = gameObject.transform.Find("1st person model/Propulsion_Cannon_anim/body/muzzle");
            repulsionCannon.animator = gameObject.transform.Find("1st person model/Propulsion_Cannon_anim").GetComponent<Animator>();
            repulsionCannon.reloadMode = PlayerTool.ReloadMode.Animation;
            repulsionCannon.callBubblesFX = false;
            repulsionCannon.leftHandIKTarget = gameObject.transform.Find("1st person model/Propulsion_Cannon_anim/leftAttach/left_hand_target");
            repulsionCannon.ikAimRightArm = true;
            repulsionCannon.ikAimLeftArm = true;
            repulsionCannon.hasFirstUseAnimation = true;
            repulsionCannon.savedIkAimRightArm = true;
            repulsionCannon.drawTime = 0;
        }
    }
}
