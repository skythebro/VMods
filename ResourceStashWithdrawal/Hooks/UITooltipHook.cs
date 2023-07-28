using HarmonyLib;
using ProjectM;
using ProjectM.Scripting;
using ProjectM.Shared;
using ProjectM.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using VMods.Shared;
using Bloodstone.API;
using ProjectM.Network;
using Unity.Collections;

namespace VMods.ResourceStashWithdrawal
{
	[HarmonyPatch]
	public class UITooltipHook
	{
		#region Public Methods

		[HarmonyPatch(typeof(GridSelectionEntry), nameof(GridSelectionEntry.OnPointerEnter))]
		[HarmonyPostfix]
		public static void OnPointerEnter(GridSelectionEntry __instance, PointerEventData eventData)
		{
			if(!VWorld.IsClient)
			{
				return;
			}

			var hasRefinementstationRecipeEntry = __instance.TryGetComponent<RefinementstationRecipeEntry>(out var refinementstationRecipeEntry);
			var hasRefinementstationRecipeItem = __instance.TryGetComponent<RefinementstationRecipeItem>(out var refinementstationRecipeItem);
			var hasItemGridSelectionEntry = __instance.TryGetComponent<ItemGridSelectionEntry>(out var itemGridSelectionEntry);
			var hasWorkstationRecipeGridSelectionEntry = __instance.TryGetComponent<WorkstationRecipeGridSelectionEntry>(out var workstationRecipeGridSelectionEntry);
			var hasResearchEntry = __instance.TryGetComponent<ResearchEntry>(out var researchEntry);
			var hasBuildMenuStructureEntry = __instance.TryGetComponent<BuildMenu_StructureEntry>(out var buildMenuStructureEntry);
			if(!hasRefinementstationRecipeEntry && !hasRefinementstationRecipeItem && !hasItemGridSelectionEntry &&
			   !hasWorkstationRecipeGridSelectionEntry && !hasResearchEntry && !hasBuildMenuStructureEntry)
			{
#if DEBUG
				Utils.Logger.LogMessage($"Unknown/unhandled {nameof(GridSelectionEntry)} PointerEnter for Type: {__instance.GetScriptClassName()}");
#endif
				return;
			}

			// Find the current tooltip
			var refinementstationSubMenu = __instance.GetComponentInParent<RefinementstationSubMenu>();
			var unitSpawnerstationSubMenu = __instance.GetComponentInParent<UnitSpawnerstationSubMenu>();
			var workstationSubMenu = __instance.GetComponentInParent<WorkstationSubMenu>();
			var researchstationSubMenu = __instance.GetComponentInParent<ResearchstationSubMenu>();
			var inventorySubMenu = __instance.GetComponentInParent<InventorySubMenu>();
			var buildMenu = __instance.GetComponentInParent<BuildMenu>();
			var servantInventorySubMenu = __instance.GetComponentInParent<ServantInventorySubMenu>();
			var salvagestationSubMenu = __instance.GetComponentInParent<SalvagestationSubMenu>();
			FakeTooltip tooltip = null;
			if(refinementstationSubMenu != null)
			{
				tooltip = refinementstationSubMenu.FakeTooltip;
			}
			else if(unitSpawnerstationSubMenu != null)
			{
				tooltip = unitSpawnerstationSubMenu.FakeTooltip;
			}
			else if(workstationSubMenu != null)
			{
				tooltip = workstationSubMenu.FakeTooltip;
			}
			else if(researchstationSubMenu != null)
			{
				tooltip = researchstationSubMenu.FakeTooltip;
			}
			else if(inventorySubMenu != null)
			{
				tooltip = inventorySubMenu.FakeTooltip;
			}
			else if(buildMenu != null)
			{
				tooltip = buildMenu.FakeTooltip;
			}
			else if(servantInventorySubMenu != null)
			{
				tooltip = servantInventorySubMenu.FakeTooltip;
			}
			else if(salvagestationSubMenu != null)
			{
				tooltip = salvagestationSubMenu.FakeTooltip;
			}

			if(tooltip == null)
			{
#if DEBUG
				Utils.Logger.LogMessage($"Unknown/unhandled Tooltip for Type: {__instance.GetScriptClassName()}");
#endif
				return;
			}

			var client = VWorld.Client;
			if(client.Systems.Count == 0)
			{
				// No systems -> No tooltip
				return;
			}
			var entityManager = client.EntityManager;
			var gameDataSystem = client.GetExistingSystem<GameDataSystem>();
			var clientGameManager = client.GetExistingSystem<ClientScriptMapper>()?._ClientGameManager;
			Entity userCharEntity = Entity.Null;
			foreach (var UsersEntity in entityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp))
			{
				entityManager.TryGetComponentData<User>(UsersEntity, out var userComponent);
				userCharEntity = userComponent.LocalCharacter._Entity;
			}

