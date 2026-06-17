# The Everything Mod

A content mod for **Hybrid Animals** (BepInEx / IL2CPP) that adds craftable items, furniture, and custom interactable containers, including a per-player "ender chest" and a "shulker" that keeps its contents when moved.

![BepInEx](https://img.shields.io/badge/BepInEx-IL2CPP-f7a84f?style=flat)
![.NET](https://img.shields.io/badge/.NET-10-4f8ef7?style=flat)
![Hybrid Animals](https://img.shields.io/badge/Hybrid%20Animals-Mod-f7a84f?style=flat)
![License](https://img.shields.io/badge/License-GPLv3-4f8ef7?style=flat)
![Stars](https://img.shields.io/github/stars/Segually/TheEverythingModHA?style=flat&color=f7a84f&label=Stars)

## What It Adds

**Items & gear**
* **Ancient Gold Alloy**, a refined crafting material (Crucible)
* **TGA Armor**, clan armor with custom paint layouts (Crucible)
* **Spartan Helm**, a horned, plumed helmet (Crucible)
* **Pinetree Pioneer Hat**, function over fashion (Loom)

**Furniture & containers**
* **Modern Bed**, a sleepable, paintable bed (Crafting Table)
* **Uranium Chest**, a 3-page storage container (Anvil)
* **Tele-Chest**, a personal "ender chest": every player sees only their own contents in any Tele-Chest, keyed to their username and persistent across sessions
* **Perma-chest**, a "shulker" that keeps its stored items when it's picked up and moved

## How It Works

Built on the **[Hybrid Animals Mod Helper](https://github.com/eris-webserv/HAModHelper)** API:

* **Items & recipes** are registered through `ItemManager` and slotted into vanilla crafting stations with `CraftingInjectionManager`.
* **Custom appearances** are served from embedded Unity AssetBundles through `AssetBundleManager`.
* **Interactables** (beds, multi-page containers, and fully custom behaviours) are wired through `InteractableManager`, with the game's interactable and basket internals reverse-engineered to drive opening, storage, and persistence (including the Tele-Chest's per-player keying and the Perma-chest's keep-on-pickup behaviour).

## Requirements

* **Hybrid Animals** (IL2CPP build)
* **BepInEx 6** (Unity IL2CPP)
* **HAModHelper.GamePlugin** (hard dependency)

> **Note:** when a feature this mod needs isn't in HAModHelper yet, it may ship against a custom build of the helper until the change lands upstream.

## Building

```sh
dotnet build HybridAnimalsMod/HybridAnimalsMod.csproj -c Release
```

NuGet dependencies (BepInEx and `HybridAnimals.GameLibs.Android`) restore from the feeds configured in `nuget.config`. The build output `TheEverythingMod.dll` is the plugin.

## Installation

1. Install **BepInEx 6 (IL2CPP)** into your Hybrid Animals install. The easiest way to set this up is the **[FusionCore launcher](https://github.com/All-Of-Us-Mods/FusionCore)**, which handles BepInEx for you.
2. Drop **`HAModHelper.GamePlugin.dll`** and **`TheEverythingMod.dll`** into `BepInEx/plugins`.
3. Launch the game.

## Known Limitations

Issues we're aware of and don't plan to fix:

* The Perma-chest still shows the vanilla "everything inside will disappear" warning when you pick it up, even though the contents are actually kept.
* The Tele-Chest and Perma-chest don't share their contents across different worlds or servers. Storage is per world and per server.

## A Note on Development

Part of this mod's logic was written with AI assistance. Much of the reverse-engineering and interactable wiring was worked out using **[Claude Code](https://claude.com/product/claude-code)**, which I'd recommend for this kind of RE and modding work.

## Credits

* **[HAModHelper](https://github.com/eris-webserv/HAModHelper)**, the modding API this is built on
* **[Hybrid Animals Multiplayer](https://github.com/eris-webserv/HAMP)**, the multiplayer revival

## License

Licensed under the **[GNU General Public License v3.0](LICENSE)**. Use it, modify it, and share it freely (no credit required), but any project that incorporates part of it must also be released as open source under the same license.
