using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AirplanePeriferic : MonoBehaviour, IPointerClickHandler    
{
    private LevelThreeController controller;

    private int timeToExecute;
    private int score;
    private Priority.PriorityEnum priority;
    private VerticalLayoutGroup queue;
    public string color;
    private bool busy = false;
    private bool isInCorrectQeueu;
    public VerticalLayoutGroup correctQueue;
    public VerticalLayoutGroup priorityRenderer;

    void Awake()
    {

        controller = LevelThreeController.Instance;

        if (controller == null)
        {
            Debug.LogError("LevelThreeController instance n�o encontrado na cena.");
        }
    }

    public void SetQueue(VerticalLayoutGroup queue)
    {
        this.queue = queue;
    }

    public bool GetCorrectQueue()
    {
        return this.isInCorrectQeueu;
    }

    public int GetScore()
    {
        return this.score;
    }

    void Start()
    {
        
    }

    public Priority.PriorityEnum GetPriority()
    {
        return this.priority;
    }

    void Update()
    {
        

    }

    public void SetBusy()
    {
        this.busy = true;
    }

    public VerticalLayoutGroup GetQueue()
    {
        return this.queue;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (busy)
        {
            controller.firstSelected = null;
            return;
        }

        int index = GetIndexOfChild();

        if (index == -1 || index != 0) return;

        if (controller.firstSelected != null && controller.firstSelected != this)
        {
            controller.secondSelected = this;
            controller.Swap();
            this.isInCorrectQeueu = this.queue == correctQueue;
        }
        else
        {
            controller.firstSelected = this;
        }
    }

    int GetIndexOfChild()
    {
        Transform parentTransform = this.queue.transform;
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            if (parentTransform.GetChild(i).gameObject == gameObject)
            {
                return i;
            }
        }
        return -1;

    }

    private int GetRandInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    public void Instanciate(int airplaneIndex, VerticalLayoutGroup queue)
    {
        this.queue = queue;
        this.isInCorrectQeueu = queue == correctQueue;
        int score = GetRandInt(1, 4);
        this.score = score;
        this.priority = Priority.GetFromInt(score-1);

        UpdatePriorityRenderer();

        Show();
    }

    private void UpdatePriorityRenderer()
    {
        Transform transform = priorityRenderer.transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            SpriteRenderer sprite = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (sprite != null && (int)this.priority >= i)
            {
                sprite.color = Color.red;
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}