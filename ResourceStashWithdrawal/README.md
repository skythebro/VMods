# ![img.png](https://i.imgur.com/GD9JnrZ.png) Resource Stash Withdrawal - Gloomrot Update
forked from [WhiteFang5](https://github.com/WhiteFang5/VMods) and updated to Gloomrot.

# A server & client side mod that allows players to withdraw items for a recipe directly from their stash.
* This mod also adds the stash count of items to the tooltips (given that you're in/near your base)
* When a player is at a crafting or other workstation, they can click on the recipe or an individual component of a recipe with their middle-mouse button to withdraw the missing item(s) directly from their stash.
* (The withdraw the full amount, CTRL+Middle Mouse Button can be used)
---

## Installation
- Make sure you use [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/) if you're running a server via client!
- Place the **_VMods.ResourceStashWithdrawal.dll_** file inside of `(Vrising Server)\BepInEx\plugins` folder and your `(Vrising)\BepInEx\plugins` folder.
- If you're using <ins>**_ServerLaunchFix_**</ins> you only need to place the **_VMods.ResourceStashWithdrawal.dll_** file inside of `(Vrising)\BepInEx\plugins` folder.
- If you use any other VMod mods you can put them together in a folder called VMods for less clutter. `(Vrising Server)\BepInEx\plugins\VMods`
- **_Note:_** Players without the client-side mod can still join and play the server, but won't be able to make use of this feature.
---
## Configurable Values
```ini
[Server]

## Enabled/disable the resource stash withdrawal system.
# Setting type: Boolean
# Default value: true
ResourceStashWithdrawalEnabled = true
```

## Support me!
* I have a Patreon now so please support me [Here](https://patreon.com/user?u=97347013) so I can mod as much as I can!
* 10$ Patreons get early access to releases. (I need some way to test mods before I release them to the public :D)

## Developer & credits
<details>

### V rising modding discord [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord

### Original Creator & Developer
- [WhiteFang5](https://github.com/WhiteFang5/VMods)

</details>