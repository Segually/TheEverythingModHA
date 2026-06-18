using System;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HAModHelper.GamePlugin.Assets.Systems;
using HAModHelper.GamePlugin.Items.Systems;


namespace HybridAnimalsMod;

[BepInPlugin("dev.segually.everythingmod", "The Everything Mod!", "v0.0.1")]
// Hard dependency: this plugin calls HAModHelper.GamePlugin types in Load(), so BepInEx must load
// HAModHelper first. Without this, plugin load order is undefined and the mod throws
// FileNotFoundException for HAModHelper.GamePlugin on machines that load this plugin first.
[BepInDependency("HAModHelper.GamePlugin", BepInDependency.DependencyFlags.HardDependency)]
public partial class HybridAnimalsTemplatePlugin : BasePlugin
{
    public override void Load()
    {
        // This mod's own Harmony patches (Perma-chest basket behaviour). Separate from the helper's.
        new Harmony("dev.segually.everythingmod").PatchAll();

        AssetBundleManager.Instance
.RegisterBundleFromEmbeddedResource(
Assembly.GetExecutingAssembly(),
"TheEverythingMod.Bundles.TGA.ancientgold",
"AncientGoldAlloy")
.AddPrefab("Assets/Prefabs/AncientGoldAlloy.prefab", "everythingmodbysegual:goldalloy", "goldalloy");


        AssetBundleManager.Instance
            .RegisterBundleFromEmbeddedResource(
                Assembly.GetExecutingAssembly(),
                "TheEverythingMod.Bundles.TGA.tga",
                "IceArmor")
            .AddPrefab("Assets/Prefabs/IceArmor.prefab", "everythingmodbysegual:tgaarmor", "tgaarmor");

        AssetBundleManager.Instance
    .RegisterBundleFromEmbeddedResource(
        Assembly.GetExecutingAssembly(),
        "TheEverythingMod.Bundles.ModernBed.modernbed",
        "ModernBed")
    .AddPrefab("Assets/Prefabs/ModernBed.prefab", "everythingmodbysegual:modernbed", "modernbed");


        // The uraniumchest bundle contains BOTH prefabs (uraniumchest.prefab and telechest.prefab),
        // so it's registered once and both prefabs are added to that single bundle. Do NOT register
        // the same bundle a second time under another id — Unity refuses to load identical AssetBundle
        // content twice, so the duplicate load fails and its prefab never resolves.
        AssetBundleManager.Instance
.RegisterBundleFromEmbeddedResource(
Assembly.GetExecutingAssembly(),
"TheEverythingMod.Bundles.UraniumChest.uraniumchest",
"uraniumchest")
.AddPrefab("Assets/Prefabs/uraniumchest.prefab", "everythingmodbysegual:uraniumchest", "uraniumchest")
.AddPrefab("Assets/Prefabs/telechest.prefab", "everythingmodbysegual:telechest", "telechest")
.AddPrefab("Assets/Prefabs/permachest.prefab", "everythingmodbysegual:permachest", "permachest");



        AssetBundleManager.Instance
.RegisterBundleFromEmbeddedResource(
Assembly.GetExecutingAssembly(),
"TheEverythingMod.Bundles.Pioneer.pioneerhats",
"pioneerhats")
.AddPrefab("Assets/Prefabs/pioneerhats.prefab", "everythingmodbysegual:pioneerhat", "pioneerhat");


        AssetBundleManager.Instance
.RegisterBundleFromEmbeddedResource(
Assembly.GetExecutingAssembly(),
"TheEverythingMod.Bundles.TGA.spartahelm",
"SpartasHelmet")
.AddPrefab("Assets/Prefabs/SpartasHelmet.prefab", "everythingmodbysegual:spartahelm", "spartahelm");


        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "goldalloy",
            Name = "Ancient Gold Alloy",
            StackLimit = 1,
            Description = "A metallic bar combining the properties of raw gold and ancient bone calcium.",
            ExtraFields = new()
            {
                ["Crafting_ingredient_A"] = "Gold Ore",
                ["Crafting_ingredient_A_count"] = "1",
                ["Crafting_ingredient_B"] = "Ancient Bones",
                ["Crafting_ingredient_B_count"] = "1",
                ["Default Coloring"] = "Weapon - Magma",
                ["Copy3dModelPosition"] = "Wizard Hat",
                // Resolves to Addressables key "Assets/Prefabs/TUNG_BAT.prefab", served from tungbat.bundle.
                ["World_obj_path"] = "AncientGoldAlloy",
                ["Craft_required_lvl"] = "270", // 270
                ["Vendor0"] = "Refined Materials",
                ["show_model3d"] = "true",
            },
        });

        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "tgaarmor",
            Name = "TGA Armor",
            StackLimit = 1,
            Description = "The armor worn by the members of TGA. It is adorned with the TGA logo on the back, the TGA Badge on the front and the Alliance colors.",
            ExtraFields = new()
            {
                ["Crafting_ingredient_A"] = "everythingmodbysegual:goldalloy",
                ["Crafting_ingredient_A_count"] = "5",
                ["Crafting_ingredient_B"] = "Amethyst",
                ["Crafting_ingredient_B_count"] = "1",
                // ["Default Coloring"] = "Weapon - Spirit",
                ["Type"] = "Armor",
                // Resolves to Addressables key "Assets/Prefabs/TUNG_BAT.prefab", served from tungbat.bundle.
                ["World_obj_path"] = "IceArmor",
                ["Copy3dModelPosition"] = "Ancient Armor",
                ["Equip_required_stat"] = "Health",
                ["Equip_required_stat_lvl"] = "300", // 300
                ["Craft_required_lvl"] = "300", //300
                ["Vendor0"] = "Armor",
                ["Defend Amount"] = "99",
                ["show_model3d"] = "true",
                ["weapon_display_copy"] = "Ancient Scythe",
            },
        });

        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "pioneerhat",
            Name = "Pinetree Pioneer Hat",
            StackLimit = 1,
            Description = "Form and function over fashion.",
            ExtraFields = new()
            {
                ["Crafting_ingredient_A"] = "Silk",
                ["Crafting_ingredient_A_count"] = "4",
                ["Default Coloring"] = "Basic Blue",
                ["Type"] = "Helmet",
                ["Helmet Style"] = "top_of_head",
                ["Copy3dModelPosition"] = "Wizard Hat",
                // Resolves to Addressables key "Assets/Prefabs/TUNG_BAT.prefab", served from tungbat.bundle.
                ["World_obj_path"] = "pioneerhats",
                ["Craft_required_lvl"] = "14",
                ["Vendor0"] = "Clothing",
                ["show_model3d"] = "true",
            },
        });

        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "spartahelm",
            Name = "Spartan Helm",
            StackLimit = 1,
            Description = "The Helmet worn by the Spartans, members of the clan Sparta. It has four prominent horns on the back and a red plume.",
            ExtraFields = new()
            {
                ["Crafting_ingredient_A"] = "Sharp Tooth",
                ["Crafting_ingredient_A_count"] = "4",
                ["Crafting_ingredient_B"] = "everythingmodbysegual:goldalloy",
                ["Crafting_ingredient_B_count"] = "3",
                ["Default Coloring"] = "Weapon - Ancient",
                ["Type"] = "Helmet",
                ["Helmet Style"] = "top_of_head",
                ["Copy3dModelPosition"] = "Metal Legion Helm",
                ["World_obj_path"] = "SpartasHelmet",
                ["Equip_required_stat"] = "Health",
                ["Equip_required_stat_lvl"] = "300", // 300
                ["Craft_required_lvl"] = "300", //300
                ["Vendor0"] = "Armor",
                ["Defend Amount"] = "99",
                ["show_model3d"] = "true",
                ["PaintLayout0_SolidColors"] = "0-2, 1-3, 2-6, 3-1, ",
                ["PaintLayout0_TriColor"] = "0-1, 1-3, 2-2, 3-3",
                ["PaintLayout0_Trimmed"] = "0-1, 1-3, 2-2, 3-3",
                ["PaintLayout0_Pastels"] = "0-1, 1-3, 2-2, 3-3 ",
                ["PaintLayout0_Candycane"] = "0-1, 1-3, 2-2, 3-3",
                ["PaintLayout0_Nomad"] = "0-1, 1-4, 2-2, 3-3",
                ["PaintLayout0_Goldy"] = "0-1, 1-2, 2-3, 3-2",
                ["PaintLayout0_Stone & Wood"] = "0-4, 1-6, 2-1, 3-3, ",
            },
        });


        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "modernbed",
            Name = "Modern Bed",
            StackLimit = 1,
            Description = "Way more comfortable than a regular bed.",
            ExtraFields = new()
            {
                ["Crafting_ingredient_A"] = "Stick",
                ["Crafting_ingredient_A_count"] = "1",
                //["Crafting_ingredient_B"] = "Amethyst",
                //["Crafting_ingredient_B_count"] = "1",
                ["PaintLayout0_SolidColors"] = "0-6, 1-5, 2-2, 3-4, ",
                ["PaintLayout0_TriColor"] = "0-2, 1-3, 2-1, 3-3, ",
                ["PaintLayout0_Trimmed"] = "0-3, 1-2, 2-1, 3-3, ",
                ["PaintLayout0_Pastels"] = "0-3, 1-2, 2-1, 3-3, ",
                ["PaintLayout0_Candycane"] = "0-2, 1-3, 2-1, 3-3, ",
                ["PaintLayout0_Nomad"] = "0-2, 1-3, 2-1, 3-4, ",
                ["PaintLayout0_Goldy"] = "0-2, 1-3, 2-1, 3-2, ",
                ["PaintLayout0_Stone & Wood"] = "0-4, 1-6, 2-1, 3-3, ",
                ["Default Coloring"] = "Lumberjack",
                ["World_obj_path"] = "ModernBed",
                ["Vendor0"] = "Furniture",
                ["Type"] = "Place_in_world",
                ["World_geometry"] = "x_plus_1",
                ["model3d_camDist"] = "3.9",
                ["show_model3d"] = "true",
                ["model3d_fov"] = "40",
                ["model3d_xRot"] = "-100",
                ["model3d_camHeight"] = "0.3",
                ["model3d_yRot"] = "40",
                ["model3d_recenterX"] = "-14",
                ["model3d_recenterY"] = "-17",
                ["paintable_by_player"] = "true",
            },
        });



        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "uraniumchest",
            Name = "Uranium Chest",
            StackLimit = 1,
            Description = "This chest may be small, but it has plenty of storage for your needs.",
            ExtraFields = new()
            {
                ["Craft_required_lvl"] = "180",
                ["Crafting_ingredient_A"] = "Uranium Bar",
                ["Crafting_ingredient_A_count"] = "4",
                ["Crafting_ingredient_B"] = "Chest",
                ["Crafting_ingredient_B_count"] = "1",
                ["Default Coloring"] = "Weapon - Uranium",
                ["World_obj_path"] = "uraniumchest",
                ["Vendor0"] = "Furniture",
                ["Type"] = "Place_in_world",
                ["World_geometry"] = "1_by_1",
                ["show_model3d"] = "true",
                // make it copy the model3d view from Chest
                ["Copy3dModelPosition"] = "Chest",
            },
        });

        // Tele-Chest: a personal "ender chest". Same definition as the uranium chest (appearance,
        // geometry, model3d) except its id, name, and recipe (1 Stick, no second ingredient).
        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "telechest",
            Name = "Tele-Chest",
            StackLimit = 1,
            Description = "A personal chest unique to every player. Your stored items follow you to every Tele-Chest, and nobody else can see or take them.",
            ExtraFields = new()
            {
                ["Craft_required_lvl"] = "71",
                ["Crafting_ingredient_A"] = "Dark Shard",
                ["Crafting_ingredient_A_count"] = "5",
                ["Crafting_ingredient_B"] = "Chest",
                ["Crafting_ingredient_B_count"] = "1",
                ["Default Coloring"] = "Weapon - Magma",
                ["World_obj_path"] = "telechest",
                ["Vendor0"] = "Furniture",
                ["Type"] = "Place_in_world",
                ["World_geometry"] = "1_by_1",
                ["show_model3d"] = "true",
                // make it copy the model3d view from Chest
                ["Copy3dModelPosition"] = "Chest",
            },
        });

        // Perma-chest: a "shulker". Same appearance as the Tele-Chest, crafted in the crafting table
        // from a single Stick. Its contents stay with the physical object even when it's picked up and
        // moved (see OpenPermaChest).
        ItemManager.Instance.AddItem(new Item
        {
            ModId = "everythingmodbysegual",
            ItemId = "permachest",
            Name = "Perma-chest",
            StackLimit = 1,
            Description = "This chest will keep it's contents after you picked it up.",
            ExtraFields = new()
            {
                ["Craft_required_lvl"] = "53",
                ["Crafting_ingredient_A"] = "Ice Shard",
                ["Crafting_ingredient_A_count"] = "4",
                ["Crafting_ingredient_B"] = "Chest",
                ["Crafting_ingredient_B_count"] = "1",
                ["Default Coloring"] = "Weapon - Ice",
                ["World_obj_path"] = "permachest",
                ["Vendor0"] = "Furniture",
                ["Type"] = "Place_in_world",
                ["World_geometry"] = "1_by_1",
                ["show_model3d"] = "true",
                // make it copy the model3d view from Chest
                ["Copy3dModelPosition"] = "Chest",
            },
        });


        CraftingInjectionManager.Instance.Inject(
                "Crafting - Crafting Table",
                "everythingmodbysegual:modernbed",
                insertAfter: "Big Table"
        );

        CraftingInjectionManager.Instance.Inject(
        "Crafting - Crucible",
        "everythingmodbysegual:permachest",
        insertAfter: "Magma Shovel"
);

        CraftingInjectionManager.Instance.Inject(
        "Crafting - Anvil",
        "everythingmodbysegual:uraniumchest",
        insertAfter: "Uranium Hasta"
);

        CraftingInjectionManager.Instance.Inject(
        "Crafting - Crucible",
        "everythingmodbysegual:telechest",
        insertAfter: "Ice SwirlSword"
);

        CraftingInjectionManager.Instance.Inject(
"Crafting - Crucible",
"everythingmodbysegual:spartahelm",
insertAfter: "Spirit Lance"
);


        CraftingInjectionManager.Instance.Inject(
                "Crafting - Crucible",
                "everythingmodbysegual:tgaarmor",
                insertAfter: "everythingmodbysegual:spartahelm"
        );

        CraftingInjectionManager.Instance.Inject(
        "Crafting - Crucible",
        "everythingmodbysegual:goldalloy",
        insertAfter: "everythingmodbysegual:tgaarmor"
);




        CraftingInjectionManager.Instance.Inject(
        "Crafting - Loom",
        "everythingmodbysegual:pioneerhat",
        insertAfter: "Hunter Rug"
);




        InteractableManager.Instance.RegisterBed("everythingmodbysegual:modernbed");
        InteractableManager.Instance.RegisterContainer("everythingmodbysegual:uraniumchest", "Uranium Chest", 3);

        // Tele-Chest = personal "ender chest". Registered as a custom interactable: instead of the
        // object's own basket_id (it has none), interacting opens a container keyed by the player's
        // multiplayer username, so every player sees only their own contents in any Tele-Chest and the
        // contents persist across sessions. We never touch the placed object's item — we build a copy
        // carrying our computed basket_id and feed that through the normal open flow (which stores on
        // the server when connected, locally when offline), so moving/destroying the chest can't affect
        // anyone's stored items.
        InteractableManager.Instance.RegisterCustom("everythingmodbysegual:telechest", ctx =>
        {
            var inv = inventory_ctr.Instance;
            if (inv == null) return;

            var it = ctx.Interactable;
            var baseItem = it.corresponding_item;
            if (baseItem == null) return;

            int personalId = TeleChestContainerId();

            var extra = baseItem.GetExtraDataCopy();
            extra.SetLong("basket_id", personalId);
            var personalItem = new InventoryItem(baseItem.item_name, extra);

            ctx.Controller.NoteInteractingElement(
                it.origin_chunkX, it.origin_chunkZ,
                it.origin_innerX, it.origin_innerZ,
                personalItem, it.temp_rot);

            inv.TryOpenWorldContainer(
                personalItem, it.temp_rot,
                it.origin_innerX, it.origin_innerZ,
                it.origin_chunkX, it.origin_chunkZ);
        });

        // Perma-chest = "shulker": a shared container whose contents stay with the object through
        // pickup/move. Also a custom interactable (see OpenPermaChest).
        InteractableManager.Instance.RegisterCustom("everythingmodbysegual:permachest", OpenPermaChest);


    }

    // Perma-chest interaction: just open the placed object's own container. The container id lives in
    // the object's "basket_id", which the game assigns and persists at PLACEMENT — the same proven path
    // the Uranium Chest uses (reliable in both singleplayer and multiplayer). The PermaChestBasketPatch
    // below is what makes the game assign that basket_id (and keep it through pickup). No minting here;
    // the vanilla flow handles load/save.
    private static void OpenPermaChest(InteractContext ctx)
    {
        var inv = inventory_ctr.Instance;
        if (inv == null) return;

        var it = ctx.Interactable;
        var item = it.corresponding_item;
        if (item == null) return;

        ctx.Controller.NoteInteractingElement(
            it.origin_chunkX, it.origin_chunkZ,
            it.origin_innerX, it.origin_innerZ,
            item, it.temp_rot);

        inv.TryOpenWorldContainer(
            item, it.temp_rot,
            it.origin_innerX, it.origin_innerZ,
            it.origin_chunkX, it.origin_chunkZ);
    }

    // Stable container id for the local player's Tele-Chest, derived from their multiplayer username
    // (the global "username_lower", same source the save tools use). Deterministic, so it resolves to
    // the same container every session. If no username is set, a stable random one is generated and
    // persisted to the global file so the player still gets a consistent personal chest.
    private static int TeleChestContainerId()
    {
        var pd = PlayerData.Instance;
        string? username = pd?.GetGlobalString("username_lower");
        if (string.IsNullOrEmpty(username))
        {
            username = "tele_" + System.Guid.NewGuid().ToString("N").Substring(0, 12);
            pd?.SetGlobalString("username_lower", username);
        }

        // FNV-1a 32-bit: stable across sessions/machines (unlike string.GetHashCode()).
        uint hash = 2166136261u;
        foreach (char c in username)
        {
            hash ^= c;
            hash *= 16777619u;
        }

        int id = (int)(hash & 0x7FFFFFFF);
        if (id == 0) id = 1;
        return -id; // negative space never collides with the game's positive per-object basket ids
    }
}

