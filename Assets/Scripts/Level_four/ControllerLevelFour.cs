using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class ControllerLevelFour : UserController
{
    public VerticalLayoutGroup dinoQueue;
    public TextMeshProUGUI pointsText;
    public Customer modelDino;
    public DialogLevelFour dialog;
    public TextMeshProUGUI timeText;
    public GameObject timeContainer;
    public GameObject startButton;

    private float leftTime = 60f;
    public Award awardSecMemory;
    private int points = 0;
    private bool hasError = false;
    private bool gameOver = true;

    private Customer currentCustomer;
    private PlanFile selectedPlanFile;

    public GridLayoutGroup swapArea;
    public GridLayoutGroup secondMemoryArea;


    private static ControllerLevelFour instance;


    public static ControllerLevelFour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ControllerLevelFour>();

                if (instance == null)
                {
                    Debug.LogError("No instance of ControllerLevelFour found in the scene.");
                }

            }
            return instance;
        }
    }

    void Start()
    {
        this.dialog.showDialog(DialogLevelFour.DialogType.intro);

        if (user.levelFour.awards.Contains("SECMEMORY"))
        {
            awardSecMemory.Unlock();
        }

        this.pointsText.text = user.levelFour.score.ToString();
    }

    void Update()
    {
        if (!gameOver)
        {
            if (leftTime > 0)
            {
                leftTime -= Time.deltaTime;
                timeText.text = Mathf.Round(leftTime).ToString();
            }
            else
            {
                FinishGame();
            }
        }
    }


    private void FinishGame()
    {
        gameOver = true;
        user.levelFour.score = points;
        this.leftTime = 60f;
        this.timeText.text = "";
        timeContainer.SetActive(false);
        startButton.SetActive(true);
        ClearQueue(1, dinoQueue);
        ClearQueue(0, swapArea);
        ClearQueue(0, secondMemoryArea);

        if (!hasError && awardSecMemory.IsLocked())
        {
            dialog.showDialog(DialogLevelFour.DialogType.award);
            awardSecMemory.Unlock();
            if (!user.levelFour.awards.Contains("SECMEMORY"))
            {
                user.levelFour.awards.Add("SECMEMORY");
            }
        }
        UpdateUser();
    }

    private void ClearQueue(int initialIndex, LayoutGroup queue)
    {
        for (int i = initialIndex; i < queue.transform.childCount; i++)
        {
            Destroy(queue.transform.GetChild(i).gameObject);
        }
    }


    public void InitGame()
    {
        timeContainer.SetActive(true);
        startButton.SetActive(false);
        this.gameOver = false;
        this.pointsText.text = "0";
        this.points = 0;
        this.AddDino();
        this.AddDino();
        this.AddDino();
        this.AddDino();
    }

    public void LoadInitialScene()
    {
        SceneManager.LoadScene("Home");

    }

    private void AddDino()
    {
        Debug.Log("add dino");

        if (dinoQueue == null)
        {
            Debug.LogError("dinoQueue is null");
            return;
        }

        Customer dino = Instantiate(modelDino, dinoQueue.transform);
        dino.Init();
        dino.SetActive();
        UpdateQueue();
    }

    private void UpdateQueue()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(dinoQueue.GetComponent<RectTransform>());
    }

    public Customer GetFirstCustomerOfQueue()
    {
        if (QueueSize() <= 0) return null;

        Transform firstChildTransform = dinoQueue.transform.GetChild(1);

        return firstChildTransform.GetComponent<Customer>();
    }

    private void RemoveFirstDino()
    {
        if (QueueSize() <= 0) return;

        Destroy(dinoQueue.transform.GetChild(1).gameObject);
        UpdateQueue();

        SetCurrentCustomer(null);
        SetSelectedPlanFile(null);

        if (!this.gameOver)
        {
            AddDino();
        }
    }

    public void SetCurrentCustomer(Customer customer)
    {
        this.currentCustomer = customer;
    }

    public Customer GetCurrentCustomer()
    {
        return this.currentCustomer;
    }

    public void SetSelectedPlanFile(PlanFile file)
    {
        this.selectedPlanFile = file;
    }

    public PlanFile GetSelectedPlanFile()
    {
        return this.selectedPlanFile;
    }

    public void ComputeError()
    {
        Debug.Log("ComputeError");
        this.points--;
        pointsText.text = points.ToString();
        this.RemoveFirstDino();
        hasError = true;
    }

    public void ComputeSuccess()
    {
        Debug.Log("ComputeSuccess");

        this.points++;
        pointsText.text = points.ToString();
        this.RemoveFirstDino();
    }

    private int QueueSize()
    {
        return dinoQueue.transform.childCount;
    }

    public bool FileWithPriorityExist()
    {
        foreach (Transform child in this.swapArea.transform)
        {
            if (child.GetComponent<PlanFile>()?.GetHasPriority() == true)
                return true;
        }

        foreach (Transform child in this.secondMemoryArea.transform)
        {
            if (child.GetComponent<PlanFile>()?.GetHasPriority() == true)
                return true;
        }

        return false;
    }

    public bool FileWithNoPriorityExist()
    {
        foreach (Transform child in this.swapArea.transform)
        {
            if (child.GetComponent<PlanFile>()?.GetHasPriority() == false)
                return true;
        }

        foreach (Transform child in this.secondMemoryArea.transform)
        {
            if (child.GetComponent<PlanFile>()?.GetHasPriority() == false)
                return true;
        }

        return false;
    }
}
