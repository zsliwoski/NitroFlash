using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"int\", \"string\", \"uint\"][\"string\", \"uint\"][\"string\"][][][\"Vector3\", \"Quaternion\", \"string\"][\"string\"][][]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"damage\", \"instigator\", \"instigatorID\"][\"instigator\", \"instigatorID\"][\"killed\"][][][\"position\", \"rotation\", \"instigator\"][\"killed\"][][]]")]
	public abstract partial class NetworkPlayerBehavior : NetworkBehavior
	{
		public const byte RPC_SERVER__TAKE_DAMAGE = 0 + 5;
		public const byte RPC_SERVER__DEATH = 1 + 5;
		public const byte RPC_SERVER__GET_KILL = 2 + 5;
		public const byte RPC_SERVER__RESPAWN = 3 + 5;
		public const byte RPC_MULTICAST__GUN_FIRED = 4 + 5;
		public const byte RPC_MULTICAST__DEATH = 5 + 5;
		public const byte RPC_CLIENT__GET_KILL = 6 + 5;
		public const byte RPC_MULTICAST__TAKE_DAMAGE = 7 + 5;
		public const byte RPC_MULTICAST__RESPAWN = 8 + 5;
		
		public NetworkPlayerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (NetworkPlayerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("Server_TakeDamage", Server_TakeDamage, typeof(int), typeof(string), typeof(uint));
			networkObject.RegisterRpc("Server_Death", Server_Death, typeof(string), typeof(uint));
			networkObject.RegisterRpc("Server_GetKill", Server_GetKill, typeof(string));
			networkObject.RegisterRpc("Server_Respawn", Server_Respawn);
			networkObject.RegisterRpc("Multicast_GunFired", Multicast_GunFired);
			networkObject.RegisterRpc("Multicast_Death", Multicast_Death, typeof(Vector3), typeof(Quaternion), typeof(string));
			networkObject.RegisterRpc("Client_GetKill", Client_GetKill, typeof(string));
			networkObject.RegisterRpc("Multicast_TakeDamage", Multicast_TakeDamage);
			networkObject.RegisterRpc("Multicast_Respawn", Multicast_Respawn);

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId))
					ProcessOthers(gameObject.transform, obj.NetworkId + 1);
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata == null)
				return;

			byte transformFlags = obj.Metadata[0];

			if (transformFlags == 0)
				return;

			BMSByte metadataTransform = new BMSByte();
			metadataTransform.Clone(obj.Metadata);
			metadataTransform.MoveStartIndex(1);

			if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
			{
				MainThreadManager.Run(() =>
				{
					transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
					transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
				});
			}
			else if ((transformFlags & 0x01) != 0)
			{
				MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
			}
			else if ((transformFlags & 0x02) != 0)
			{
				MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
			}
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new NetworkPlayerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new NetworkPlayerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// int damage
		/// string instigator
		/// uint instigatorID
		/// </summary>
		public abstract void Server_TakeDamage(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string instigator
		/// uint instigatorID
		/// </summary>
		public abstract void Server_Death(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string killed
		/// </summary>
		public abstract void Server_GetKill(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Server_Respawn(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Multicast_GunFired(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Multicast_Death(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Client_GetKill(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Multicast_TakeDamage(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Multicast_Respawn(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}