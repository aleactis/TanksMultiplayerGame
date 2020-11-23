using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathMatchManager : NetworkBehaviour {

    static List<PlayerHealth_DM> players = new List<PlayerHealth_DM>();

    public static void AddPlayer(PlayerHealth_DM player)
    {
        //Adiciona o Player à lista
        players.Add(player);
    }

    public static bool RemovePlayerAndCheckWinner(PlayerHealth_DM player)
    {
        //Remove o Player que morreu
        players.Remove(player);

        //Se resta apenas 1 player, retorna verdadeiro que significa que ele é o vencedor
        if (players.Count == 1)
            return true;

        //De outra forma, retorna false quando há múltiplos Players restantes
        return false;
    }

    public static PlayerHealth_DM GetWinner()
    {
        //Checa para ter certeza que há um vencedor
        if (players.Count != 1)
            return null;

        //Retorna o último player: O vencedor
        return players[0];
    }
}
