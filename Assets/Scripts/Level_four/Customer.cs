using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Customer : MonoBehaviour, IPointerClickHandler
{
    public enum Action { READ, WRITE };

    public PlanFile planFileModel;
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI countdownText;

    private PlanFile planFile;
    private Action action;
    private ControllerLevelFour controller;

    private bool counting = false;
    private float customerWaitTime; 
    private float leftTime; // Time for the customer
    private bool planFileBeingFetched = false;

    void Awake()
    {
        controller = ControllerLevelFour.Instance;

        if (controller == null)
        {
            Debug.LogError("ControllerLevelFour instance not found in scene.");
        }
    }

    void Update()
    {
        if (counting)
        {
            if (leftTime > 0)
            {
                if (controller.IsTutorial()) return;
                leftTime -= Time.deltaTime;
                countdownText.text = Mathf.Round(leftTime).ToString();
            }
            else
            {
                // If customer's waiting time finished, but the file isn't start fetching yet
                if (!planFileBeingFetched)
                {
                    controller.ComputeError(ControllerLevelFour.ErrorType.TIMEOUT);
                }
            }
        }
    }

    public PlanFile GetPlanFile()
    {
        return this.planFile;
    }

    public Action GetAction()
    {
        return this.action;
    }

    public void SetActive()
    {
        gameObject.SetActive(true);
    }

    public void Init(bool initialHasPriority, Action initialAction)
    {
        if (this.controller == null)
        {
            this.controller = ControllerLevelFour.Instance;
        }

        if (this.controller == null)
        {
            Debug.LogError("ControllerLevelFour instance is still null in Init()");
            return;
        }

        this.planFile = Instantiate(
            this.planFileModel,
            this.planFileModel.transform.position,
            this.planFileModel.transform.rotation
        );

        this.planFile.Initialize(initialHasPriority);
        this.action = initialAction;

        if (this.action == Action.READ)
        {
            this.customerWaitTime = this.planFile.GetHasPriority() ? 5f : 10f;
        }

        this.leftTime = customerWaitTime; 
        this.infoText.text = "Olá, gostaria de " + (this.action == Action.READ ? "LER" : "ESCREVER")
            + " um plano de voo com prioridade: " + (this.planFile.GetHasPriority() ? "ALTA" : "BAIXA");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // just the first customer can be clicked
        if (this != controller.GetFirstCustomerOfQueue()) {
            return;
        };

        PlanFile selectedPlanFile = controller.GetSelectedPlanFile();
        if (selectedPlanFile != null)
        {
            selectedPlanFile.OriginalSize();
        }

        if (controller.GetCurrentCustomer() == this)
        {
            if (selectedPlanFile != null)
            {
                selectedPlanFile.OriginalSize();

                if (this.action == Action.WRITE)
                {
                    controller.ComputeError(ControllerLevelFour.ErrorType.READ_WHEN_MUST_WRITE);
                    return;
                }

                if(selectedPlanFile.GetHasPriority() != this.planFile.GetHasPriority())
                {
                    controller.ComputeError(ControllerLevelFour.ErrorType.DIFFERENT_PRIORITY);
                    return;
                }

                selectedPlanFile.StartFetching();
                this.planFileBeingFetched = true;
                StartCoroutine(FetchPlanFile());
            }
        }
        else
        {
            controller.SetCurrentCustomer(this);
            this.infoPanel.SetActive(true);

            if(this.action == Action.READ)
            {
               this.counting = true;
            }
        }
        if(controller.GetStep() % 2 == 0)
        {
            controller.NextStepTutorial();
        }
    }

    private IEnumerator FetchPlanFile()
    {
        controller.HiddenTutorial();

        PlanFile file = controller.GetSelectedPlanFile();
        // Wait for the recovery time of the plan file
        yield return new WaitForSeconds(file.GetTimeToFetch());

        controller.ShowTutorial();

        // Once the plan file is ready, check if the customer is still waiting
        if (leftTime > 0)
        {
            // Plan file is ready before the customer time finishes
            Destroy(file.gameObject);
            controller.ComputeSuccess();
        }
        else
        {
            // Customer's waiting time finished before the plan file is fetched
            file.Reset();
            controller.ComputeError(ControllerLevelFour.ErrorType.TIMEOUT);
        }
    }
}
