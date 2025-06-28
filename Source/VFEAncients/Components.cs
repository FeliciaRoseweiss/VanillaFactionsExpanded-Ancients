using RimWorld;
using Verse;
using UnityEngine;

namespace VFEAncients
{
    public class CompProperties_SolarPowerUp : CompProperties
    {
        public float PowerPlantOutputMult = 1f;

        public CompProperties_SolarPowerUp()
        {
            compClass = typeof(CompSolarPowerUp);
        }
    }

    public class CompSolarPowerUp : ThingComp
    {
        public CompProperties_SolarPowerUp Props => (CompProperties_SolarPowerUp)props;
        public float PowerMultiplier => Props.PowerPlantOutputMult;
    }

    public class CompProperties_Quest : CompProperties
    {
        public string Quest;
        public GraphicData hackedGraphic;

        public CompProperties_Quest()
        {
            compClass = typeof(CompQuestOnHacked);
        }
    }

    public class CompQuestOnHacked : ThingComp
    {
        public CompProperties_Quest Props => (CompProperties_Quest)props;
    }

    public class CompDisappearOnHacked : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }

    public class CompProperties_SupplyCrate : CompProperties
    {
        public float raidChance = 0.5f;
        public int expiryTicks = 120000;

        public CompProperties_SupplyCrate()
        {
            compClass = typeof(CompSupplyCrate);
        }
    }

    public class CompSupplyCrate : ThingComp
    {
        public CompProperties_SupplyCrate Props => (CompProperties_SupplyCrate)props;
    }

    public class CompSupplySlingshot : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }

    public class CompAncientSolar : CompPowerPlant
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }

    public class CompPowerPlantSteam : CompPowerPlant
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }

    public class CompBiobattery : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }

    public class CompGeneTailoringPod : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }

    public class CompProperties_Facility_PowerUnlock : CompProperties_Facility
    {
        public int unlockedLevels = 1;
        public int maxSimultaneous = 1;
        public float maxDistance = 16f;

        public CompProperties_Facility_PowerUnlock()
        {
            compClass = typeof(CompFacility_PowerUnlock);
        }
    }

    public class CompFacility_PowerUnlock : CompFacility
    {
        public CompProperties_Facility_PowerUnlock Props => (CompProperties_Facility_PowerUnlock)props;
    }

    public class CompProperties_NeedsContainment : CompProperties
    {
        public List<string> validContainers = new List<string>();

        public CompProperties_NeedsContainment()
        {
            compClass = typeof(CompNeedsContainment);
        }
    }

    public class CompNeedsContainment : ThingComp
    {
        public CompProperties_NeedsContainment Props => (CompProperties_NeedsContainment)props;
    }
}
