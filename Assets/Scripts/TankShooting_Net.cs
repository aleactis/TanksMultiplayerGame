using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TankShooting_Net : NetworkBehaviour
{
    [SerializeField] float power = 800f; //Quão difícil atirar com as balas de canhão
    [SerializeField] Transform gunBarrel; //Onde as armas disparam
    [SerializeField] public GameObject shell; //Prefab shell

    [SerializeField]
    public AudioSource m_ShootingAudio; //Referência ao audio source usado para o disparo das balas. Nota: Diferente do Audio Source do movimento do tanque
    [SerializeField]
    public AudioClip m_FireClip; //Audio que tocará quando cada tiro for disparado




    public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

    private string m_FireButton;                // The input axis that is used for launching shells.
    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    private void Start()
    {
        // The fire axis is based on the player number.
        m_FireButton = "Fire1";

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    void Reset()
    {
        //Pega a localização do canhão (usamos código lento (Run Slow Code). Funciona como
        //um "localizar" no Editor, onde há impacto no desempenho e pode afetar os jogadores na execução
        //gunBarrel = transform.Find("GunBarrel");

        //Pega a localização do canhão (código recomendado pela Unity)
        //O GameObject.FindWithTag retorna um GameObject e temos um transform.
        //Felizmente, GameObjects sempre possuem um transform. 
        //Quando acessamos o transform do GameObject etiquetado "GunBarrel", e não
        //apenas tentando salvar a referência ao GameObject em si.
        gunBarrel = GameObject.FindWithTag("GunBarrel").transform;
    }

    //[ClientCallback]
    void Update()
    {
        //Se não for o Local Player, retorne. Note que não podemos remover este script
        // de non-Local players, assim como fizemos com o Script TankController. 
        //A razão para isso é que o script tem um comando (CmdSpawnShell) que precisa
        //existir neste objeto mesmo que ele não seja o Local Player
        if (!isLocalPlayer)
            return;

        //The slider should have a default value of the minimum launch force.
        m_AimSlider.value = m_MinLaunchForce;

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // ... use the max force and launch the shell.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            CmdFire();
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // Change the clip to the charging clip and start it playing.
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton("Fire1") || Input.GetButton("Jump") && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp(m_FireButton) || Input.GetButtonUp("Jump") && !m_Fired)
        {
            // ... launch the shell.
            CmdFire();
        }
    }

    //Este comando é chamado do LocalPlayer e executado no servidor. Note que
    // o comando precisa começar com 'Cmd'
    [Command]
    void CmdFire()
    {
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        //Este comando é chamado do Local Player e executado no servidor.
        //Instanciar uma arma no cano do tanque com a posição e rotação corretas
        GameObject shellInstance = Instantiate(shell, gunBarrel.position,
            gunBarrel.rotation) as GameObject;

        //Localizar o componente RigidBody da arma e adicionarmos uma força física à ele
        shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * gunBarrel.forward;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;

        //Instanciar este objeto na rede para que todos os players o vejam
        NetworkServer.Spawn(shellInstance);

        //Destrói as balas de canhão
        Destroy(shellInstance, 2.0f);
    }
}


