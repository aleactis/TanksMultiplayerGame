using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class TankHealth_DM : NetworkBehaviour
{
    [SerializeField] float m_StartingHealth = 100f;     //A quantidade de vida que cada tanque começa a partida
    public Slider m_Slider;                             //O slider para representar a quantidade de saúde que o tanque atualmente possui de vida
    public Image m_FillImage;                           //O componente Image do Slider
    public Color m_FullHealthColor = Color.green;       // A cor da barra de vida quando estiver cheio de vida
    public Color m_ZeroHealthColor = Color.red;         // A cor da barra de vida quando estiver morrendo
    public GameObject m_ExplosionPrefab;                //Um prefab que será instanciado em Awake, então usado sempre que o tanque morrer

    private AudioSource m_ExplosionAudio; //A fonte de áudio para jogar quando o tanque explode
    private ParticleSystem m_ExplosionParticles; //O sistema de partículas que irá jogar quando o tanque for destruído
    [SyncVar (hook = "OnHealthChanged")] float m_CurrentHealth; //Quanta vida o tanque tem atualmente
    private bool m_Dead; //O tanque foi morto?

    Text informationText; //Elemento texto que dará informações para o jogador

    void Start()
    {
        //Se isto for Servidor, chama o DeathMatchManager que este tanque gerou
        if (isServer)
            DeathMatchMannager1.AddTank(this);


        //Instancia o Prefab da explosão e pega uma referência para o sistema de partículas nele
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        //Pega uma referência para o audio source no prefab instanciado
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive(false);


        //Quando o tanque está habilitado, reinicia a vida do tanque e se está ou não morto
        m_CurrentHealth = m_StartingHealth;

        //Atualizar o slider de vida e cor
        SetHealthUI(m_CurrentHealth);

        m_Dead = false;
    }

    //Método para atualizar o slider de vida atual (UI)
    private void SetHealthUI(float value)
    {
        // Set the slider's value appropriately.
        m_Slider.value = value;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    public void TakeDamage(int amount)
    {
        //Dano será calculado somente no servidor. Isto evita que um cliente
        //tente hackear. Também, se este tanque já está morto, não precisa rodar
        if (!isServer || m_CurrentHealth <= 0)
            return;

        //Reduza a vida atual pela quantidade de dano causado
        m_CurrentHealth -= amount;

        //Atualizar o slider de vida e cor
        SetHealthUI(m_CurrentHealth);

        //Se o jogador está com energia zerada ou menor que zero
        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            m_CurrentHealth = 0;

            //Chama um método em todas as instâncias deste objeto em todos os 
            //clientes (Isto é chamdado de RPC)
            RpcDied();

            //Chamar o DeathMatchManager para este tanque morto
            if (DeathMatchMannager1.RemoveTankAndCheckWinner(this))
            {
                TankHealth_DM tank = DeathMatchMannager1.GetWinner();
                tank.RpcWon();

                //Assim que a partida acabou, o servidor trará os jogadores de volta
                //ao Lobby após 3 segundos
                Invoke("BackToLobby", 3f);
            }

            //Sair do método. Isto é utilizado caso tivermos efeitos de "ferido"
            //conforme abaixo. Não queremos os efeitos de "ferir" ao jogador quando
            //o tanque foi destruído. Por isso abandonamos o método e evitamos 
            //que estes efeitos sejam executados.
            return;
        }
        //Se tivermos algum efeito de "ferimento" quando o tanque é 
        //danificado, poderíamos executá-los aqui.
    }

    //Uma vez que o dano foi executado no servidor, se o código de um tanque 
    //que estava sendo destruído fosse executado lá, ele só seria visível na
    //máquina do servidor. Como queremos o mesmo tanque destruído em todos os
    //clientes para que todos os jogadores vejam o mesmo tanque destruído,
    //precisamos usar um RPC.
    [ClientRpc]
    void RpcDied()
    {
        // Set the flag so that this function is only called once.
        m_Dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        //Escurecer o tanque para mostrar que ele morreu
        GetComponent<TankCollor>().HideTank();

        //Se um tanque morreu e é o LocalPlayer, significa que eles 
        //perderam (desde que eles sejam um dos que morreram).
        if (isLocalPlayer)
        {
            //Localiza o objeto texto Game Over na tela
            informationText = GameObject.FindObjectOfType<Text>();
            informationText.color = Color.red;
            informationText.text = "Game Over";

            //Desabilitar as funções do tanque
            GetComponent<TankController_Net>().enabled = false;
            GetComponent<TankShooting_Net>().enabled = false;
        }
    }

    [ClientRpc]
    public void RpcWon()
    {
        //Se um tanque venceu e é o LocalPlayer, significa que eles venceram
        //(desde que eles não sejam um dos que morreram)
        if (isLocalPlayer)
        {
            //Localiza o objeto texto Game Over na tela
            informationText = GameObject.FindObjectOfType<Text>();
            informationText.color = Color.blue;
            informationText.text = "Game Win";
        }
    }

    void BackToLobby()
    {
        //Vai para o Lobby
        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    }

    void OnHealthChanged(float value)
    {
        m_CurrentHealth = value;
        SetHealthUI(m_CurrentHealth);
    }
}

