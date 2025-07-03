using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace RakazielPsycasts;

[DefOf]
public class RPDefOf
{
    [UsedImplicitly]
    public static HediffDef RP_DryadReincarnation;

    [UsedImplicitly]
    public static HediffDef RP_Floramancer;

    [UsedImplicitly]
    public static HediffDef RP_Liberation;

    [UsedImplicitly]
    public static HediffDef MissingBodyPart;

    [UsedImplicitly, MayRequireRoyalty]
    public static GeneDef VRE_AnimaAffinity;

    [UsedImplicitly, MayRequireIdeology]
    public static GeneDef VRE_GauranlenAffinity;

    [UsedImplicitly, MayRequireBiotech]
    public static GeneDef VRE_PoluxAffinity;

    [UsedImplicitly, MayRequireRoyalty]
    public static ThingDef Plant_TreeAnima;

    [UsedImplicitly, MayRequireIdeology]
    public static ThingDef Plant_TreeGauranlen;

    [UsedImplicitly, MayRequireBiotech]
    public static ThingDef Plant_TreePolux;

    [UsedImplicitly, MayRequireRoyalty]
    public static XenotypeDef VRE_Animakin;

    [UsedImplicitly, MayRequireIdeology]
    public static XenotypeDef VRE_Gauranlenkin;

    [UsedImplicitly, MayRequireBiotech]
    public static XenotypeDef VRE_Poluxkin;
}
