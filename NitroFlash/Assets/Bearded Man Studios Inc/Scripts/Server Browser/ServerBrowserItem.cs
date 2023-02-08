using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BeardedManStudios.Forge.Networking.Unity
{
    public class ServerBrowserItem : MonoBehaviour
    {
        public Text NameText;
		public Text playerCountText;
        public Button ConnectButton;

		public void SetData(string name, int playerCount, int maxPlayers, UnityAction callback)
        {
            NameText.text = name;
			playerCountText.text = playerCount + "/" + maxPlayers;
            ConnectButton.onClick.AddListener(callback);
        }
    }
}