//public int maxHealth = 3; //Energia máxima dos jogadores

//Text informationText; //Elemento texto que dará informações para o jogador
//int health; //Energia do jogador atual

//void Start()
//{
//    health = maxHealth; //Setar a energia do jogador

//    //Se isto for Servidor, chama o DeathMatchManager que este tanque gerou
//    if (isServer)
//        DeathMatchMannager.AddTank(this);
//}

//public void TakeDamage(int amount)
//{
//    //Dano será calculado somente no servidor. Isto evita que um cliente
//    //tente hackear. Também, se este tanque já está morto, não precisa rodar
//    if (!isServer || health <= 0)
//        return;

//    health -= amount;

//    //Se o jogador está com energia zerada ou menor que zero
//    if (health <= 0)
//    {
//        health = 0;

//        //Chama um método em todas as instâncias deste objeto em todos os 
//        //clientes (Isto é chamdado de RPC)
//        RpcDied();

//        //Chamar o DeathMatchManager para este tanque morto
//        if (DeathMatchMannager.RemoveTankAndCheckWinner(this))
//        {
//            TankHealth_DM tank = DeathMatchMannager.GetWinner();
//            tank.RpcWon();

//            //Assim que a partida acabou, o servidor trará os jogadores de volta
//            //ao Lobby após 3 segundos
//            Invoke("BackToLobby", 3f);
//        }

//        //Sair do método. Isto é utilizado caso tivermos efeitos de "ferido"
//        //conforme abaixo. Não queremos os efeitos de "ferir" ao jogador quando
//        //o tanque foi destruído. Por isso abandonamos o método e evitamos 
//        //que estes efeitos sejam executados.
//        return;
//    }
//    //Se tivermos algum efeito de "ferimento" quando o tanque é 
//    //danificado, poderíamos executá-los aqui.
//}

////Uma vez que o dano foi executado no servidor, se o código de um tanque 
////que estava sendo destruído fosse executado lá, ele só seria visível na
////máquina do servidor. Como queremos o mesmo tanque destruído em todos os
////clientes para que todos os jogadores vejam o mesmo tanque destruído,
////precisamos usar um RPC.
//[ClientRpc]
//void RpcDied()
//{
//    //Escurecer o tanque para mostrar que ele morreu
//    GetComponent<TankCollor>().HideTank();

//    //Se um tanque morreu e é o LocalPlayer, significa que eles 
//    //perderam (desde que eles sejam um dos que morreram).
//    if (isLocalPlayer)
//    {
//        //Localiza o objeto texto Game Over na tela
//        informationText = GameObject.FindObjectOfType<Text>();
//        informationText.text = "Game Over";

//        //Desabilitar as funções do tanque
//        GetComponent<TankController_Net>().enabled = false;
//        GetComponent<TankShooting_Net>().enabled = false;
//    }
//}

//[ClientRpc]
//public void RpcWon()
//{
//    //Se um tanque venceu e é o LocalPlayer, significa que eles venceram
//    //(desde que eles não sejam um dos que morreram)
//    if (isLocalPlayer)
//    {
//        //Localiza o objeto texto Game Over na tela
//        informationText = GameObject.FindObjectOfType<Text>();
//        informationText.text = "Você Venceu!";
//    }
//}

//void BackToLobby()
//{
//    //Vai para o Lobby
//    FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
//}


//    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
//    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
//    public Image m_FillImage;                           // The image component of the slider.
//    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
//    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
//    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


//    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
//    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
//    private float m_CurrentHealth;                      // How much health the tank currently has.
//    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?




//    public int maxHealth = 3; //Energia máxima dos jogadores

//    Text informationText; //Elemento texto que dará informações para o jogador
//    int health; //Energia do jogador atual

//    void Start()
//    {
//        m_CurrentHealth = maxHealth; //Setar a energia do jogador