			var character = userCharEntity;

			// Ensure we're hovering a single item or the recipe as a whole
			PrefabGUID? itemGUID = null;
			StoredBlood? storedBlood = null;
			List<PrefabGUID> requiredItemGUIDs = null;
			List<PrefabGUID> repairItemGUIDs = null;
			if(refinementstationRecipeEntry != null)
			{
				RefinementstationRecipeEntry.Data recipe;
				if(refinementstationSubMenu != null)
				{
					recipe = refinementstationSubMenu.RecipesSelectionGroup.Entries._items[refinementstationRecipeEntry.EntryIndex];
				}
				else if(unitSpawnerstationSubMenu != null)
				{
					recipe = unitSpawnerstationSubMenu.RecipesSelectionGroup.Entries._items[refinementstationRecipeEntry.EntryIndex];
				}
				else
				{
#if DEBUG
					Utils.Logger.LogMessage($"Unknown/unhandled itemGUID SubMenu for Type: {__instance.GetScriptClassName()}");
#endif
					return;
				}
				requiredItemGUIDs = new();
				foreach(var requiredItem in recipe.Requirements)
				{
					requiredItemGUIDs.Add(requiredItem.Guid);
				}
				itemGUID = recipe.OutputItems[0].Guid;
			}
			else if(refinementstationRecipeItem != null)
			{
				itemGUID = refinementstationRecipeItem.Guid;
			}
			else if(itemGridSelectionEntry != null)
			{
				if(itemGridSelectionEntry.EntryId == PrefabGUID.Empty)
				{
					// It's an empty slot.
					return;
				}
				itemGUID = itemGridSelectionEntry.EntryId;

				if(inventorySubMenu != null && itemGridSelectionEntry.SyncedEntity != Entity.Null)
				{
					var hasDurability = entityManager.TryGetComponentData<Durability>(itemGridSelectionEntry.SyncedEntity, out var durability);
					if(hasDurability)
					{
						var prefabCollectionSystem = client.GetExistingSystem<PrefabCollectionSystem>();
						var prefabLookupMap = prefabCollectionSystem.PrefabLookupMap;

						repairItemGUIDs = new();
						var repairCosts = durability.GetRepairCost(prefabLookupMap, entityManager);
						foreach(var repairCost in repairCosts)
						{
							repairItemGUIDs.Add(repairCost.Guid);
						}
					}
					var hasStoredBlood = entityManager.TryGetComponentData<StoredBlood>(itemGridSelectionEntry.SyncedEntity, out var blood);
					if(hasStoredBlood)
					{
						storedBlood = blood;
					}
				}
			}
			else if(workstationRecipeGridSelectionEntry != null)
			{
				WorkstationRecipeGridSelectionEntry.Data recipe;
				if(workstationSubMenu != null)
				{
					recipe = workstationSubMenu.RecipesGridSelectionGroup.Entries._items[workstationRecipeGridSelectionEntry.EntryIndex];
				}
				else if(inventorySubMenu != null)
				{
					recipe = inventorySubMenu.RecipesGridSelectionGroup.Entries._items[workstationRecipeGridSelectionEntry.EntryIndex];
				}
				else
				{
#if DEBUG
					Utils.Logger.LogMessage($"Unknown/unhandled itemGUID SubMenu for Type: {__instance.GetScriptClassName()}");
#endif
					return;
				}
				if(gameDataSystem.RecipeHashLookupMap.ContainsKey(recipe.EntryId))
				{
					var recipeData = gameDataSystem.RecipeHashLookupMap[recipe.EntryId];
					var hasRecipeRequirementBuffer = entityManager.TryGetBuffer<RecipeRequirementBuffer>(recipeData.Entity, out var requirements);
					if(hasRecipeRequirementBuffer)
					{
						requiredItemGUIDs = new();
						foreach(var requirement in requirements)
						{
							requiredItemGUIDs.Add(requirement.Guid);
						}
					}
					
					var hasRecipeOutputBuffer = entityManager.TryGetBuffer<RecipeOutputBuffer>(recipeData.Entity, out var outputs);
					
					if(hasRecipeOutputBuffer)
					{
						itemGUID = outputs[0].Guid;
					}
				}
			}
			else if(researchEntry != null)
			{
				foreach(var category in researchstationSubMenu.ResearchCategories)
				{
					if(category.ResearchGridSelectionGroup.Entries._items.Count > researchEntry.EntryIndex &&
						category.ResearchGridSelectionGroup.Entries._items[researchEntry.EntryIndex].EntryId == researchEntry.EntryId)
					{
						var recipe = category.ResearchGridSelectionGroup.Entries._items[researchEntry.EntryIndex];
						requiredItemGUIDs = new();
						foreach(var requiredItem in recipe.Requirements)
						{
							if(Utils.TryGetPrefabGUIDForItemName(gameDataSystem, requiredItem.ItemName, out var requiredItemGUID))
							{
								requiredItemGUIDs.Add(requiredItemGUID);
							}
						}
						break;
					}
				}
			}
			else if(buildMenuStructureEntry != null)
			{
				
				if(gameDataSystem.BlueprintHashLookupMap.ContainsKey(buildMenuStructureEntry.PrefabGuid))
				{
					var blueprintData = gameDataSystem.BlueprintHashLookupMap[buildMenuStructureEntry.PrefabGuid];
					var hasBlueprintRequirementBuffer = entityManager.TryGetBuffer<BlueprintRequirementBuffer>(blueprintData.Entity, out var requirements);
					
					if(hasBlueprintRequirementBuffer)
					{
						requiredItemGUIDs = new();
						foreach(var requirement in requirements)
						{
							requiredItemGUIDs.Add(requirement.PrefabGUID);
						}
					}
				}
			}
			else
			{
#if DEBUG
				Utils.Logger.LogMessage($"Unknown/unhandled ItemGUID Retrieval for Type: {__instance.GetScriptClassName()}");
#endif
				return;
			}

