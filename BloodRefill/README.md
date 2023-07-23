# ![img.webp](https://i.imgur.com/w9Cl5rk.png) Vmods Blood Refill - Gloomrot Update
forked from [WhiteFang5](https://github.com/WhiteFang5/VMods) and updated to Gloomrot.

# A server-side only mod that allows players to refill their blood pool.
* When feed-killing an enemy, you'll be able to regain some blood.
* The amount of blood regained is based on the level difference, blood type and blood quality of the killed enemy with V-Bloods refilling your blood pool for a much larger amount.
* Specific blood types can also be disabled so no blood can be regained from them.
* Theres also a setting that makes it so you cannot regain blood if you bloodquality is above a certain percentage.
* Lots of changes can be made in the config as you can see [<ins>**_Configurable Values_**<ins/>](#Configurable-Values)

---

## Installation
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
BloodRefillMultiplier = 1

## The multiplier used in the blood refill calculation as a penalty for feeding on a different blood type (only works when BloodRefillRequiresSameBloodType is disabled).
# Setting type: Single
# Default value: 0.1
BloodRefillDifferentBloodTypeMultiplier = 0.1

## When enabled, a refill chat message is sent to the player.
# Setting type: Boolean
# Default value: true
BloodRefillSendRefillMessage = true

[CommandSystemConfig]

## Leave disabled for now because It interferes with community commands. Enabled/disable the Commands system (for this specific mod).
# Setting type: Boolean
# Default value: false
CommandSystemEnabled = false

## The prefix that needs to be used to execute a command (for this specific mod).
# Setting type: String
# Default value: !
CommandSystemPrefix = !

## The amount of seconds between two commands (for non-admins).
# Setting type: Single
# Default value: 5
CommandSystemCommandCooldown = 5
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
