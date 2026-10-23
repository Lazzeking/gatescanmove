using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace GatesCanMove;

/// <summary>
/// Plays the "opened" animation of the closed gate shape, the same way vanilla doors do.
/// </summary>
public class BlockEntityGate : BlockEntity
{
    private BlockEntityAnimationUtil animUtil;

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);
        if (api.Side != EnumAppSide.Client) return;

        Block closed = api.World.GetBlock(Block.CodeWithVariant("state", "closed"));
        Shape shape = Shape.TryGet(api, closed.Shape.Base.Clone().WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json"));

        animUtil = new BlockEntityAnimationUtil(api, this);
        animUtil.InitializeAnimator("gatescanmove-" + closed.Code, shape, null, new Vec3f(0, closed.Shape.rotateY, 0));
        ToggleAnimation();
    }

    // Vanilla gates swap between the opened and closed block variants, this block entity survives that swap
    public override void OnExchanged(Block block)
    {
        base.OnExchanged(block);
        if (animUtil != null) ToggleAnimation();
    }

    private void ToggleAnimation()
    {
        if (Block.Variant["state"] != "opened")
        {
            animUtil.StopAnimation("opened");
            return;
        }

        float easingSpeed = Block.Attributes?["easingSpeed"].AsFloat(10f) ?? 10f;
        animUtil.StartAnimation(new AnimationMetaData { Animation = "opened", Code = "opened", EaseInSpeed = easingSpeed, EaseOutSpeed = easingSpeed });
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
    {
        return animUtil.activeAnimationsByAnimCode.Count > 0 || animUtil.animator.ActiveAnimationCount > 0;
    }

    public override void OnBlockRemoved()
    {
        base.OnBlockRemoved();
        animUtil?.Dispose();
    }

    public override void OnBlockUnloaded()
    {
        base.OnBlockUnloaded();
        animUtil?.Dispose();
    }
}