			int? stashCount = itemGUID == null ? null : Utils.GetStashItemCount(entityManager, character, itemGUID.Value, storedBlood);

			var requiredItemStashCount = requiredItemGUIDs?.Select(x => Utils.GetStashItemCount(entityManager, character, x)).ToList();
			var repairItemStashCount = repairItemGUIDs?.Select(x => Utils.GetStashItemCount(entityManager, character, x)).ToList();

			if((stashCount.HasValue && stashCount.Value == -1) ||
				(requiredItemStashCount != null && requiredItemStashCount.Contains(-1)) ||
				(repairItemStashCount != null && repairItemStashCount.Contains(-1)))
			{
				// Don't add stash values outside of the castle, we can't count them anyway!
				return;
			}

			var id = tooltip.GetInstanceID();
			bool shouldStartRoutine = _tooltipInfo.TryGetValue(id, out var record);
			_tooltipInfo[id] = (DateTime.UtcNow, stashCount, requiredItemStashCount, repairItemStashCount);
			if(shouldStartRoutine || DateTime.UtcNow.Subtract(record.creationTime).TotalSeconds >= 0.2f)
			{
				__instance.StartCoroutine(UpdateTooltip(tooltip));
			}
		}

		private static readonly Dictionary<int, (DateTime creationTime, int? stashCount, List<int> requiredItemStashCount, List<int> repairItemStashCount)> _tooltipInfo = new();

