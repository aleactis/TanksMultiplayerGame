using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathMatchMannager1 : NetworkBehaviour {
    
    //Criar uma lista estática de tanques baseado em TankHealth_DM
    static List<TankHealth_DM> tanks = new List<TankHealth_DM>();

    public static void AddTank(TankHealth_DM tank)
    {
        //Adiciona o tanque a lista
        tanks.Add(tank);
    }

    public static bool RemoveTankAndCheckWinner(TankHealth_DM tank)
    {
        //Remove o tanque que morreu
        tanks.Remove(tank);

        //Se resta apenas 1 tanque, retorna verdadeiro que significa que ele é o vencedor
        if (tanks.Count == 1)
            return true;

        //De outra forma, retorna false quando há múltiplos tanques restantes
        return false;
    }

    public static TankHealth_DM GetWinner()
    {
        //Checa para ter certeza que há um vencedor
        if (tanks.Count != 1)
            return null;

        //Retorna o último tanque: O vencedor
        return tanks[0];
    }
}
