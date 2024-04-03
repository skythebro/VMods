# VMods 
forked from [WhiteFang5](https://github.com/WhiteFang5/VMods) and updated to Gloomrot.


VMods is a selection of Mods for V-Rising made using a common/shared codebase that all follow the same coding principals and in-game usage.

## List of VMods
* [Blood Refill](#blood-refill)
* [Resource Stash Withdrawal](#resource-stash-withdrawal)

## General Mod info (Applies to most mods)
* Check out the other readme's in their respective folders for more info
### How to manually install
* Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
* Install [Bloodstone](https://v-rising.thunderstore.io/package/deca/Bloodstone/)
* (Locally hosted games only) Install [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/)
* Extract the Vmods._mod-name_.dll
* Move the desired mod(s) to the `[VRising (server) folder]/BepInEx/WetstonePlugins/`
* Launch the server (or game) to auto-generate the config files
* Edit the configs as you desire (found in `[VRising (server) folder]/BepInEx/config/`)

## Blood Refill
A server-side only mod that allows players to refill their blood pool.  
  
When feed-killing an enemy, you'll be able to regain some blood.  
The amount of blood regained is based on the level difference, blood type and blood quality of the killed enemy with V-Bloods refilling your blood pool for a much larger amount.

<details>
<summary>Configuration Options</summary>

* Enable/disable requiring feed-killing (when disabled, any kill grants some blood).
* Choose the amount of blood gained on a 'regular refill' (i.e. a refill without any level, blood type or quality punishments applied)
* A multiplier to reduce the amount of gained blood when feeding on an enemy of a different blood type. (blood dilution)
* The ability to disable different blood type refilling (i.e. a 0 multiplier for different blood types)
* Switch between having V-Blood act as diluted or pure blood, or have V-Blood completely refill your blood pool
* The options to make refilling random between 0.1L and the calculated amount (which then acts as a max refill amount)
* A global refill multiplier (applied after picking a random refill value)
* Enable/disable blood refill chat messages for everyone

</details>

## Resource Stash Withdrawal
A server & client side mod that allows players to withdraw items for a recipe directly from their stash.  
This mod also adds the stash count of items to the tooltips (given that you're in/near your base)  
  
When a player is at a crafting or other workstation, he/she can click on the recipe or an individual component of a recipe with their middle-mouse button to withdraw the missing item(s) directly from their stash.  
(The withdraw the full amount, CTRL+Middle Mouse Button can be used)  
  
Note: Players without the client-side mod can still join and play the server, but won't be able to make use of this feature.

## Developer & credits
<details>

### V rising modding discord [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord

### Original Creator & Developer
- [WhiteFang5](https://github.com/WhiteFang5/VMods)

</details>
