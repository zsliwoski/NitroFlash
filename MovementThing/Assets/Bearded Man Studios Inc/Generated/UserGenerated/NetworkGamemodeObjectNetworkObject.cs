using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0,0,0,0]")]
	public partial class NetworkGamemodeObjectNetworkObject : NetworkObject
	{
		public const int IDENTITY = 5;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private int _timeRunning;
		public event FieldEvent<int> timeRunningChanged;
		public InterpolateUnknown timeRunningInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int timeRunning
		{
			get { return _timeRunning; }
			set
			{
				// Don't do anything if the value is the same
				if (_timeRunning == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_timeRunning = value;
				hasDirtyFields = true;
			}
		}

		public void SettimeRunningDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_timeRunning(ulong timestep)
		{
			if (timeRunningChanged != null) timeRunningChanged(_timeRunning, timestep);
			if (fieldAltered != null) fieldAltered("timeRunning", _timeRunning, timestep);
		}
		private bool _gamemodeActive;
		public event FieldEvent<bool> gamemodeActiveChanged;
		public InterpolateUnknown gamemodeActiveInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public bool gamemodeActive
		{
			get { return _gamemodeActive; }
			set
			{
				// Don't do anything if the value is the same
				if (_gamemodeActive == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_gamemodeActive = value;
				hasDirtyFields = true;
			}
		}

		public void SetgamemodeActiveDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_gamemodeActive(ulong timestep)
		{
			if (gamemodeActiveChanged != null) gamemodeActiveChanged(_gamemodeActive, timestep);
			if (fieldAltered != null) fieldAltered("gamemodeActive", _gamemodeActive, timestep);
		}
		private int _gamemodeType;
		public event FieldEvent<int> gamemodeTypeChanged;
		public InterpolateUnknown gamemodeTypeInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int gamemodeType
		{
			get { return _gamemodeType; }
			set
			{
				// Don't do anything if the value is the same
				if (_gamemodeType == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_gamemodeType = value;
				hasDirtyFields = true;
			}
		}

		public void SetgamemodeTypeDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_gamemodeType(ulong timestep)
		{
			if (gamemodeTypeChanged != null) gamemodeTypeChanged(_gamemodeType, timestep);
			if (fieldAltered != null) fieldAltered("gamemodeType", _gamemodeType, timestep);
		}
		private int _teamAScore;
		public event FieldEvent<int> teamAScoreChanged;
		public InterpolateUnknown teamAScoreInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int teamAScore
		{
			get { return _teamAScore; }
			set
			{
				// Don't do anything if the value is the same
				if (_teamAScore == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_teamAScore = value;
				hasDirtyFields = true;
			}
		}

		public void SetteamAScoreDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_teamAScore(ulong timestep)
		{
			if (teamAScoreChanged != null) teamAScoreChanged(_teamAScore, timestep);
			if (fieldAltered != null) fieldAltered("teamAScore", _teamAScore, timestep);
		}
		private int _teamBScore;
		public event FieldEvent<int> teamBScoreChanged;
		public InterpolateUnknown teamBScoreInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int teamBScore
		{
			get { return _teamBScore; }
			set
			{
				// Don't do anything if the value is the same
				if (_teamBScore == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x10;
				_teamBScore = value;
				hasDirtyFields = true;
			}
		}

		public void SetteamBScoreDirty()
		{
			_dirtyFields[0] |= 0x10;
			hasDirtyFields = true;
		}

		private void RunChange_teamBScore(ulong timestep)
		{
			if (teamBScoreChanged != null) teamBScoreChanged(_teamBScore, timestep);
			if (fieldAltered != null) fieldAltered("teamBScore", _teamBScore, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			timeRunningInterpolation.current = timeRunningInterpolation.target;
			gamemodeActiveInterpolation.current = gamemodeActiveInterpolation.target;
			gamemodeTypeInterpolation.current = gamemodeTypeInterpolation.target;
			teamAScoreInterpolation.current = teamAScoreInterpolation.target;
			teamBScoreInterpolation.current = teamBScoreInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _timeRunning);
			UnityObjectMapper.Instance.MapBytes(data, _gamemodeActive);
			UnityObjectMapper.Instance.MapBytes(data, _gamemodeType);
			UnityObjectMapper.Instance.MapBytes(data, _teamAScore);
			UnityObjectMapper.Instance.MapBytes(data, _teamBScore);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_timeRunning = UnityObjectMapper.Instance.Map<int>(payload);
			timeRunningInterpolation.current = _timeRunning;
			timeRunningInterpolation.target = _timeRunning;
			RunChange_timeRunning(timestep);
			_gamemodeActive = UnityObjectMapper.Instance.Map<bool>(payload);
			gamemodeActiveInterpolation.current = _gamemodeActive;
			gamemodeActiveInterpolation.target = _gamemodeActive;
			RunChange_gamemodeActive(timestep);
			_gamemodeType = UnityObjectMapper.Instance.Map<int>(payload);
			gamemodeTypeInterpolation.current = _gamemodeType;
			gamemodeTypeInterpolation.target = _gamemodeType;
			RunChange_gamemodeType(timestep);
			_teamAScore = UnityObjectMapper.Instance.Map<int>(payload);
			teamAScoreInterpolation.current = _teamAScore;
			teamAScoreInterpolation.target = _teamAScore;
			RunChange_teamAScore(timestep);
			_teamBScore = UnityObjectMapper.Instance.Map<int>(payload);
			teamBScoreInterpolation.current = _teamBScore;
			teamBScoreInterpolation.target = _teamBScore;
			RunChange_teamBScore(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _timeRunning);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _gamemodeActive);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _gamemodeType);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _teamAScore);
			if ((0x10 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _teamBScore);

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
				if (timeRunningInterpolation.Enabled)
				{
					timeRunningInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					timeRunningInterpolation.Timestep = timestep;
				}
				else
				{
					_timeRunning = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_timeRunning(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (gamemodeActiveInterpolation.Enabled)
				{
					gamemodeActiveInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					gamemodeActiveInterpolation.Timestep = timestep;
				}
				else
				{
					_gamemodeActive = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_gamemodeActive(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (gamemodeTypeInterpolation.Enabled)
				{
					gamemodeTypeInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					gamemodeTypeInterpolation.Timestep = timestep;
				}
				else
				{
					_gamemodeType = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_gamemodeType(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (teamAScoreInterpolation.Enabled)
				{
					teamAScoreInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					teamAScoreInterpolation.Timestep = timestep;
				}
				else
				{
					_teamAScore = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_teamAScore(timestep);
				}
			}
			if ((0x10 & readDirtyFlags[0]) != 0)
			{
				if (teamBScoreInterpolation.Enabled)
				{
					teamBScoreInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					teamBScoreInterpolation.Timestep = timestep;
				}
				else
				{
					_teamBScore = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_teamBScore(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (timeRunningInterpolation.Enabled && !timeRunningInterpolation.current.Near(timeRunningInterpolation.target, 0.0015f))
			{
				_timeRunning = (int)timeRunningInterpolation.Interpolate();
				RunChange_timeRunning(timeRunningInterpolation.Timestep);
			}
			if (gamemodeActiveInterpolation.Enabled && !gamemodeActiveInterpolation.current.Near(gamemodeActiveInterpolation.target, 0.0015f))
			{
				_gamemodeActive = (bool)gamemodeActiveInterpolation.Interpolate();
				RunChange_gamemodeActive(gamemodeActiveInterpolation.Timestep);
			}
			if (gamemodeTypeInterpolation.Enabled && !gamemodeTypeInterpolation.current.Near(gamemodeTypeInterpolation.target, 0.0015f))
			{
				_gamemodeType = (int)gamemodeTypeInterpolation.Interpolate();
				RunChange_gamemodeType(gamemodeTypeInterpolation.Timestep);
			}
			if (teamAScoreInterpolation.Enabled && !teamAScoreInterpolation.current.Near(teamAScoreInterpolation.target, 0.0015f))
			{
				_teamAScore = (int)teamAScoreInterpolation.Interpolate();
				RunChange_teamAScore(teamAScoreInterpolation.Timestep);
			}
			if (teamBScoreInterpolation.Enabled && !teamBScoreInterpolation.current.Near(teamBScoreInterpolation.target, 0.0015f))
			{
				_teamBScore = (int)teamBScoreInterpolation.Interpolate();
				RunChange_teamBScore(teamBScoreInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public NetworkGamemodeObjectNetworkObject() : base() { Initialize(); }
		public NetworkGamemodeObjectNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkGamemodeObjectNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}