// ── Perma-chest "shulker" behaviour ─────────────────────────────────────────────
//
// The Perma-chest needs a unique, persistent container id. The reliable way to get one is the game's
// own placement assignment: InventoryUtils.UsesBasketId(item) gates whether inventory_ctr.FinalizeItemBeforePutDown
// assigns a basket_id at put-down (and whether the game recognises it afterwards). It's a hardcoded
// item_name list, so a modded id isn't in it. Forcing it true for the Perma-chest makes the game assign
// + persist a real basket_id at placement, exactly like the Uranium Chest (works in singleplayer and
// multiplayer).
//
// To make it a SHULKER (keep contents when picked up), we return false from UsesBasketId only while
// inventory_ctr.DropExtraItemDataOnPickup is running: that method clears basket_id only when UsesBasketId
// is true, so suppressing it there leaves the basket_id on the item, and the contents travel with the
// chest when it's moved.

[HarmonyPatch(typeof(inventory_ctr), nameof(inventory_ctr.DropExtraItemDataOnPickup))]
internal static class PermaChestPickupGuard
{
    [ThreadStatic] internal static bool InPickup;

    [HarmonyPrefix]
    static void Prefix() => InPickup = true;

    [HarmonyFinalizer]
    static void Finalizer() => InPickup = false;
}

