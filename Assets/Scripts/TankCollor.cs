using UnityEngine;
using UnityEngine.Networking;

public class TankCollor : NetworkBehaviour {

    [SyncVar]
    public Color color; //A cor para mudar o tanque. O atributo SyncVar significa que o valor será compartilhado com todos os tanques

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

    public void HideTank()
    {
        //Dar um loop no Mesh Renderer e limpá-los
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = Color.clear;
        }
    }
}
