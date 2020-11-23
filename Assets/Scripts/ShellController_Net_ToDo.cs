using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Alterar a classe derivada para trabalharmos com Multiplayer
public class ShellController_Net_ToDo : MonoBehaviour
{
    //Campo serializado que armazenará por quanto tempo a arma ficará visível

    //Campo serializado booleano que verificará se o projétil pode danificar os jogadores

    //Campo serializado que verificará se o jogo é Deathmatch

    //Variável booleana que verifica se o projétil está habilitado para explodir

    //Variável float que armazena o lifetime do projétil

    //Variável que armazenará o efeito de explosão do projétil

    //Vriável que armazenará o Meshrenderer do projétil

    // Start is called before the first frame update
    void Start()
    {
        //Pegar a referência para os componentes que utilizaremos (Sistema de partículas e Mesh Renderer)
    }

    // Update is called once per frame
    //Os projéteis são atualizados pelo servidor (ServerCallback)
    void Update()
    {
        //Soma Time.deltaTime a idade do projétil

        //Se o projétil estiver visível por mais tempo que seu tempo de vida
         
            //Destruí-lo na rede
    }


    //Verificar se o projétil atinge algo
      
        //Se o projétil não está ativo, retorne

        //O projétil vai explodir e não encontra-se mais ativo...

        //Desabilita o corpo do projétil

        //Exibir os efeitos de partículas

        //Se não for um servidor, retorne

        //Se o projétil não é letal ou não atingiu um jogador
}