[HarmonyPatch(typeof(InventoryUtils), nameof(InventoryUtils.UsesBasketId), new Type[] { typeof(InventoryItem) })]
internal static class PermaChestUsesBasketIdPatch
{
    [HarmonyPostfix]
    static void Postfix(InventoryItem item, ref bool __result)
    {
        if (item == null || item.item_name != "everythingmodbysegual:permachest") return;
        // True normally (so placement assigns + the game keeps a basket_id); false during pickup so
        // the basket_id is NOT cleared, keeping the chest's contents when it's moved (shulker).
        __result = !PermaChestPickupGuard.InPickup;
    }
}

// Block nesting a Perma-chest inside another Perma-chest. ToContainer is the game's "store an item
// into the open container" method (it resolves the container's permitted slots, then places the item),
// and it's only invoked on a real store action, so it's the right place to veto the move. If both the
// item being stored AND the open container are Perma-chests, we cancel the drag, shut the chest, and
// show a message. Returning the full incoming count as "leftover" means nothing was stored, so the
// Perma-chest stays in the player's inventory.
[HarmonyPatch(typeof(inventory_ctr), "ToContainer", new Type[] { typeof(InventoryItem), typeof(int), typeof(inventory_ctr.ptype) })]
internal static class NoPermaChestInPermaChestPatch
{
    [HarmonyPrefix]
    static bool Prefix(InventoryItem __0, int __1, ref int __result)
    {
        try
        {
            if (__0 == null || __0.item_name != "everythingmodbysegual:permachest") return true;

            var openContainer = GameController.Instance?.interacting_element_item;
            if (openContainer == null || openContainer.item_name != "everythingmodbysegual:permachest") return true;

            var inv = inventory_ctr.Instance;
            inv?.CancelDragging();
            WindowControl.Instance?.CloseMiniwindow(true);
            PopupControl.Instance?.ShowMessage("...");

            __result = __1;  // full count reported as not-stored -> the item stays in inventory
            return false;    // skip the store
        }
        catch
        {
            return true;     // on any error, fall back to vanilla behaviour
        }
    }
}