//        //Se isto for Servidor, chama o DeathMatchManager que este tanque gerou
//        if (isServer)
//            DeathMatchMannager.AddTank(this);


//        // Instantiate the explosion prefab and get a reference to the particle system on it.
//        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

//        // Get a reference to the audio source on the instantiated prefab.
//        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

//        // Disable the prefab so it can be activated when it's required.
//        m_ExplosionParticles.gameObject.SetActive(false);


//        // When the tank is enabled, reset the tank's health and whether or not it's dead.
//        m_CurrentHealth = m_StartingHealth;
//        m_Dead = false;

//        // Update the health slider's value and color.
//        SetHealthUI();
//    }

//    private void SetHealthUI()
//    {
//        // Set the slider's value appropriately.
//        m_Slider.value = m_CurrentHealth;

//        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
//        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
//    }

//    public void TakeDamage(int amount)
//    {
//        //Dano será calculado somente no servidor. Isto evita que um cliente
//        //tente hackear. Também, se este tanque já está morto, não precisa rodar
//        if (!isServer || m_CurrentHealth <= 0)
//            return;

//        // Reduce current health by the amount of damage done.
//        m_CurrentHealth -= amount;

//        // Change the UI elements appropriately.
//        SetHealthUI();

//        //Se o jogador está com energia zerada ou menor que zero
//        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
//        if (m_CurrentHealth <= 0f && !m_Dead)
//        {
//            m_CurrentHealth = 0;

//            //Chama um método em todas as instâncias deste objeto em todos os 
//            //clientes (Isto é chamdado de RPC)
//            RpcDied();

//            //Chamar o DeathMatchManager para este tanque morto
//            if (DeathMatchMannager.RemoveTankAndCheckWinner(this))
//            {
//                TankHealth_DM tank = DeathMatchMannager.GetWinner();
//                tank.RpcWon();

//                //Assim que a partida acabou, o servidor trará os jogadores de volta
//                //ao Lobby após 3 segundos
//                Invoke("BackToLobby", 3f);
//            }

//            //Sair do método. Isto é utilizado caso tivermos efeitos de "ferido"
//            //conforme abaixo. Não queremos os efeitos de "ferir" ao jogador quando
//            //o tanque foi destruído. Por isso abandonamos o método e evitamos 
//            //que estes efeitos sejam executados.
//            return;
//        }
//        //Se tivermos algum efeito de "ferimento" quando o tanque é 
//        //danificado, poderíamos executá-los aqui.
//    }

//    //Uma vez que o dano foi executado no servidor, se o código de um tanque 
//    //que estava sendo destruído fosse executado lá, ele só seria visível na
//    //máquina do servidor. Como queremos o mesmo tanque destruído em todos os
//    //clientes para que todos os jogadores vejam o mesmo tanque destruído,
//    //precisamos usar um RPC.
//    [ClientRpc]
//    void RpcDied()
//    {
//        // Set the flag so that this function is only called once.
//        m_Dead = true;

//        // Move the instantiated explosion prefab to the tank's position and turn it on.
//        m_ExplosionParticles.transform.position = transform.position;
//        m_ExplosionParticles.gameObject.SetActive(true);

//        // Play the particle system of the tank exploding.
//        m_ExplosionParticles.Play();

//        // Play the tank explosion sound effect.
//        m_ExplosionAudio.Play();

//        //Escurecer o tanque para mostrar que ele morreu
//        GetComponent<TankCollor>().HideTank();

//        //Se um tanque morreu e é o LocalPlayer, significa que eles 
//        //perderam (desde que eles sejam um dos que morreram).
//        if (isLocalPlayer)
//        {
//            //Localiza o objeto texto Game Over na tela
//            informationText = GameObject.FindObjectOfType<Text>();
//            informationText.text = "Game Over";

//            //Desabilitar as funções do tanque
//            GetComponent<TankController_Net>().enabled = false;
//            GetComponent<TankShooting_Net>().enabled = false;
//        }
//    }

//    [ClientRpc]
//    public void RpcWon()
//    {
//        //Se um tanque venceu e é o LocalPlayer, significa que eles venceram
//        //(desde que eles não sejam um dos que morreram)
//        if (isLocalPlayer)
//        {
//            //Localiza o objeto texto Game Over na tela
//            informationText = GameObject.FindObjectOfType<Text>();
//            informationText.text = "Você Venceu!";
//        }
//    }

//    void BackToLobby()
//    {
//        //Vai para o Lobby
//        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
//    }
//}