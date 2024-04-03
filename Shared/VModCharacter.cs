using Bloodstone.API;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace VMods.Shared
{
	public readonly struct VModCharacter
	{
		#region Variables

		public readonly User User;
		public readonly PlayerCharacter Character;
		public readonly FromCharacter FromCharacter;

		#endregion

		#region Properties

		public ulong PlatformId => User.PlatformId;

		public bool IsAdmin => User.IsAdmin;

		public AdminLevel AdminLevel { get; }

		public string CharacterName => User.CharacterName.ToString();

		#endregion

		#region Lifecycle

		public VModCharacter(User user, PlayerCharacter character, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			User = user;
			Character = character;
			FromCharacter = new FromCharacter()
			{
				User = character.UserEntity,
				Character = user.LocalCharacter._Entity,
			};
			AdminLevel = Utils.GetAdminLevel(FromCharacter.User, entityManager);
		}

		public VModCharacter(FromCharacter fromCharacter, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			entityManager.Value.TryGetComponentData<User>(fromCharacter.User, out var user);
			User = user;
			entityManager.Value.TryGetComponentData<PlayerCharacter>(fromCharacter.Character, out var character);
			Character = character;
			FromCharacter = fromCharacter;
			AdminLevel = Utils.GetAdminLevel(FromCharacter.User, entityManager);
		}

		public VModCharacter(Entity userEntity, Entity charEntity, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			entityManager.Value.TryGetComponentData<User>(userEntity, out var user);
			User = user;
			entityManager.Value.TryGetComponentData<PlayerCharacter>(charEntity, out var character);
			Character = character;
			FromCharacter = new FromCharacter()
			{
				User = Character.UserEntity,
				Character = User.LocalCharacter._Entity,
			};
			AdminLevel = Utils.GetAdminLevel(FromCharacter.User, entityManager);
		}

		public VModCharacter(Entity charEntity, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			entityManager.Value.TryGetComponentData<PlayerCharacter>(charEntity, out var character);
			Character = character;
			FromCharacter = new FromCharacter()
			{
				User = Character.UserEntity,
				Character = charEntity,
			};
			entityManager.Value.TryGetComponentData<User>(FromCharacter.User, out var user);
			User = user;
			AdminLevel = Utils.GetAdminLevel(FromCharacter.User, entityManager);
		}

		#endregion

		#region Public Methods

		public static bool operator ==(VModCharacter left, VModCharacter right)
		{
			if(Equals(left, right))
			{
				return true;
			}
			return left.User == right.User;
		}

		public static bool operator !=(VModCharacter left, VModCharacter right) => !(left == right);

		public static VModCharacter? GetVModCharacter(string charactername, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			var users = entityManager.Value.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
			foreach(var userEntity in users)
			{
				entityManager.Value.TryGetComponentData<User>(userEntity, out var userData);
				if(userData.CharacterName.ToString() != charactername)
				{
					continue;
				}

				var characterEntity = userData.LocalCharacter._Entity;
				var hasPlayerCharacter = entityManager.Value.TryGetComponentData<PlayerCharacter>(characterEntity, out var playerCharacter);
				if(!hasPlayerCharacter)
				{
					continue;
				}
				
				return new VModCharacter(userData, playerCharacter);
			}
			return null;
		}

		public override bool Equals(object obj)
		{
			if(base.Equals(obj))
			{
				return true;
			}
			if(obj is VModCharacter vmodCharacter)
			{
				return this == vmodCharacter;
			}
			return false;
		}

		public override int GetHashCode() => (User, Character).GetHashCode();

		public void ApplyBuff(PrefabGUID buffGUID)
		{
			Utils.ApplyBuff(FromCharacter, buffGUID);
		}

		public void RemoveBuff(PrefabGUID buffGUID)
		{
			Utils.RemoveBuff(FromCharacter.Character, buffGUID);
		}

		public bool HasBuff(PrefabGUID buffGUID, EntityManager? entityManager = null)
		{
			entityManager ??= Utils.CurrentWorld.EntityManager;
			return BuffUtility.HasBuff(entityManager.Value, FromCharacter.Character, buffGUID);
		}

		public void SendSystemMessage(string message)
		{
			User.SendSystemMessage(message);
		}

		#endregion
	}
}
