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
1. Start placing foundations (reformation mode)
2. Toggle the drawing mode to enable the mod (default Ctrl+D)
3. Click to select starting corner point
4. Optionally, swap between shortest and longest longitudinal path (default Ctrl+P)
   - Longest mode is for drawing very wide rectangles, or even entire latitude bands
5. Click to select ending corner point

## Notes and Limitations
- Does not affect achievements or milestones
- Very large selection areas are supported but the game's rendering is not optimized for it, so low framerates may occur while making extremely large selections
  - Make multiple smaller selections instead if the framerate drops
- Soil piles are calculated using the standard game logic, but the numbers may appear slightly different from the default foundation tool applied to the same area because:
  - The calculation is path dependent, so the same end result can give different numbers depending on which order the tiles are updated
  - This tool gives perfectly straight edges, while the built in tool uses a circular radius, resulting in slightly different edges
- Special logic in the default foundation tool for drawing over Dark Fog bases is not yet implemented
- Burying/unburying ore veins is not yet implemented; switch back to the default tool for this

## Planned Features
- Border drawing modes (border only, border + fill, fill only)
- Border color selection
- Border thickness
- UI buttons
- More configuration
- Restore original brush size
- Handle Dark Fog special cases
- Burying/unburying veins
