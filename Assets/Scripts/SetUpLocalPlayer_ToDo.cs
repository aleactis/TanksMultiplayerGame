using UnityEngine;

//Adicionar a classe da Unet para trabalhar com Multiplayer
public class SetUpLocalPlayer_ToDo : MonoBehaviour
{
    //Criar uma variável públic do tipo string com o nome pname, inicializada como "Player"
    //e com o atributo [SyncVar]

    void OnGUI()
    {
        //Se for o LocalPlayer
        //pname receberá o GUI.TextField(new Rect (25, Screen.height - 40, 100, 30), pname)
        //Se (GUI.Button(new Rect(130, Screen.height - 40, 80, 30),"Change"))
            //Chama o método para ser executado no servidor CmdChangeName passando como parâmetro pname
    }

    public void ChangeName(string newName)
    {
        //pname recebe newName
        //Este objeto pega o componente filho do tipo TextMesh.text e recebe o conteúdo de pname
    }

    // Start is called before the first frame update
    void Start()
    {
        //Se for o LocalPlayer
            //Pega o componente (script de movimentação) e o habilita
            //Chama o script SmoothCameraFollow.target = this.transform
    }

    // Update is called once per frame
    void Update()
    {
        //Este objeto pega o componente filho TextMesh.text e recebe o conteúdo de pname
    }
}