/*
public class TankShooting_Net : NetworkBehaviour
{
    [SerializeField] float power = 800f; //Quão difícil atirar com as balas de canhão
    [SerializeField] Transform gunBarrel; //Onde as armas disparam
    [SerializeField] public GameObject shell; //Prefab shell

    [SerializeField]
    public AudioSource m_ShootingAudio; //Referência ao audio source usado para o disparo das balas. Nota: Diferente do Audio Source do movimento do tanque
    [SerializeField]
    public AudioClip m_FireClip; //Audio que tocará quando cada tiro for disparado

    void Reset()
    {
        //Pega a localização do canhão (usamos código lento (Run Slow Code). Funciona como
        //um "localizar" no Editor, onde há impacto no desempenho e pode afetar os jogadores na execução
        //gunBarrel = transform.Find("GunBarrel");

        //Pega a localização do canhão (código recomendado pela Unity)
        //O GameObject.FindWithTag retorna um GameObject e temos um transform.
        //Felizmente, GameObjects sempre possuem um transform. 
        //Quando acessamos o transform do GameObject etiquetado "GunBarrel", e não
        //apenas tentando salvar a referência ao GameObject em si.
        gunBarrel = GameObject.FindWithTag("GunBarrel").transform;
    }

    void Update()
    {
        //Se não for o Local Player, retorne. Note que não podemos remover este script
        // de non-Local players, assim como fizemos com o Script TankController. 
        //A razão para isso é que o script tem um comando (CmdSpawnShell) que precisa
        //existir neste objeto mesmo que ele não seja o Local Player
        if (!isLocalPlayer)
        {
            return;
        }

        //Se nós clicarmos no mouse,  tocar na tela ou apertar o spacebar
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            //Roda o comando no servidor para gerar uma arma
            CmdSpawnShell();
        }
    }

    //Este comando é chamado do LocalPlayer e executado no servidor. Note que
    // o comando precisa começar com 'Cmd'
    [Command]
    void CmdSpawnShell()
    {
        //Nós instanciamos uma arma no cano com a rotação correta
        GameObject instance = Instantiate(shell, gunBarrel.position,
            gunBarrel.rotation) as GameObject;
        
        //Localizamos o componente RigidBody da arma e adicionamos uma força direta para ele
        instance.GetComponent<Rigidbody>().AddForce(gunBarrel.forward * power);

        //Finalmente, vamos instanciar este objeto na rede para que todos os jogadores visualizem
        NetworkServer.Spawn(instance);

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    [ClientRpc]
    void RpcTeste()
    {
        //Nós instanciamos uma arma no cano com a rotação correta
        GameObject instance = Instantiate(shell, gunBarrel.position,
            gunBarrel.rotation) as GameObject;

        //Localizamos o componente RigidBody da arma e adicionamos uma força direta para ele
        instance.GetComponent<Rigidbody>().AddForce(gunBarrel.forward * power);

        //Finalmente, vamos instanciar este objeto na rede para que todos os jogadores visualizem
        NetworkServer.Spawn(instance);

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}






   [SerializeField] float power = 800f; //Quão difícil atirar com as balas de canhão
    [SerializeField] Transform gunBarrel; //Onde as armas disparam
    [SerializeField] public GameObject shell; // Prefab of the shell.


    //O método Reset() usamos o código lento (Run Slow Code), como "localizar" no 
    //Editor onde o impacto no desempenho não afetará os jogadores na execução
    void Reset()
    {
        //Pega a localização do canhão
        gunBarrel = transform.Find("GunBarrel");
    }

    void Update()
    {
        //Se não for o Local Player, retorne. Note que não podemos remover este script
        // de non-Local players, assim como fizemos com o Script TankController. 
        //A razão para isso é que o script tem um comando (CmdSpawnShell) que precisa
        //existir neste objeto mesmo que ele não seja o Local Player
        if (!isLocalPlayer)
            return;

        //Se nós clicarmos no mouse,  tocar na tela ou apertar o spacebar
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            //Roda o comando no servidor para gerar uma arma
            CmdSpawnShell();
            //m_ShootingAudio.clip = m_FireClip;
            // m_ShootingAudio.Play();
        }
    }

    //Este comando é chamado do LocalPlayer e executado no servidor. Note que
    // o comando precisa começar com 'Cmd'
    [Command]
    void CmdSpawnShell()
    {
        //Nós instanciamos uma arma no cano da arma com a rotação correta
        GameObject instance = Instantiate(shell, gunBarrel.position,
            gunBarrel.rotation) as GameObject;
        //Localizamos o componente RigidBody da arma e adicionamos uma força direta para ele
        instance.GetComponent<Rigidbody>().AddForce(gunBarrel.forward * power);

        //Finalmente, vamos instanciar este objeto na rede para que todos os jogadores visualizem
        NetworkServer.Spawn(instance);
    }*/


//public int m_PlayerNumber = 1;              // Used to identify the different players.
//public GameObject m_Shell;                   // Prefab of the shell.
//                                             //public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
//public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
//public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
//public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
//public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
//public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
//public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.


//private string m_FireButton;                // The input axis that is used for launching shells.
//private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
//private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
//private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

//[SerializeField] float power = 800f; //Quão difícil atirar com as balas de canhão
//[SerializeField] Transform gunBarrel; //Onde as armas disparam

//void Reset()
//{
//    //Pega a localização do canhão
//    gunBarrel = transform.Find("GunBarrel");
//}

//private void OnEnable()
//{
//    // When the tank is turned on, reset the launch force and the UI
//    m_CurrentLaunchForce = m_MinLaunchForce;
//    //m_AimSlider.value = m_MinLaunchForce;
//}


//private void Start()
//{
//    // The fire axis is based on the player number.
//    m_FireButton = "Fire1";

//    // The rate that the launch force charges up is the range of possible forces by the max charge time.
//    m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
//}


//private void Update()
//{
//    if (!isLocalPlayer)
//        return;

//    // The slider should have a default value of the minimum launch force.
//    //m_AimSlider.value = m_MinLaunchForce;

//    // If the max force has been exceeded and the shell hasn't yet been launched...
//    if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
//    {
//        // ... use the max force and launch the shell.
//        m_CurrentLaunchForce = m_MaxLaunchForce;
//        CmdFire();
//    }
//    // Otherwise, if the fire button has just started being pressed...
//    else if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
//    {
//        // ... reset the fired flag and reset the launch force.
//        m_Fired = false;
//        m_CurrentLaunchForce = m_MinLaunchForce;

//        // Change the clip to the charging clip and start it playing.
//        m_ShootingAudio.clip = m_ChargingClip;
//        m_ShootingAudio.Play();
//    }
//    // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
//    else if (Input.GetButton("Fire1") || Input.GetButton("Jump") && !m_Fired)
//    {
//        // Increment the launch force and update the slider.
//        m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

//        // m_AimSlider.value = m_CurrentLaunchForce;
//    }
//    // Otherwise, if the fire button is released and the shell hasn't been launched yet...
//    else if (Input.GetButtonUp(m_FireButton) || Input.GetButtonUp("Jump") && !m_Fired)
//    {
//        // ... launch the shell.
//        CmdFire();
//    }
//}

//[Command]
//void CmdFire()
//{
//    // Set the fired flag so only Fire is only called once.
//    m_Fired = true;

//    GameObject shellInstance = Instantiate(m_Shell, gunBarrel.position,
//        gunBarrel.rotation) as GameObject;

//    shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * gunBarrel.forward;

//    // Change the clip to the firing clip and play it.
//    m_ShootingAudio.clip = m_FireClip;
//    m_ShootingAudio.Play();

//    // Reset the launch force.  This is a precaution in case of missing button events.
//    m_CurrentLaunchForce = m_MinLaunchForce;

//    NetworkServer.Spawn(shellInstance);
//}