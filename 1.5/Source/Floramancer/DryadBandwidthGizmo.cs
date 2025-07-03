using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RakazielPsycasts.Floramancer;

[StaticConstructorOnStartup]
public sealed class DryadBandwidthGizmo : Gizmo
{
    public const int InRectPadding = 6;

    private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private static readonly Color FilledBlockColor = Color.magenta;

    private static readonly Color ExcessBlockColor = ColorLibrary.Red;

    private Hediff_Floramancer hediff;

    public override bool Visible => Find.Selector.SelectedPawns.Count == 1;

    public DryadBandwidthGizmo(Hediff_Floramancer hediff)
    {
        this.hediff = hediff;
        Order = -90f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        Rect outerRect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        Rect innerRect = outerRect.ContractedBy(InRectPadding);
        Widgets.DrawWindowBackground(outerRect);

        int totalBandwidth = hediff.bandwidth;
        int usedBandwidth = hediff.dryads.Count;
        string bandwidthText = $"{usedBandwidth} / {totalBandwidth}";

        TaggedString tooltipText = "RP_Dryads".Translate().Colorize(ColoredText.TipSectionTitleColor) + $": {bandwidthText}\n\n" +
                                   "RP_DryadsGizmoTip".Translate(hediff.ComaDuration.ToStringTicksToPeriod(), totalBandwidth);

        if (usedBandwidth > 0)
        {
            IEnumerable<string> dryadLabels = hediff.dryads.Select(dryad => dryad.LabelCap);
            tooltipText += "\n\n" + "RP_DryadUsage".Translate() + "\n" + dryadLabels.ToLineList(" - ");
        }

        TooltipHandler.TipRegion(outerRect, tooltipText);

        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Rect labelRect = new(innerRect.x, innerRect.y, innerRect.width, 20f);
        Widgets.Label(labelRect, "RP_Dryads".Translate());

        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(labelRect, bandwidthText);
        Text.Anchor = TextAnchor.UpperLeft;

        int maxBlocks = Mathf.Max(usedBandwidth, totalBandwidth);
        Rect blocksArea = new(innerRect.x, labelRect.yMax + InRectPadding, innerRect.width, innerRect.height - labelRect.height - InRectPadding);

        int rows = 2;
        int blockSize = Mathf.FloorToInt(blocksArea.height / rows);
        int columns = Mathf.FloorToInt(blocksArea.width / blockSize);
        int iterationCount = 0;

        while (rows * columns < maxBlocks)
        {
            rows++;
            blockSize = Mathf.FloorToInt(blocksArea.height / rows);
            columns = Mathf.FloorToInt(blocksArea.width / blockSize);
            iterationCount++;

            if (iterationCount < 1000)
            {
                continue;
            }

            Log.Error("Failed to fit dryad bandwidth gizmo in the rect.");
            return new GizmoResult(GizmoState.Clear);
        }

        float horizontalPadding = (blocksArea.width - columns * blockSize) / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int blockIndex = row * columns + column + 1;
                if (blockIndex > maxBlocks) continue;

                Rect blockRect = new Rect(blocksArea.x + column * blockSize + horizontalPadding, blocksArea.y + row * blockSize, blockSize, blockSize).ContractedBy(2f);
                Color blockColor = blockIndex <= usedBandwidth
                    ? (blockIndex <= totalBandwidth ? FilledBlockColor : ExcessBlockColor)
                    : EmptyBlockColor;

                Widgets.DrawRectFast(blockRect, blockColor);
            }
        }

        return new GizmoResult(GizmoState.Clear);
    }

    public override float GetWidth(float maxWidth)
    {
        return 136f;
    }
}
