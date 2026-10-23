# Gates Can Move

Adds opening and closing animations to fence gates that don't have one.

## Animating your own fence gate

Any block using the vanilla `BlockFenceGate` or `BlockFenceGateRoughHewn` class animates automatically when the shape of its closed variant has an animation with the code `opened`. No patch and no dependency on this mod are needed.

1. Open the closed gate shape in the model creator.
2. Add an animation called `opened` with a single keyframe at frame 0 that holds the gate in its open pose, like vanilla door shapes do. Set it to hold on end and rewind when stopped.
3. Optionally set `easingSpeed` in the block's attributes to change how fast it swings (default `10`, same as doors).

The game eases the animation in when the gate opens and out when it closes.

See [examples/examplegate](examples/examplegate) for a complete, working gate mod set up this way.

## Building

- `dotnet build` builds the mod.
- `dotnet build -c ReleaseToMods` also packs it as `GatesCanMove-v<version>.zip` straight into the game's `Mods` folder, the version comes from `resources/modinfo.json`.
- `dotnet build -c ReleaseToModsWithExample` does the same and also packs the [example gate](examples/examplegate) as `ExampleGate-v<version>.zip` next to it.

The game and data folders default to the Flatpak install; set `VINTAGE_STORY` and `VINTAGE_STORY_DATA` to use others.
