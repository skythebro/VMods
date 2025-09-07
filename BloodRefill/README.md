# ![img.webp](https://i.imgur.com/w9Cl5rk.png) Vmods Blood Refill - 1.1 update
forked from [WhiteFang5](https://github.com/WhiteFang5/VMods) and updated to Gloomrot, 1.0 and now 1.1.
All credit goes to him for the original mod.

# A server-side only mod that allows players to refill their blood pool.
* When feed-killing an enemy, you'll be able to regain some blood.
* The amount of blood regained is based on the level difference, blood type and blood quality of the killed enemy with V-Bloods refilling your blood pool for a much larger amount.
* Specific blood types can also be disabled so no blood can be regained from them.
* There's also a setting that makes it so you cannot regain blood if you bloodquality is above a certain percentage.
* Lots of changes can be made in the config as you can see in the section <ins>**_Configurable Values_**<ins/>
* Be sure to configure the mod to your liking!

### New in 1.1
* Added support for the new blood type added in 1.1 (Corruption)
* ~~Added a new config option to enable/disable secondary blood buffs being kept when refilling blood.~~ WIP currently only primary blood litre amount can be changed 😭 (if you have a solution please let me know)

---

## Installation
- This mod requires [VAMP](https://thunderstore.io/c/v-rising/p/skytech6/VAMP)
- Make sure you use [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/) if you're running a server via client!
- Place the **_VMods.BloodRefill.dll_** file inside of `(Vrising Server)\BepInEx\plugins` folder
- If you use any other VMod mods you can put them together in a folder called VMods for less clutter. `(Vrising Server)\BepInEx\plugins\VMods`

---

## Configurable Values
```ini

[BloodRefillConfig]

## Enabled/disable the blood refilling system.
# Setting type: Boolean
# Default value: true
BloodRefillEnabled = true

## When enabled, secondary blood buffs are kept when refilling blood.
# Setting type: Boolean
# Default value: true
BloodRefillKeepSecondaryBloodBuffs = true

## When enabled, blood can only be refilled when feeding (i.e. when aborting the feed).
# Setting type: Boolean
# Default value: true
BloodRefillRequiresFeeding = true

## When enabled, blood can only be refilled when the target has the same blood type.
# Setting type: Boolean
# Default value: true
BloodRefillRequiresSameBloodType = true

## When enabled, creature blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillCreatureEnabled = true

## When enabled, warrior blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillWarriorEnabled = true

## When enabled, rogue blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillRogueEnabled = true

## When enabled, brute blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillBruteEnabled = true

## When enabled, scholar blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillScholarEnabled = true

## When enabled, worker blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillWorkerEnabled = true

## When enabled, mutant blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillMutantEnabled = true

## When enabled, V-blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillVBloodEnabled = true

## When enabled, Draculin blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillDraculinEnabled = true

## When enabled, GateBoss blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillGateBossEnabled = true

## When enabled, DraculaTheImmortal blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillDraculaTheImmortalEnabled = true

## When enabled, Corruption blood can be refilled.
# Setting type: Boolean
# Default value: true
BloodRefillCorruptionEnabled = true

## When enabled, V-blood is excluded from the 'same blood type' check (i.e. it's always considered to be 'the same blood type' as the player's blood type).
# Setting type: Boolean
# Default value: true
BloodRefillExcludeVBloodFromSameBloodTypeCheck = true

## 0 = disabled (i.e. normal refill); 1 = fully refill; 2 = refill based on V-blood monster level; 3 = refill based on V-blood monster level but not if the player is above BloodRefillVBloodCutoffThreshold % of blood quality. only works when BloodRefillVBloodEnabled is enabled.
# Setting type: Int32
# Default value: 3
BloodRefillVBloodRefillType = 3

## 0 = disabled (i.e. normal refill); 1 = normal refill but not if the player is above BloodRefillBloodCutoffThreshold % of blood quality.
# Setting type: Int32
# Default value: 1
BloodRefillBloodRefillType = 1

## [Only applies when BloodRefillBloodRefillType is set to 1] The blood quality percentage cutoff threshold (i.e. if the player's blood quality is above this threshold, npc blood will not refill the player's blood).
# Setting type: Single
# Default value: 95
BloodRefillBloodCutoffThreshold = 95

## [Only applies when BloodRefillVBloodRefillType is set to 3] The blood quality percentage cutoff threshold (i.e. if the player's blood quality is above this threshold, V-blood will not refill the player's blood).
# Setting type: Single
# Default value: 95
BloodRefillVBloodCutoffThreshold = 95

## [Only applies when BloodRefillVBloodRefillType is set to 2 or 3] The multiplier used in the V-blood refill calculation ('EnemyLevel' * 'BloodRefillVBloodRefillMultiplier' * 'BloodRefillMultiplier').
# Setting type: Single
# Default value: 0.1
BloodRefillVBloodRefillMultiplier = 0.1

## When enabled, the amount of refilled blood is randomized (between 1 and the calculated refillable amount).
# Setting type: Boolean
# Default value: true
BloodRefillRandomRefill = true

## The maximum amount of blood to refill with no level difference, a matching blood type and quality (Expressed in Litres of blood).
# Setting type: Single
# Default value: 1
BloodRefillAmount = 1

## The multiplier used in the blood refill calculation. [Formula: (('Enemy Level' / 'Player Level') * ((100 - ('Player Blood Quality %' - 'Enemy Blood Quality %')) / 100)) * 'BloodRefillAmount' * '(If applicable) BloodRefillDifferentBloodTypeMultiplier' * 'BloodRefillMultiplier']
# Setting type: Single
# Default value: 0.5
BloodRefillMultiplier = 0.5

## The multiplier used in the blood refill calculation as a penalty for feeding on a different blood type (only works when BloodRefillRequiresSameBloodType is disabled).
# Setting type: Single
# Default value: 0.1
BloodRefillDifferentBloodTypeMultiplier = 0.1

## When enabled, a refill chat message is sent to the player.
# Setting type: Boolean
# Default value: false
BloodRefillSendRefillMessage = false
```

## Any issues please report them on the [issues page](

## Developer & credits
<details>

### V rising modding [discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord

### Original Creator & Developer
- [WhiteFang5](https://github.com/WhiteFang5/VMods)

</details>
