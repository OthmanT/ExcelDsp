# Foundation Painter
Mod for Dyson Sphere Program that allows drawing foundations using rectangles.

## Installation
1. Get [r2modman](https://dsp.thunderstore.io/package/ebkr/r2modman/) from Thunderstore
2. Find the mod in the `Online` tab
3. Configure the mod (optional)
   1. Click `Start modded`
   2. Exit the game and return to `r2modman`
   3. Click `Config editor`
   4. Find the mod's configuration file and make changes
4. Click `Start modded`

## Usage
1. Start placing foundations
2. Enable drawing mode (default Ctrl+D)
3. Click to select starting corner point
4. Optionally, swap between shortest and longest path (default Ctrl+P)
5. Click to select ending corner point

## Notes
- Very large selection areas are supported, but the game's code is not designed for it so expect low framerates while making the selection.
- Soil piles are calculated using the standard game logic, but the numbers may appear slightly different from the default foundation tool applied to the same area because:
  - The calculation is path dependent, so the same end result can give different numbers depending on which order the tiles are updated
  - This tool gives perfectly straight edges, while the built in tool uses a circular radius, resulting in slightly different edges
- Special logic in the default foundation tool for drawing over Dark Fog bases is not currently implemented in this tool

## Planned Features
- Border drawing modes (border only, border + fill, fill only)
- Border color selection
- Border thickness
- UI buttons
- More configuration
