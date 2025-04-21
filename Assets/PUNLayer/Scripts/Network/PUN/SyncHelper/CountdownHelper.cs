using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CountdownHelper : MonoBehaviourPunCallbacks, INetworkCountdown
{

	public string countdownKey = "defaultCountDownKey"; // clean key without prefix
	[SerializeField]private string prefix = "cd-";
	private string PrefixedKey => prefix + countdownKey;
	private int NetworkTimestamp => PhotonNetwork.ServerTimestamp;

	public Action OnCountdownFinished { get; set; }
	public Action<INetworkCountdown.CountdownState, int> OnCountdownTick { get; set; }
	
	[SerializeField]INetworkCountdown.CountdownState previousState = INetworkCountdown.CountdownState.Idle;
	
#if UNITY_EDITOR
	[ContextMenu("DebugPrint")]
	public void DebugPrint()=>Debug.LogWarningFormat("PrefixedKey:{0}, State:{1}, TimeRemaining:{2}, IsRunning:{3}", PrefixedKey, State, TimeRemaining, IsRunning);
#endif
	public INetworkCountdown.CountdownState State
	{
		get
		{
			if (!PhotonNetwork.InRoom ||
			    !PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PrefixedKey, out var val) ||
			    val is not int endTimestamp)
			{
				return INetworkCountdown.CountdownState.Idle;
			}

			var remaining = endTimestamp - NetworkTimestamp;
			return remaining > 0 ? INetworkCountdown.CountdownState.Running : INetworkCountdown.CountdownState.Ended;
		}
	}

	public int TimeRemaining
	{
		get
		{
			if (PhotonNetwork.InRoom && 
			    PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PrefixedKey, out var val))
			{
				return (int)val - NetworkTimestamp;
			}
		
			return 0;
		}
	}

	public bool IsRunning => State == INetworkCountdown.CountdownState.Running && TimeRemaining > 0;

	private ExitGames.Client.Photon.Hashtable ht = new();
	// Start the countdown (only for master client)
	public void StartCountdown(int countdownDuration = 10000)
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) 
			return;

		// Only set the property if it's not already set
		if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PrefixedKey)) 
			return;

		ht.Clear();
		ht[PrefixedKey] = NetworkTimestamp + countdownDuration;

		// Set room property to sync countdown across all clients
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
	}

	// // Try to initialize countdown by reading the start time from room properties
	// public void TryLoadCountdown()
	// {
	// }
	//
	// public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	// {
	//  if (!propertiesThatChanged.ContainsKey(PrefixedKey))
	//   return;
	//  
	//  TryLoadCountdown();
	// }
	//
	// public override void OnJoinedRoom()
	// {
	//     TryLoadCountdown();
	// }
	
	public override void OnLeftRoom()
	{
		previousState = INetworkCountdown.CountdownState.Idle;
	}
	
	private void Update()
	{
		var sta = State;
		var remaining = TimeRemaining;

		// Notify tick listener ALWAYS
		OnCountdownTick?.Invoke(sta, remaining);

		// If countdown ends, set state to Ended and trigger OnCountdownFinished ONCE
		if (previousState != sta && sta == INetworkCountdown.CountdownState.Ended)
		{
			OnCountdownFinished?.Invoke();
		}

		previousState = sta;
	}
}
