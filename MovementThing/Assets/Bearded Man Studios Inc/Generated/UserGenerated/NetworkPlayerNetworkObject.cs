using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0,0,0,0,0,0,0,0]")]
	public partial class NetworkPlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 6;

		private byte[] _dirtyFields = new byte[2];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private Vector3 _position;
		public event FieldEvent<Vector3> positionChanged;
		public InterpolateVector3 positionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		private Quaternion _rotation;
		public event FieldEvent<Quaternion> rotationChanged;
		public InterpolateQuaternion rotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion rotation
		{
			get { return _rotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_rotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_rotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetrotationDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_rotation(ulong timestep)
		{
			if (rotationChanged != null) rotationChanged(_rotation, timestep);
			if (fieldAltered != null) fieldAltered("rotation", _rotation, timestep);
		}
		private bool _ballHeld;
		public event FieldEvent<bool> ballHeldChanged;
		public InterpolateUnknown ballHeldInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public bool ballHeld
		{
			get { return _ballHeld; }
			set
			{
				// Don't do anything if the value is the same
				if (_ballHeld == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_ballHeld = value;
				hasDirtyFields = true;
			}
		}

		public void SetballHeldDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_ballHeld(ulong timestep)
		{
			if (ballHeldChanged != null) ballHeldChanged(_ballHeld, timestep);
			if (fieldAltered != null) fieldAltered("ballHeld", _ballHeld, timestep);
		}
		private int _health;
		public event FieldEvent<int> healthChanged;
		public InterpolateUnknown healthInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int health
		{
			get { return _health; }
			set
			{
				// Don't do anything if the value is the same
				if (_health == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_health = value;
				hasDirtyFields = true;
			}
		}

		public void SethealthDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_health(ulong timestep)
		{
			if (healthChanged != null) healthChanged(_health, timestep);
			if (fieldAltered != null) fieldAltered("health", _health, timestep);
		}
		private int _kills;
		public event FieldEvent<int> killsChanged;
		public InterpolateUnknown killsInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int kills
		{
			get { return _kills; }
			set
			{
				// Don't do anything if the value is the same
				if (_kills == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x10;
				_kills = value;
				hasDirtyFields = true;
			}
		}

		public void SetkillsDirty()
		{
			_dirtyFields[0] |= 0x10;
			hasDirtyFields = true;
		}

		private void RunChange_kills(ulong timestep)
		{
			if (killsChanged != null) killsChanged(_kills, timestep);
			if (fieldAltered != null) fieldAltered("kills", _kills, timestep);
		}
		private int _score;
		public event FieldEvent<int> scoreChanged;
		public InterpolateUnknown scoreInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int score
		{
			get { return _score; }
			set
			{
				// Don't do anything if the value is the same
				if (_score == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x20;
				_score = value;
				hasDirtyFields = true;
			}
		}

		public void SetscoreDirty()
		{
			_dirtyFields[0] |= 0x20;
			hasDirtyFields = true;
		}

		private void RunChange_score(ulong timestep)
		{
			if (scoreChanged != null) scoreChanged(_score, timestep);
			if (fieldAltered != null) fieldAltered("score", _score, timestep);
		}
		private int _death;
		public event FieldEvent<int> deathChanged;
		public InterpolateUnknown deathInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int death
		{
			get { return _death; }
			set
			{
				// Don't do anything if the value is the same
				if (_death == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x40;
				_death = value;
				hasDirtyFields = true;
			}
		}

		public void SetdeathDirty()
		{
			_dirtyFields[0] |= 0x40;
			hasDirtyFields = true;
		}

		private void RunChange_death(ulong timestep)
		{
			if (deathChanged != null) deathChanged(_death, timestep);
			if (fieldAltered != null) fieldAltered("death", _death, timestep);
		}
		private bool _grappleHeld;
		public event FieldEvent<bool> grappleHeldChanged;
		public InterpolateUnknown grappleHeldInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public bool grappleHeld
		{
			get { return _grappleHeld; }
			set
			{
				// Don't do anything if the value is the same
				if (_grappleHeld == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x80;
				_grappleHeld = value;
				hasDirtyFields = true;
			}
		}

		public void SetgrappleHeldDirty()
		{
			_dirtyFields[0] |= 0x80;
			hasDirtyFields = true;
		}

		private void RunChange_grappleHeld(ulong timestep)
		{
			if (grappleHeldChanged != null) grappleHeldChanged(_grappleHeld, timestep);
			if (fieldAltered != null) fieldAltered("grappleHeld", _grappleHeld, timestep);
		}
		private bool _isDead;
		public event FieldEvent<bool> isDeadChanged;
		public InterpolateUnknown isDeadInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public bool isDead
		{
			get { return _isDead; }
			set
			{
				// Don't do anything if the value is the same
				if (_isDead == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[1] |= 0x1;
				_isDead = value;
				hasDirtyFields = true;
			}
		}

		public void SetisDeadDirty()
		{
			_dirtyFields[1] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_isDead(ulong timestep)
		{
			if (isDeadChanged != null) isDeadChanged(_isDead, timestep);
			if (fieldAltered != null) fieldAltered("isDead", _isDead, timestep);
		}
		private int _team;
		public event FieldEvent<int> teamChanged;
		public InterpolateUnknown teamInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int team
		{
			get { return _team; }
			set
			{
				// Don't do anything if the value is the same
				if (_team == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[1] |= 0x2;
				_team = value;
				hasDirtyFields = true;
			}
		}

		public void SetteamDirty()
		{
			_dirtyFields[1] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_team(ulong timestep)
		{
			if (teamChanged != null) teamChanged(_team, timestep);
			if (fieldAltered != null) fieldAltered("team", _team, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			rotationInterpolation.current = rotationInterpolation.target;
			ballHeldInterpolation.current = ballHeldInterpolation.target;
			healthInterpolation.current = healthInterpolation.target;
			killsInterpolation.current = killsInterpolation.target;
			scoreInterpolation.current = scoreInterpolation.target;
			deathInterpolation.current = deathInterpolation.target;
			grappleHeldInterpolation.current = grappleHeldInterpolation.target;
			isDeadInterpolation.current = isDeadInterpolation.target;
			teamInterpolation.current = teamInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _rotation);
			UnityObjectMapper.Instance.MapBytes(data, _ballHeld);
			UnityObjectMapper.Instance.MapBytes(data, _health);
			UnityObjectMapper.Instance.MapBytes(data, _kills);
			UnityObjectMapper.Instance.MapBytes(data, _score);
			UnityObjectMapper.Instance.MapBytes(data, _death);
			UnityObjectMapper.Instance.MapBytes(data, _grappleHeld);
			UnityObjectMapper.Instance.MapBytes(data, _isDead);
			UnityObjectMapper.Instance.MapBytes(data, _team);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector3>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_rotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			rotationInterpolation.current = _rotation;
			rotationInterpolation.target = _rotation;
			RunChange_rotation(timestep);
			_ballHeld = UnityObjectMapper.Instance.Map<bool>(payload);
			ballHeldInterpolation.current = _ballHeld;
			ballHeldInterpolation.target = _ballHeld;
			RunChange_ballHeld(timestep);
			_health = UnityObjectMapper.Instance.Map<int>(payload);
			healthInterpolation.current = _health;
			healthInterpolation.target = _health;
			RunChange_health(timestep);
			_kills = UnityObjectMapper.Instance.Map<int>(payload);
			killsInterpolation.current = _kills;
			killsInterpolation.target = _kills;
			RunChange_kills(timestep);
			_score = UnityObjectMapper.Instance.Map<int>(payload);
			scoreInterpolation.current = _score;
			scoreInterpolation.target = _score;
			RunChange_score(timestep);
			_death = UnityObjectMapper.Instance.Map<int>(payload);
			deathInterpolation.current = _death;
			deathInterpolation.target = _death;
			RunChange_death(timestep);
			_grappleHeld = UnityObjectMapper.Instance.Map<bool>(payload);
			grappleHeldInterpolation.current = _grappleHeld;
			grappleHeldInterpolation.target = _grappleHeld;
			RunChange_grappleHeld(timestep);
			_isDead = UnityObjectMapper.Instance.Map<bool>(payload);
			isDeadInterpolation.current = _isDead;
			isDeadInterpolation.target = _isDead;
			RunChange_isDead(timestep);
			_team = UnityObjectMapper.Instance.Map<int>(payload);
			teamInterpolation.current = _team;
			teamInterpolation.target = _team;
			RunChange_team(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _rotation);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _ballHeld);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _health);
			if ((0x10 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _kills);
			if ((0x20 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _score);
			if ((0x40 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _death);
			if ((0x80 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _grappleHeld);
			if ((0x1 & _dirtyFields[1]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _isDead);
			if ((0x2 & _dirtyFields[1]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _team);

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (rotationInterpolation.Enabled)
				{
					rotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					rotationInterpolation.Timestep = timestep;
				}
				else
				{
					_rotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_rotation(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (ballHeldInterpolation.Enabled)
				{
					ballHeldInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					ballHeldInterpolation.Timestep = timestep;
				}
				else
				{
					_ballHeld = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_ballHeld(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (healthInterpolation.Enabled)
				{
					healthInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					healthInterpolation.Timestep = timestep;
				}
				else
				{
					_health = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_health(timestep);
				}
			}
			if ((0x10 & readDirtyFlags[0]) != 0)
			{
				if (killsInterpolation.Enabled)
				{
					killsInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					killsInterpolation.Timestep = timestep;
				}
				else
				{
					_kills = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_kills(timestep);
				}
			}
			if ((0x20 & readDirtyFlags[0]) != 0)
			{
				if (scoreInterpolation.Enabled)
				{
					scoreInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					scoreInterpolation.Timestep = timestep;
				}
				else
				{
					_score = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_score(timestep);
				}
			}
			if ((0x40 & readDirtyFlags[0]) != 0)
			{
				if (deathInterpolation.Enabled)
				{
					deathInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					deathInterpolation.Timestep = timestep;
				}
				else
				{
					_death = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_death(timestep);
				}
			}
			if ((0x80 & readDirtyFlags[0]) != 0)
			{
				if (grappleHeldInterpolation.Enabled)
				{
					grappleHeldInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					grappleHeldInterpolation.Timestep = timestep;
				}
				else
				{
					_grappleHeld = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_grappleHeld(timestep);
				}
			}
			if ((0x1 & readDirtyFlags[1]) != 0)
			{
				if (isDeadInterpolation.Enabled)
				{
					isDeadInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					isDeadInterpolation.Timestep = timestep;
				}
				else
				{
					_isDead = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_isDead(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[1]) != 0)
			{
				if (teamInterpolation.Enabled)
				{
					teamInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					teamInterpolation.Timestep = timestep;
				}
				else
				{
					_team = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_team(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.Near(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector3)positionInterpolation.Interpolate();
				RunChange_position(positionInterpolation.Timestep);
			}
			if (rotationInterpolation.Enabled && !rotationInterpolation.current.Near(rotationInterpolation.target, 0.0015f))
			{
				_rotation = (Quaternion)rotationInterpolation.Interpolate();
				RunChange_rotation(rotationInterpolation.Timestep);
			}
			if (ballHeldInterpolation.Enabled && !ballHeldInterpolation.current.Near(ballHeldInterpolation.target, 0.0015f))
			{
				_ballHeld = (bool)ballHeldInterpolation.Interpolate();
				RunChange_ballHeld(ballHeldInterpolation.Timestep);
			}
			if (healthInterpolation.Enabled && !healthInterpolation.current.Near(healthInterpolation.target, 0.0015f))
			{
				_health = (int)healthInterpolation.Interpolate();
				RunChange_health(healthInterpolation.Timestep);
			}
			if (killsInterpolation.Enabled && !killsInterpolation.current.Near(killsInterpolation.target, 0.0015f))
			{
				_kills = (int)killsInterpolation.Interpolate();
				RunChange_kills(killsInterpolation.Timestep);
			}
			if (scoreInterpolation.Enabled && !scoreInterpolation.current.Near(scoreInterpolation.target, 0.0015f))
			{
				_score = (int)scoreInterpolation.Interpolate();
				RunChange_score(scoreInterpolation.Timestep);
			}
			if (deathInterpolation.Enabled && !deathInterpolation.current.Near(deathInterpolation.target, 0.0015f))
			{
				_death = (int)deathInterpolation.Interpolate();
				RunChange_death(deathInterpolation.Timestep);
			}
			if (grappleHeldInterpolation.Enabled && !grappleHeldInterpolation.current.Near(grappleHeldInterpolation.target, 0.0015f))
			{
				_grappleHeld = (bool)grappleHeldInterpolation.Interpolate();
				RunChange_grappleHeld(grappleHeldInterpolation.Timestep);
			}
			if (isDeadInterpolation.Enabled && !isDeadInterpolation.current.Near(isDeadInterpolation.target, 0.0015f))
			{
				_isDead = (bool)isDeadInterpolation.Interpolate();
				RunChange_isDead(isDeadInterpolation.Timestep);
			}
			if (teamInterpolation.Enabled && !teamInterpolation.current.Near(teamInterpolation.target, 0.0015f))
			{
				_team = (int)teamInterpolation.Interpolate();
				RunChange_team(teamInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[2];

		}

		public NetworkPlayerNetworkObject() : base() { Initialize(); }
		public NetworkPlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkPlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}