using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using Prototype.NetworkLobby;


//public class PlayerCollor_Hook : LobbyHook
    public class PlayerCollor : NetworkBehaviour
{
    [SyncVar]
    public Color color; //A cor para mudar o tanque. O atributo SyncVar significa que o valor será compartilhado com todos os tanques

    //Essas variáveis ​​terão seus valores sincronizados do servidor para clientes no jogo que estão no estado pronto.
    //Definir o valor de um [SyncVar] o marca como sujo, então ele será enviado aos clientes no final do frame atual. Somente valores simples podem ser marcados como[SyncVars]. 
    //O tipo da variável SyncVar não pode ser de uma DLL ou Assembly (externo).

    MeshRenderer[] rends; //Array para armazenar o Mesh Renderer do tanque

    void Start()
    {
        //Localiza todos os Mesh Rederers no tanque e altera suas cores para os 
        //jogadores escolherem. Com a cor sendo uma SyncVar, ela será compartilhada
        //com todas as cópias dos jogadores em todos os clientes
        rends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rends.Length; i++)
            rends[i].material.color = color;
    }

    public void HidePlayer()
    {
        //Dar um loop no Mesh Renderer e limpá-los
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = Color.clear;
        }
    }
    //public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager,
    //    GameObject lobbyPlayer, GameObject gamePlayer)
    //{
    //    LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
    //    PlayerCollor Player = gamePlayer.GetComponent<PlayerCollor>();
    //    Player.color = lobby.playerColor;
    //}
}
