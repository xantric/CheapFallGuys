// LobbyGameManager.cs
using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LobbyGameManager : MonoBehaviourPunCallbacks
{
    [Header("Match Settings")]
    [Tooltip("How many players must join before starting the countdown.")]
    [SerializeField] private byte requiredPlayers = 4;
    [Tooltip("Seconds of countdown once everyone is in.")]
    [SerializeField] private float countdownDuration = 5f;

    [Header("Spawn‑Grid Settings")]
    [Tooltip("Name of your player prefab (in Resources/).")]
    [SerializeField] private string playerPrefabName = "PlayerPrefab";
    [Tooltip("World‑space center of your spawn grid.")]
    [SerializeField] private Vector3 gridCenter = Vector3.zero;
    [Tooltip("Distance between adjacent spawn points.")]
    [SerializeField] private float spawnSpacing = 2f;
    [Tooltip("Y‑height at which to spawn players.")]
    [SerializeField] private float spawnHeight = 1f;

    [Header("UI Panels")]
    [Tooltip("Panel shown while waiting for players.")]
    [SerializeField] private GameObject waitingPanel;
    [Tooltip("Text showing “Players: X/Y”")]
    [SerializeField] private TMP_Text waitingText;
    [Tooltip("Panel shown during the countdown.")]
    [SerializeField] private GameObject countdownPanel;
    [Tooltip("Text showing the countdown timer.")]
    [SerializeField] private TMP_Text countdownText;

    private bool _countdownRunning = false;

    public override void OnJoinedRoom()
    {
        // show waiting UI
        waitingPanel.SetActive(true);
        countdownPanel.SetActive(false);
        UpdateWaitingText();
        TryKickOffCountdown();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateWaitingText();
        TryKickOffCountdown();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateWaitingText();
        // if someone leaves mid‑countdown, cancel it
        if (PhotonNetwork.IsMasterClient && _countdownRunning &&
            PhotonNetwork.CurrentRoom.PlayerCount < requiredPlayers)
        {
            _countdownRunning = false;
            countdownPanel.SetActive(false);
            waitingPanel.SetActive(true);
        }
    }

    private void UpdateWaitingText()
    {
        int count = PhotonNetwork.CurrentRoom.PlayerCount;
        waitingText.text = $"Players: {count}/{requiredPlayers}";
    }

    private void TryKickOffCountdown()
    {
        if (!PhotonNetwork.IsMasterClient || _countdownRunning) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayers)
        {
            _countdownRunning = true;
            waitingPanel.SetActive(false);
            countdownPanel.SetActive(true);
            StartCoroutine(CountdownAndStart());
        }
    }

    private IEnumerator CountdownAndStart()
    {
        float t = countdownDuration;
        while (t > 0f && _countdownRunning)
        {
            countdownText.text = Mathf.CeilToInt(t).ToString();
            yield return new WaitForSeconds(1f);
            t -= 1f;
            if (PhotonNetwork.CurrentRoom.PlayerCount < requiredPlayers)
            {
                // someone left; abort
                _countdownRunning = false;
                countdownPanel.SetActive(false);
                waitingPanel.SetActive(true);
                yield break;
            }
        }
        if (!_countdownRunning) yield break;

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownPanel.SetActive(false);
        photonView.RPC(nameof(RPC_StartGame), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        SpawnMeInGrid();
    }

    private void SpawnMeInGrid()
    {
        // build sorted list by actor number
        var players = PhotonNetwork.CurrentRoom.Players.Values
                         .OrderBy(p => p.ActorNumber)
                         .ToArray();
        int localIndex = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(maxPlayers));
        float offset = spawnSpacing * (gridSize - 1) / 2f;

        int row = localIndex / gridSize;
        int col = localIndex % gridSize;

        Vector3 localPos = new Vector3(
            col * spawnSpacing - offset,
            spawnHeight,
            row * spawnSpacing - offset
        );
        Vector3 worldPos = gridCenter + localPos;

        PhotonNetwork.Instantiate(playerPrefabName, worldPos, Quaternion.identity);
    }
}