		private static IEnumerator UpdateTooltip(FakeTooltip tooltip)
		{
			var id = tooltip.GetInstanceID();
			var lastUpdateTime = DateTime.MinValue;
			int? stashCount;
			List<int> requiredItemStashCount;
			List<int> repairItemStashCount;

			while(_tooltipInfo.TryGetValue(id, out var record) && (lastUpdateTime != record.creationTime || (!AllTextsContainStashInfo() && DateTime.UtcNow.Subtract(lastUpdateTime).TotalSeconds < 10f)))
			{
				yield return null;

				if(tooltip == null || tooltip.SubText == null || tooltip.SubText.Text == null || !_tooltipInfo.ContainsKey(id))
				{
					break;
				}

				(lastUpdateTime, stashCount, requiredItemStashCount, repairItemStashCount) = _tooltipInfo[id];

				if(stashCount.HasValue)
				{
					tooltip.SubText.Text.SetText($"{tooltip.SubText._LocalizedString.Text} <size=14><color=white>(Stash: <color=yellow>{stashCount.Value}</color>)</color></size>");
				}

				if(requiredItemStashCount != null)
				{
					for(int i = 0; i < requiredItemStashCount.Count; i++)
					{
						var requiredItem = tooltip.RecipeRequiredItems.RequiredItemsList._items[i];
						requiredItem.Name.Text.SetText($"{requiredItem.Name._LocalizedString.Text} <size=12><color=white>(Stash: <color=yellow>{requiredItemStashCount[i]}</color>)</color></size>");
					}
				}

				if(repairItemStashCount != null)
				{
					for(int i = 0; i < repairItemStashCount.Count; i++)
					{
						var requiredItem = tooltip.ItemRepairCost.RepairCostList._items[i];
						requiredItem.Name.Text.SetText($"{requiredItem.Name._LocalizedString.Text} <size=12><color=white>(Stash: <color=yellow>{repairItemStashCount[i]}</color>)</color></size>");
					}
				}

				var endTime = Time.realtimeSinceStartup + 0.2f;
				while(endTime > Time.realtimeSinceStartup && _tooltipInfo.TryGetValue(id, out record) && lastUpdateTime == record.creationTime && AllTextsContainStashInfo())
				{
					yield return null;
				}
			}

			_tooltipInfo.Remove(id);

			// Nested Method(s)
			bool AllTextsContainStashInfo()
			{
				if(tooltip == null || tooltip.SubText == null || tooltip.SubText.Text == null || !tooltip.isActiveAndEnabled)
				{
					return true;
				}

				string endPhrase = "</size>";

				if(!tooltip.SubText.Text.text.EndsWith(endPhrase))
				{
					return false;
				}

				// Seems to cause some kind of infinite loop???
				/*foreach(var requiredItem in tooltip.RequiredItemsList)
				{
					if(!requiredItem.isActiveAndEnabled)
					{
						continue;
					}
					if(!requiredItem.Name.Text.text.EndsWith(endPhrase))
					{
						return false;
					}
				}
				
				foreach(var repairItem in tooltip.RepairItemsList)
				{
					if(!repairItem.isActiveAndEnabled)
					{
						continue;
					}
					if(!repairItem.Name.Text.text.EndsWith(endPhrase))
					{
						return false;
					}
				}
				*/

				return true;
			}
		}

		#endregion
	}
}
