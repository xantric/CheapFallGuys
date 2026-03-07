using System;
using System.Collections;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    // Start is called before the first frame update
    [Header("Player Object")]
    public GameObject player;

    [Header("Player Spawn Point")]
    public Transform spanPoint;

    [Header("Free Look Camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Camera UI")]
    public GameObject roomCam;

    [Header("UI")]
    public GameObject nickNameUI;
    public GameObject connectingUI;
    public GameObject raceCountdownPanel;
    public TextMeshProUGUI raceCountdownText;

    [Header("Room Name")]
    public string roomName = "test";

    [Header("Match Settings")]
    [Tooltip("How many players needed before the game starts")]
    public byte requiredPlayers = 4;
    [Tooltip("Seconds of race‑start countdown")]
    public float raceCountdownDuration = 5f;

    [Header("Spawn Grid")]
    public Vector3 gridCenter = Vector3.zero;
    public float spawnSpacing = 2f;
    public float spawnHeight = 1f;

    [Header("Connecting Text")]
    public TextMeshProUGUI connectingTxt;

    string nickName = "unnamed";

    private bool _spawned = false;
    private GameObject localPlayerInstance;

    private void Awake()
    {
        Instance = this;
    }
    public void SetNickname(string _name)
    {
        nickName = _name;
    }
    public void OnJoinButtonPressed()
    {
        Debug.LogWarning(message: "Connecting. . . ");
        Debug.LogWarning(roomName);
        PhotonNetwork.JoinOrCreateRoom(roomName, new Photon.Realtime.RoomOptions { MaxPlayers = requiredPlayers }, null);

        nickNameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        Debug.LogWarning("Room Joined");
        requiredPlayers = (byte)PhotonNetwork.CurrentRoom.MaxPlayers;
        //SpawnPlayer();
        TryStartGame();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TryStartGame();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!_spawned) return;
        // if race already started, ignore
        if (_spawned) return;

        connectingTxt.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + requiredPlayers.ToString();
        // nothing else: we only spawn once
    }

    private void TryStartGame()
    {
        connectingTxt.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + requiredPlayers.ToString();
        // Only the MasterClient drives the start
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Check if we have enough players
        if (PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayers)
        {
            // Fire the spawn RPC for everyone
            _spawned = true;
            photonView.RPC("RPC_SpawnPlayers", RpcTarget.AllBuffered);
            
        }
    }

    [PunRPC]
    void RPC_SpawnPlayers()
    {
        roomCam.SetActive(false);
        // Build a sorted list by ActorNumber
        var players = PhotonNetwork.PlayerList;
        System.Array.Sort(players, (a, b) => a.ActorNumber.CompareTo(b.ActorNumber));

        // Find our index
        int myIndex = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

        // Calculate grid dimensions
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(players.Length));
        float offset = spawnSpacing * (gridSize - 1) / 2f;

        int row = myIndex / gridSize;
        int col = myIndex % gridSize;

        Vector3 localOffset = new Vector3(
            col * spawnSpacing - offset,
            spawnHeight,
            row * spawnSpacing - offset
        );

        Vector3 spawnPos = spanPoint.position + localOffset;
        Debug.LogWarning(spawnPos);
        SpawnPlayer( spawnPos );
        
    }
    public void SpawnPlayer(Vector3 pos)
    {
        GameObject _player = PhotonNetwork.Instantiate(player.name, pos, player.gameObject.GetComponent<Transform>().rotation);
        //_player.GetComponent<PlayerHealth>().isLocalPlayer = true;
        PhotonView view = _player.GetComponent<PhotonView>();
        
        view.RPC("SetPlayerName", RpcTarget.AllBuffered, nickName);
        if (view.IsMine)
        {
            localPlayerInstance = _player;
        }
        if (view != null && view.IsMine && cinemachineVirtualCamera != null)
        {
            Transform lookAt = _player.transform.GetChild(0);
            cinemachineVirtualCamera.Follow = lookAt;
            //cinemachineVirtualCamera.LookAt = lookAt;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // startTime = now + small buffer to let instantiation happen
            double startTime = PhotonNetwork.Time + 0.5;
            photonView.RPC(
                nameof(RPC_StartRaceCountdown),
                RpcTarget.AllBuffered,
                startTime,
                raceCountdownDuration
            );
        }
    }

    [PunRPC]
    private void RPC_StartRaceCountdown(double startTime, float duration)
    {
        StartCoroutine(RaceCountdown(startTime, duration));
    }

    private IEnumerator RaceCountdown(double startTime, float duration)
    {
        // Wait until the absolute photon time
        double now = PhotonNetwork.Time;
        double toWait = startTime - now;
        if (toWait > 0)
            yield return new WaitForSeconds((float)toWait);

        // Disable movement inputs
        if (localPlayerInstance != null)
        {
            var tpc = localPlayerInstance.GetComponent<ThirdPersonController>();
            if (tpc != null) tpc.enabled = false;
            var push = localPlayerInstance.GetComponent<BasicRigidBodyPush>();
            if (push != null) push.enabled = false;
        }

        // Show countdown UI
        raceCountdownPanel.SetActive(true);
        float t = duration;
        while (t > 0f)
        {
            raceCountdownText.text = Mathf.CeilToInt(t).ToString();
            // MasterClient triggers the tick sound for everyone
            if (PhotonNetwork.IsMasterClient)
                SoundManager.Instance.photonView.RPC(
                    nameof(SoundManager.PlayCountdownTick),
                    RpcTarget.All
                );
            yield return new WaitForSeconds(1f);
            t -= 1f;
        }
        // After loop, before GO:
        if (PhotonNetwork.IsMasterClient)
            SoundManager.Instance.photonView.RPC(
                nameof(SoundManager.PlayRaceStart),
                RpcTarget.All
            );
        raceCountdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        raceCountdownPanel.SetActive(false);

        // Re‑enable movement
        if (localPlayerInstance != null)
        {
            var tpc = localPlayerInstance.GetComponent<ThirdPersonController>();
            if (tpc != null) tpc.enabled = true;
            var push = localPlayerInstance.GetComponent<BasicRigidBodyPush>();
            if (push != null) push.enabled = true;
        }
    }

    public void SetRequiredPlayers(int value)
    {
        requiredPlayers = (byte)(value + 2);
        Debug.LogWarning("Required players set to: " + requiredPlayers);
    }

}
