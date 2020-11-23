using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerCollor_Hook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager,
        GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        TankCollor player = gamePlayer.GetComponent<TankCollor>();

        player.color = lobby.playerColor;
    }
}
