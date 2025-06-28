# RimWorld 1.6 Mod Updates

*Source: [RimWorld Wiki - Modding Tutorials](https://rimworldwiki.com/wiki/Modding_Tutorials/RimWorld_1.6_Mod_Updates)*

**NOTICE**: This is a repository of information for mod developers updating their mods to RimWorld 1.6. **THERE MAY BE ODYSSEY DLC SPOILERS HERE.**

For questions and clarifications, please come visit us at the #mod-development channel on the RimWorld Discord server.

## General Tips

If you are updating a mod between RimWorld versions for the first time:

1. Update your supportedVersions in your About.xml
2. Consider using LoadFolders to freeze your previous version content.
3. Try to recompile your assemblies. Address any issues that come up.
4. Try to load your mod. Work the error list top to bottom.
5. Check your mod functionality. This is a great time to create a functionality checklist for future updates if you haven't made one already!
6. Check back after further patches. Updates and additional content may be released in post-release patches for months to come, don't assume that mods never break once updated!

## Official Documentation

The following docs have been provided directly by Ludeon for players and mod authors:

**1.6 Change Log**: Official Google Doc - describes all base game changes in RimWorld 1.6.

**1.6 Modder Primer**: Official Google Doc - technical document intended for modders.

## RimWorld 1.6 Base Game Changes

### Designators

* As part of the new vanilla designator shapes system, the virtual DraggableDimensions property has been replaced with DrawStyleCategory, which returns a DrawStyleCategoryDef. You can use one of the several vanilla presets or create your own Def to determine what kinds of shapes can be drawn with your designator.

### Buildings

* `<placingDraggableDimensions>` has been changed to `<drawStyleCategory>` to utilize the new vanilla designator shapes system. This is a DrawStyleCategoryDef, and you can use one of the several vanilla presets or create your own if that makes more sense. The vanilla presets are: Default2D, Fill2D, Default1D, FilledRectangle, Walls, Conduits, Defenses, Zones, Areas, Orders, Mine, Paint, Plans, RemovePlans, RemoveZones, Foundations, Floors, Cancel, and Plants.
* `<soundAmbient>` has been removed in 1.6 in favor of a new ThingComp. The following is from the Toxifier Generator:

```xml
<li Class="CompProperties_AmbientSound">
    <sound>Toxifier_Working</sound>
    <disabledOnUnpowered>true</disabledOnUnpowered>
</li>
```

### Pawns

* `<wildness>` is no longer a field on RaceProperties, but is now a StatDef that goes in a race's `<statBases>`. Note that as a StatDef, it is now capitalized as well (`<Wildness>`).
* `<forceGender>` is a new field in RaceProperties. Unsure why this exists in addition to the existing `<fixedGender>` field on PawnKindDef.

### Damage

* DamageDefs now have an `<ignoreShields>` flag that causes them to ignore shield belts. This does not allow projectiles using them to ignore projectile interceptors such as high or low shields. Shield belts now check for both `<ignoreShields>` and `<isRanged>`.

### Hediffs

* `<causesNeed>` has been removed. For drugs specifically, you should now use `<chemicalNeed>` on the HediffDef root, or for non-drug needs you can use the `<enablesNeeds>` list in HediffStage.
* `<disablesNeeds>` has been moved to HediffStage as well.

### Terrains

* `<holdSnow>` has been renamed to `<holdSnowOrSand>`.

### Plants

* `<hideAtSnowDepth>` has been renamed to `<hideAtSnowOrSandDepth>`.

### Scenarios

* There is a new `ScenarioBase` abstract base for ScenarioDefs. You should inherit from this in order to inherit some new fields that deal with the map layering system; omitting them will cause errors on load.

### Interactions

* `InteractionUtility` has been renamed `SocialInteractionUtility`. There is still a class called `InteractionUtility` that appears to be for ordering pawns to interact with objects.

### Misc

* ActiveDropPodInfo has been renamed to ActiveTransporterInfo - potentially used by Odyssey shuttles now?
* The targetParams `<canTargetMutants>` has been renamed to `<canTargetSubhumans>`.

## Visual Studio and Burst

If you've decompiled the dll, exported all code to .cs files, and opened that in Visual Studio and are getting constant crashes and hangs:

Edit the file Properties/AssemblyInfo.cs and delete all the lines with "`BurstCompiler`". It's not like you're re-compiling it, so these are not needed anyway.

## Odyssey DLC Compatibility

The following features appear to be DLC-locked. Note that this information has been gathered via datamining and may not reflect the final state of the game at release, nor is it necessarily comprehensive:

* Gravships
* Transport Shuttles
* Landmarks, Tile Mutators, and several new GenSteps
* Space, Asteroid Quests, Oxygen, Vacuum
* Aggressive Habitat Animals (needs clarification)
* Coastal Animals (needs clarification)
* Fishing
* Mixed Biome Maps
* Extended animal capabilities: Foraging, Targeted Attack, Sentience
* New Weather: Droughts, Frozen Water, Floods, Sandstorms
* Plant Cultivation (needs clarification)
* New Questgivers: Traders, Beggars, Books, Orbital Scanners (?)
* Substructure (needs clarification)
* New Precepts: Fishing, Space, Colony Moving

## Migration Checklist for RimWorld 1.6

### XML Changes Required:
1. **Update About.xml** - Add 1.6 to supportedVersions
2. **Buildings** - Replace `<placingDraggableDimensions>` with `<drawStyleCategory>`
3. **Buildings** - Replace `<soundAmbient>` with CompProperties_AmbientSound
4. **Pawns** - Move `<wildness>` to `<statBases>` as `<Wildness>`
5. **Hediffs** - Replace `<causesNeed>` with `<chemicalNeed>` or `<enablesNeeds>`
6. **Terrains** - Rename `<holdSnow>` to `<holdSnowOrSand>`
7. **Plants** - Rename `<hideAtSnowDepth>` to `<hideAtSnowOrSandDepth>`
8. **Scenarios** - Inherit from ScenarioBase
9. **Targeting** - Rename `<canTargetMutants>` to `<canTargetSubhumans>`

### Code Changes Required:
1. **Designators** - Update DraggableDimensions to DrawStyleCategory
2. **Interactions** - Update InteractionUtility references to SocialInteractionUtility
3. **Transport** - Update ActiveDropPodInfo to ActiveTransporterInfo
4. **Damage** - Add `<ignoreShields>` flag to DamageDefs if needed

### Testing Steps:
1. Recompile assemblies
2. Load mod and check error log
3. Test core functionality
4. Verify compatibility with Odyssey DLC features (if applicable)

---

*Last updated: June 24, 2025*
*Source: [RimWorld Wiki - Modding Tutorials](https://rimworldwiki.com/wiki/Modding_Tutorials/RimWorld_1.6_Mod_Updates)* 