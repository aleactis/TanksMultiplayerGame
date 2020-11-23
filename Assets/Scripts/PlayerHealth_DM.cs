using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth_DM : NetworkBehaviour
{
    [SerializeField] float startingHealth = 100f;     //A quantidade de vida que cada Player começa a partida
    public Slider slider;                             //O slider para representar a quantidade de saúde que o Player atualmente possui de vida
    public Image fillImage;                           //O componente Image do Slider
    public Color fullHealthColor = Color.green;       // A cor da barra de vida quando estiver cheio de vida
    public Color zeroHealthColor = Color.red;         // A cor da barra de vida quando estiver morrendo
    public GameObject explosionPrefab;                //Um prefab que será instanciado em Awake (Start), então usado sempre que o Player morrer

    private AudioSource explosionAudio; //A fonte de áudio para jogar quando o Player explode
    private ParticleSystem explosionParticles; //O sistema de partículas que irá jogar quando o Player for destruído

    //O atributo hook pode ser usado para especificar uma função a ser chamada 
    //quando a variável de sincronização altera o valor no cliente.
    [SyncVar (hook = "OnChangeHealth")]
    private float currentHealth; //Quanta vida o Player tem atualmente. 

    private bool isDead; //O Player foi morto?

    Text informationText; //Elemento texto que dará informações para o jogador

    void Start()
    {
        //Se for Servidor, chama o DeathMatchManager que este Player gerou
        if (isServer)
            DeathMatchManager.AddPlayer(this);

        //Instancia o Prefab da explosão e pega uma referência para o sistema de partículas nele
        explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();

        //Pega uma referência para o audio source no prefab instanciado
        explosionAudio = explosionParticles.GetComponent<AudioSource>();

        // Desabilita o prefab para que ele possa ser ativado quando necessário
        explosionParticles.gameObject.SetActive(false);


        //Quando o Player está habilitado, reinicia a vida do Player e se está ou não morto
        currentHealth = startingHealth;

        //Atualizar o slider de vida e cor
        SetHealthUI(currentHealth);

        isDead = false;
    }

    //Método para atualizar o slider de vida atual (UI)
    private void SetHealthUI(float value)
    {
        // Definir o valor do slider apropriadamente
        slider.value = value;

        // Interpolar as cores da barra entre as cores escolhidas baseado no percentual atual da vida atual.
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }

    [Command]
    public void CmdTakeDamage(int amount)
    {
        //Dano será calculado somente no servidor. Isto evita que um cliente
        //tente hackear. Também, se este Player já está morto, não precisa rodar
        if (!isServer || currentHealth <= 0)
            return;

        //Reduz a vida atual pela quantidade de dano causado
        currentHealth -= amount;

        //Atualizar o slider de vida e cor
        SetHealthUI(currentHealth);

        //Se o jogador está com energia zerada ou menor que zero
        if (currentHealth <= 0f && !isDead)
        {
            currentHealth = 0;

            //Chama um método em todas as instâncias deste objeto em todos os 
            //clientes (Isto é chamdado de RPC)
            RpcDied();

            //Chamar o DeathMatchManager para este Player morto
            if (DeathMatchManager.RemovePlayerAndCheckWinner(this))
            {
                PlayerHealth_DM player = DeathMatchManager.GetWinner();
                player.RpcWon();

                //Assim que a partida acabou, o servidor trará os jogadores de volta
                //ao Lobby após 3 segundos
                Invoke("BackToLobby", 3f);
            }

            //Sair do método. Isto é utilizado caso tivermos efeitos de "ferido"
            //conforme abaixo. Não queremos os efeitos de "ferir" ao jogador quando
            //o Player foi destruído. Por isso abandonamos o método e evitamos 
            //que estes efeitos sejam executados.
            return;
        }
        //Se tivermos algum efeito de "ferimento" quando o Player é 
        //danificado, poderíamos executá-los aqui.
    }

    //Uma vez que o dano foi executado no servidor, se o código de um Player 
    //que estava sendo destruído fosse executado lá, ele só seria visível na
    //máquina do servidor. Como queremos o mesmo Player destruído em todos os
    //clientes para que todos os jogadores vejam o mesmo Player destruído,
    //precisamos usar um RPC.
    [ClientRpc]
    void RpcDied()
    {
        // Define a flag para true, pois esta função é chamada apenas uma vez
        isDead = true;

        //Move a esplosão instanciada para a posição do Player e ativa
        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);

        //Executa o sistema de partículas
        explosionParticles.Play();

        //Toca o audio de explosão
        explosionAudio.Play();

        //Escurece o Player para mostrar que ele morreu
        //GetComponent<PlayerCollor>().HidePlayer();

        //Se um Player morreu e é o LocalPlayer, significa que eles 
        //perderam (desde que eles sejam um dos que morreram).
        if (isLocalPlayer)
        {
            //Localiza o objeto texto Game Over na tela
            informationText = GameObject.FindObjectOfType<Text>();
            informationText.text = "Game Over";

            //Desabilitar as funções do Player
            //GetComponent<ShellController_Net>().enabled = false;
            //GetComponent<PlayerShooting_Net>().enabled = false;
        }
    }

    [ClientRpc]
    public void RpcWon()
    {
        //Se um Player venceu e é o LocalPlayer, significa que eles venceram
        //(desde que eles não sejam um dos que morreram)
        if (isLocalPlayer)
        {
            //Localiza o objeto texto Game Over na tela
            informationText = GameObject.FindObjectOfType<Text>();
            informationText.text = "Você Venceu";
        }
    }

    void BackToLobby()
    {
        //Vai para o Lobby
        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    }

    //Altera a barra de vida
    void OnChangeHealth(float value)
    {
        currentHealth = value;
        SetHealthUI(currentHealth);
    }
}
