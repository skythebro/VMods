# ![img.png](https://i.imgur.com/GD9JnrZ.png) Resource Stash Withdrawal - Gloomrot Update
forked from [WhiteFang5](https://github.com/WhiteFang5/VMods) and updated to Gloomrot.
All credit goes to him for the original mod.
Credit for it working in the build menu goes to [diegosilva98](https://github.com/diegosilva98)

# A server & client side mod that allows players to withdraw items for a recipe directly from their stash.
* This mod also adds the stash count of items to the tooltips (given that you're in/near your base)
* When a player is at a crafting or other workstation, they can click on the recipe with their middle-mouse button to withdraw the missing item(s) directly from their stash.
* (To withdraw the full amount, CTRL+Middle Mouse Button can be used)
---

## Installation
- Make sure you use [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/) if you're running a server via client!
- Place the **_VMods.ResourceStashWithdrawal.dll_** file inside of `(Vrising Server)\BepInEx\plugins` folder and your `(Vrising)\BepInEx\plugins` folder.
- If you're using <ins>**_ServerLaunchFix_**</ins> you only need to place the **_VMods.ResourceStashWithdrawal.dll_** file inside of `(Vrising)\BepInEx\plugins` folder.
- If you use any other VMod mods you can put them together in a folder called VMods for less clutter. `(Vrising Server)\BepInEx\plugins\VMods`
- **_Note:_** Players without the client-side mod can still join and play on the server, but won't be able to make use of this feature.
---
## Configurable Values
```ini
[Server]

## Enabled/disable the resource stash withdrawal system.
# Setting type: Boolean
# Default value: true
ResourceStashWithdrawalEnabled = true
```

## Developer & credits
<details>

### V rising modding discord [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord
- - `diegosilva98` - helped with getting the mod to work in the build menu as well

### Original Creator & Developer
- [WhiteFang5](https://github.com/WhiteFang5/VMods)

</details>