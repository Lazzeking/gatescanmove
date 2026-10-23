# Example Gate

A reference content mod for adding Gates Can Move support to your own fence gate. It is a copy of the vanilla wooden fence gate, trimmed to oak without snow, under its own `examplegate` domain.

It has no code and does not depend on Gates Can Move. Without Gates Can Move it is a normal gate that snaps open and closed; with it, the gate swings.

## What makes it animate

- `blocktypes/examplegate.json` is an ordinary gate using the vanilla `BlockFenceGate` class with the usual `state` (`opened`/`closed`) variant. The optional `easingSpeed` attribute sets the swing speed (default `10`, same as vanilla doors).
- `shapes/block/examplegate/closed_left.json` and `closed_right.json` have an animation with the code `opened`: one keyframe at frame 0 rotating the gate element `W` into its open pose, set to hold on end and rewind when stopped. This is the same format vanilla door shapes use, and it can be edited in the model creator.
- `opened_left.json` and `opened_right.json` are unchanged vanilla shapes, used as usual when the gate rests open without Gates Can Move.

In `closed_right.json` the `rotationOrigin` of `W` was moved to the far post (`[14.5, 2, 8]`) so the gate swings around that hinge. Moving the origin of an element that has no rotation of its own does not change how it looks.

## Trying it

From the repository root, `dotnet build -c ReleaseToModsWithExample` packs it as `ExampleGate-v<version>.zip` into the game's `Mods` folder, next to Gates Can Move.

The gate is in the creative inventory as "Example Gate".
