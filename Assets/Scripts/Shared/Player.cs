using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    public List<Tilemap> tilemapObjects = new List<Tilemap>();
    public Tilemap tilemapProcessPortal;
    public Tilemap tilemapInitialPortal;
    public Tilemap tilemapRAMPortal;

    public DialogInitial initialDialog;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;  

            input.Normalize();

            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.y);

            var targetPosition = transform.position;
            targetPosition.x += input.x;
            targetPosition.y += input.y;

            bool collision = false;

            for(int i = 0; i <  tilemapObjects.Count; i++)

            {
                if(tilemapObjects[i] != null)
                {
                    Vector3Int obstacleMap = tilemapObjects[i].WorldToCell(targetPosition);

                    if (tilemapObjects[i].GetTile(obstacleMap) != null)
                    {
                        collision = true;
                        break;
                    }
                }
            }

            if (input != Vector2.zero && !collision)
            {
                StartCoroutine(Move(targetPosition));
            }


            CheckPortal(targetPosition);
        }

        animator.SetBool("IsMoving", isMoving); 
    }

    private void CheckPortal(Vector3 targetPosition)
    {
        if (tilemapProcessPortal != null)
        {
            Vector3Int obstacleMap = tilemapProcessPortal.WorldToCell(targetPosition);

            if (tilemapProcessPortal.GetTile(obstacleMap) != null)
            {
                initialDialog.showDialog(DialogInitial.InitialDialogType.process);
                initialDialog.SetCurrentSceneType(DialogInitial.InitialDialogType.process);
            }

        }

        if (tilemapRAMPortal != null)
        {
            Vector3Int obstacleMap = tilemapRAMPortal.WorldToCell(targetPosition);

            if (tilemapRAMPortal.GetTile(obstacleMap) != null)
            {
                initialDialog.showDialog(DialogInitial.InitialDialogType.RAM);
                initialDialog.SetCurrentSceneType(DialogInitial.InitialDialogType.RAM);
            }

        }

        if (tilemapInitialPortal != null)
        {
            Vector3Int obstacleMap = tilemapInitialPortal.WorldToCell(targetPosition);

            if (tilemapInitialPortal.GetTile(obstacleMap) != null)
            {
                SceneManager.LoadScene("Initial");
            }

        }
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;

        while((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

        isMoving = false;
    }
}