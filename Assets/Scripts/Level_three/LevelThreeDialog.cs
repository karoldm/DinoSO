using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelThreeDialog : MonoBehaviour
{
    public enum DialogType
    {
        award,
        intro,
        none,
    }

    private static string[] awardTexts =
        {
          "Conquista desbloqueada: Mestre em comunica��es!",
          "Parab�ns! Voc� est� sempre garantindo que as coisas estejam no lugar correto e se comuniquem da melhor forma poss�vel!"
        };
    private LinkedList<string> awardDialog = new LinkedList<string>(awardTexts);


    private static string[] introTexts =
     {
        "Bem vindo a uma das partes mais agitadas do nosso aeroporto! Aqui � onde nossos avi�es decolam, pousam ou fazem paradas. Tudo aqui � gerenciado pela torre de controle e ela garante que os pilotos conduzam os avi�es para a pista correta.",
        "Aqui temos diversos tipos de avi�es: de passeios, de carga, particulares e comerciais. Cada um � indicado por uma cor (sendo elas verde, azul, amarelo e vermelho), desse modo, cada pista possui uma bandeira da mesma cor para indicar ao piloto que � ali que o avi�o dele deve pousar.",
        "Para n�o haver problemas de comunica��o com nossos funcion�rios, os pilotos devem vestir um uniforme da mesma cor que a bandeira, indicando o tipo de avi�o que ele est� pilotando. Desse modo, um piloto com uniforme azul, so pode pousar ou decolar da pista indicada pela bandeira azul.",
        "Toda vez que um piloto precisa utilizar a pista, ele deve se comunicar com a torre de controle por meio de chamados de radio. A nossa torre ir� analisar qual funcion�rio vai atender a solicita��o e enviar um chamado para o mesmo na pista.",
        "Cada funcion�rio demora uma quantidade diferente de tempo para executar um chamado, e esse tempo � indicado logo abaixo de cada um no nosso painel.",
        "Vale lembrar que os v�os possuem prioridade por chamado, por exemplo, um chamado de pouso de emerg�ncia com certeza tem uma prioridade maior do que um chamado de permiss�o para decolar.",
        "Veja, temos v�rios avi�es chegando no momento, que tal tentar gerenciar nossos funcionarios e atender os chamados? Lmebre-se que o avi�o deve estar na pista correta para ser atendido, voc� coloc�-lo na pista correta trocando eles de lugar com um clique (clique em um avi�o e depois clique em outro para trocar eles de lugares). Avi�es em processo de atendimento n�o podem ser trocados!",
        "Estando na posi��o correta voc� pode atribuir um chamado a um funcion�rio. Fa�a isso clicando no chamado e depois no Dino que voc� quer que seja respons�vel. Um Dino s� pode atender um chamado por vez.",
        "Lembre-se de dar uma aten��o especial para os chamados de alta prioridade e boa sorte!"
    };
    private LinkedList<string> introDialog = new LinkedList<string>(introTexts);

    private static string[] NoneTexts =
        {
            "As coisas parecem meio fora de ordem por aqui...",
        };
    private LinkedList<string> NoneDialog = new LinkedList<string>(NoneTexts);

    public Button button;
    public TextMeshProUGUI dialogText;

    private LinkedList<string> currentDialog;
    private LinkedListNode<string> currentNode;

    LevelThreeController controller;

    private static LevelThreeDialog instance;
    public static LevelThreeDialog Instance => instance;

    void Awake()
    {
        controller = LevelThreeController.Instance;

        if (controller == null)
        {
            Debug.LogError("LevelThreeController instance not found in the scene.");
        }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void nextText()
    {

        if (this.currentNode != null)
        {
            this.buildText();
            this.currentNode = this.currentNode.Next;
        }
        else
        {
            this.hidden();
        }
    }

    private void buildText()
    {
        if (this.currentNode != null && this.dialogText != null)
        {
            this.dialogText.text = this.currentNode.Value;
        }
    }

    private void show()
    {
        gameObject.SetActive(true);
    }

    private void hidden()
    {
        gameObject.SetActive(false);
    }

    public void showDialog(DialogType type)
    {
        switch (type)
        {
            case DialogType.award:
                this.currentDialog = this.awardDialog; break;
            case DialogType.intro:
                this.currentDialog = this.introDialog; break;
            default:
                this.currentDialog = this.NoneDialog; break;
        }

        if (this.currentDialog != null)
        {
            this.currentNode = this.currentDialog.First;
            this.nextText();
            this.show();
        }
    }
}