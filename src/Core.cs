using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace GatesCanMove;

public class Core : ModSystem
{
    private const string GateEntityClass = "GatesCanMoveGate";

    private const int ChunkSize = GlobalConstants.ChunkSize;

    private int[] gateBlockIds;

    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockEntityClass(GateEntityClass, typeof(BlockEntityGate));
    }

    /// <summary>
    /// Every fence gate whose closed shape has an "opened" animation gets the gate block entity.
    /// That is all a modded gate needs to animate as well, no patch targeting this mod is required.
    /// </summary>
    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks)
        {
            bool isGate = block is BlockFenceGate or BlockFenceGateRoughHewn;
            if (isGate && block.EntityClass == null && HasOpenedAnimation(api, block))
            {
                block.EntityClass = GateEntityClass;
            }
        }
    }

    private static bool HasOpenedAnimation(ICoreAPI api, Block gate)
    {
        Block closedGate = api.World.GetBlock(gate.CodeWithVariant("state", "closed"));
        if (closedGate?.Shape == null) return false;

        Shape shape = Shape.TryGet(api, closedGate.Shape.Base.Clone().WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json"));
        return shape?.Animations?.Any(animation => animation.Code == "opened") == true;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        gateBlockIds = api.World.Blocks.Where(block => block?.EntityClass == GateEntityClass).Select(block => block.BlockId).ToArray();
        api.Event.ChunkColumnLoaded += (chunkCoord, chunks) => AddMissingGateEntities(api, chunkCoord, chunks);
    }

    /// <summary>
    /// Gates placed before this mod was installed have no block entity yet, so give them one when their chunk loads.
    /// </summary>
    private void AddMissingGateEntities(ICoreServerAPI api, Vec2i chunkCoord, IWorldChunk[] chunks)
    {
        List<BlockPos> gates = FindGates(chunkCoord, chunks);
        if (gates.Count == 0) return;

        // This event fires before the chunks are added to the world map, so spawn on the next tick
        api.Event.RegisterCallback(_ =>
        {
            foreach (BlockPos pos in gates)
            {
                if (api.World.BlockAccessor.GetBlockEntity(pos) == null)
                {
                    api.World.BlockAccessor.SpawnBlockEntity(GateEntityClass, pos);
                }
            }
        }, 0);
    }

    private List<BlockPos> FindGates(Vec2i chunkCoord, IWorldChunk[] chunks)
    {
        var gates = new List<BlockPos>();

        for (int chunkY = 0; chunkY < chunks.Length; chunkY++)
        {
            IWorldChunk chunk = chunks[chunkY];
            chunk.Unpack_ReadOnly();

            // Cheap palette lookup, so chunks without gates are skipped without scanning them
            if (!gateBlockIds.Any(chunk.Data.ContainsBlock)) continue;

            for (int index3d = 0; index3d < ChunkSize * ChunkSize * ChunkSize; index3d++)
            {
                if (Array.IndexOf(gateBlockIds, chunk.Data[index3d]) < 0) continue;

                // Chunk blocks are stored as index3d = (y * size + z) * size + x
                int x = index3d % ChunkSize;
                int z = index3d / ChunkSize % ChunkSize;
                int y = index3d / (ChunkSize * ChunkSize);
                gates.Add(new BlockPos(chunkCoord.X * ChunkSize + x, chunkY * ChunkSize + y, chunkCoord.Y * ChunkSize + z));
            }
        }

        return gates;
    }
}
