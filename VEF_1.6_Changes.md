# Vanilla Expanded Framework 1.6 Changes

*Source: [VEF 1.6 Changes Wiki](https://github.com/Vanilla-Expanded/VanillaExpandedFramework/wiki/16Changes)*

The 1.6 version of the game brought a lot of interesting changes to the game, and we took the opportunity to take a good look at all the code we had and improve it significantly. We also listened to performance complaints and adjusted a lot of things.

If you had a project using the VE Framework, this list of changes will help you make the transition!

## PANIC! What do I do as a modder??

* Step 1, relax. We changed a lot, but removed very little. Most of the changes were internal
* Majority of the changes were namespaces, so if you had a mod using VanillaGenesExpanded.WhateverExtension you'll need to change it to VEF.Genes.WhateverExtension in your XML. We have documented all the relevant changes here!

## General changes:

* VFECore namespace changed to VEF. Individual namespaces created for all of the libraries included in the Framework, as specified below, and they all follow the same naming convention, VEF.Library
* Performance improvements on the more impactful patches
* All Harmony patches are now called "VanillaExpandedFramework_Class_Method" (why? This way if one of the patches fails it will point to the Framework, not to a namespace that might confuse people, such as VanillaGenesExpanded, a non-existent mod)

## Included libraries:

### Abilities:
* Namespace is **VEF.Abilities**

### Aesthetic scaling:
* Namespace is **VEF.AestheticScaling**

### AI:
* Namespace is **VEF.AI**
* Moved DraftedJobs here

### Animal Behaviours:
* Namespace is **VEF.AnimalBehaviours**
* NocturnalAnimals namespace moved to VEF.AnimalBehaviours
* Removed salamander code (from the old, now removed VAE Caves)

### Apparels:
* Namespace is **VEF.Apparels**
* Apparel_Shield moved here
* Moved CompApparelHediffs here
* Moved CompShield, CompShieldField and CompShieldBubble here
* Merged the two existing ApparelExtension, as they were a duplicate

### Buildings:
* Namespace is **VEF.Buildings**, renamed from VanillaFurnitureExpanded
* Moved RecipeInheritance here, extension now called RecipeInheritanceExtension instead of just ThingDefExtension
* Moved DoorTeleporter here
* Moved Building_AutoDoorLockable here
* Moved FacilityExtension here
* Moved CompBouncingArrow here
* Moved CompCustomizableGraphic here
* Moved CompCustomCauseHediff_AoE here
* Moved PlaceWorker_CompCustomCauseHediff_AoE
* Moved CompScheduleExtended here
* Moved CompStatsWhenPowered here
* Moved CompThrowMote here
* Moved PlaceWorker_DeepDrillLimitation here
* Moved PlaceWorker_AttachedToWallMultiCell here
* Moved CompHediffGiver here

### Cooking:
* Namespace is **VEF.Cooking**, instead of VanillaCookingExpanded
* Moved CompIngestedThoughtFromQuality here
* Moved IngredientValueGetter_NutritionWithExtraIngredient from VE Cooking here

### ExcludeFromQuestsExtension
* Gone, reduced to atoms. It has been deprecated for a long time. Use FactionDefExtension's excludeFromQuests field.

### Factions:
* Namespace is **VEF.Factions**
* Moved ScenPart_ForcedFactionGoodwill here
* Moved StockGenerator_ThingSetMakerTags here
* Moved FactionDefExtension here

### Genes:
* Namespace is **VEF.Genes**, instead of VanillaGenesExpanded (this one caused a lot of confusion!)
* Moved HediffCompProperties_CustomBlood and HediffComp_CustomBlood here

### Graphics:
* Namespace is **VEF.Graphics**
* Moved GraphicCustomization here
* Moved Graphic_Animated, Graphic_AnimatedMote, Graphic_DarklightMulti, Graphic_DarklightSingle, Graphic_Fleck_Animated, Graphic_FleckCollection and Graphic_StackCountByIngredient all here
* Moved GraphicOffsets here
* Moved CompGlower_DirtyMapMesh here
* Moved ThingWithFloorGraphic and FloorGraphicExtension here
* Moved HeadExtension here
* Moved ConditionalGraphics, DynamicGraphics and TaggedGraphics here

### Hediffs
* Namespace is **VEF.Hediffs**
* Moved Hediff shields here, HediffComp_DamageAura, HediffComp_Draw, HediffComp_Shield and ShieldsSystem

### ItemProcessor
* Gone, reduced to atoms. The automation of machines now uses the PipeSystem library, which is SO MUCH BETTER!

### Maps:
* Namespace is **VEF.Maps**
* Moved ActiveTerrain here
* Moved LordJob_DefendShip here
* Moved ObjectSpawnsDef here

### Memes:
* Namespace is **VEF.Memes**, instead of VanillaMemesExpanded

### OptionalFeatures
* Namespace is **VEF.OptionalFeatures**
* Externalized creation of new features using XML

### Pawns
* Namespace is **VEF.Pawns**
* Moved PawnKindDefExtension here
* Moved ThoughtExtensions here
* Moved TraitExtension here
* Moved PawnShieldGenerator here
* Moved CompProperties_DependsOnBuilding here
* Moved CompProperties_PawnDependsOn here
* Moved BackstoryDef here
* Moved PregnancyApproaches here
* Moved WorkGiver_ConstructWithSkill here
* Moved MoteAttached_TargetingLock here
* Moved ApparelDrawPosExtension here

### Planet
* Namespace is **VEF.Planet**
* MovingBases moved here
* HireableSystem moved here
* Moved LordToil_SiegeCustom here
* Moved LordToilData_SiegeCustom here
* Moved LordJob_SiegeCustom here
* Moved ScenPart_ArriveAtEdge here

### Plants
* Namespace is **VEF.Plants**
* A bunch of the plants classes in Vanilla Plants Expanded and More Plants moved here

### Research
* Namespace is **VEF.Research**

### Storyteller
* Namespace is **VEF.Storyteller**

### Sounds
* Namespace is **VEF.Sounds**

### Things
* Namespace is **VEF.Things**

### Weapons
* Namespace is **VEF.Weapons**

### Weathers
* Namespace is **VEF.Weathers**

## Deprecated Features

### ExcludeFromQuestsExtension
* Gone, reduced to atoms. It has been deprecated for a long time. Use FactionDefExtension's excludeFromQuests field.

### ItemProcessor
* Gone, reduced to atoms. The automation of machines now uses the PipeSystem library, which is SO MUCH BETTER!

### Biome extension
* Deprecated

## Migration Checklist

For modders updating to VEF 1.6:

1. **Update XML namespace references** from old namespaces to new VEF namespaces
2. **Check C# code** for any using statements referencing old namespaces
3. **Replace deprecated features**:
   - Replace ExcludeFromQuestsExtension with FactionDefExtension.excludeFromQuests
   - Replace ItemProcessor with PipeSystem
4. **Test compatibility** with the new VEF version
5. **Update Harmony patch names** if you have custom patches

## Performance Improvements

* Performance improvements on the more impactful patches
* All Harmony patches now use consistent naming convention
* Optimized internal code structure

---

*Last updated: June 22, 2025*
*Source: [Vanilla Expanded Framework Wiki](https://github.com/Vanilla-Expanded/VanillaExpandedFramework/wiki/16Changes)* 