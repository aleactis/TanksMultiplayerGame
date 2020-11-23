using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShellController_Net : NetworkBehaviour
{
    [SerializeField]
    float shellLifetime = 2f; //Quanto tempo a bala ficará visível
    [SerializeField]
    bool canKill = false; //A arma pode danificar os jogadores?
    [SerializeField]
    bool isDeathmatch = false; //É um jogo Deathmatch

    bool isLive = true; //A arma está habilitada para explodir???
    float age; //Quanto tempo a arma está viva?

    ParticleSystem explosionParticles; //O efeito de explosão da bala
    MeshRenderer shellRenderer; //O Meshrenderer da arma

    // Start is called before the first frame update
    void Start()
    {
        //Pegar a referência para os componentes que utilizaremos
        explosionParticles = GetComponentInChildren<ParticleSystem>();
        shellRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    //As balas são atualizadas pelo servidor
    [ServerCallback]
    void Update()
    {
        //Se a arma estiver visível por muito tempo
        age += Time.deltaTime;
        if (age >= shellLifetime)
        {
            //Destruí-la na rede
            NetworkServer.Destroy(gameObject);
        }
    }

    //Quando a bala atinge algo
    void OnCollisionEnter(Collision other)
    {
        //Se a bala não está ativa...
        if (!isLive)
            return;

        //A bala vai explodir e não encontra-se mais ativa...
        isLive = false;

        //Esconde o corpo da bala
        shellRenderer.enabled = false;

        //Mostrar os efeitos de partículas
        explosionParticles.Play(true);

        //Se não for um servidor, retorne
        if (!isServer)
            return;

        //Se a bala não é letal ou não atingiu um jogador
        if (!canKill || other.gameObject.tag != "Player")
            return;

        if (isDeathmatch)
        {
            //Obtém uma referência do script PlayerHealth_DM e informa que tomou dano
            TankHealth_DM health = other.gameObject.GetComponent<TankHealth_DM>();
            if (health != null)
            {
                health.TakeDamage(25);
            }
        }
    }
}