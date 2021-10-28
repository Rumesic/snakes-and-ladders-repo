using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DicePhysics : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] Transform mesh;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI diceText;
    Rigidbody rb;
    BoxCollider col;

    [Header("Settings")]
    [SerializeField] float gravity = 9.81f;
    [SerializeField] Vector3 gravityDirection = new Vector3(0, 0, 1);
    [SerializeField] float mult = 2;
    [SerializeField] float maxPos = 110;
    [SerializeField] float minPos = 90;
    [SerializeField] bool drawGizmos = true;


    [Header("Debug")]
    Vector3 targetPos;
    [SerializeField] float velMag;
    [SerializeField] public int diceResult;
    bool dragActive;
    List<Vector3> bounds = new List<Vector3>();
    List<Vector3> boundsWorld = new List<Vector3>();
    Vector2 screenPos;
    Vector3 worldPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        CalculateBounds();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DiceBehaviour();
    }

    void DiceBehaviour()
    {
        if(GameManager.Instance.canRoll)
        {
            DragInput();
            diceText.gameObject.active = dragActive || GameManager.Instance.waitingForRoll ? false : true;
        }

        
        diceResult = DetermineDiceRoll();
        velMag = rb.angularVelocity.sqrMagnitude;
        FindBounds();
        Drag();
        Drop();
        Gravity();
        FakeDistance();
    }

    void DragInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            screenPos = mousePos;
        }
        else if (Input.touchCount > 0)
        {
            screenPos = Input.GetTouch(0).position;
        }
        else
        {
            dragActive = false;
            return;
        }

        worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        ResetVelocity();
        dragActive = true;
        GameManager.Instance.waitingForRoll = true;
    }


    void Gravity()
    {
        if(!dragActive)
        {
            rb.AddForce(gravityDirection * (gravity * mult));
        }

    }

    void CalculateBounds()
    {
        Vector3 size = col.size;

        Vector3 xPositive = new Vector3(size.x, 0, 0);
        Vector3 xNegative = new Vector3(-size.x, 0, 0);

        Vector3 yPositive = new Vector3(0, size.y, 0);
        Vector3 yNegative = new Vector3(0, -size.y, 0);

        Vector3 zPositive = new Vector3(0, 0, size.z);
        Vector3 zNegative = new Vector3(0, 0, -size.z);

       
        bounds.Add(xNegative); //Roll 1
        bounds.Add(yPositive); //Roll 2
        bounds.Add(zNegative); //Roll 3
        bounds.Add(zPositive); //Roll 4
        bounds.Add(yNegative); //Roll 5
        bounds.Add(xPositive); //Roll 6


        foreach (Vector3 pos in bounds)
            boundsWorld.Add(pos);
    }

    void FindBounds()
    {
        for(int i = 0; i < bounds.Count; i++)
        {
            boundsWorld[i] = transform.TransformPoint(bounds[i]);
        }
    }

    int DetermineDiceRoll()
    {
        int index = -1;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = Camera.main.transform.position;

        for(int i = 0; i < boundsWorld.Count; i ++)
        {
            float dist = Vector3.Distance(boundsWorld[i], currentPos);
            if (dist < minDist)
            {
                index = i;
                minDist = dist;
            }
        }
        return index +1;
    }


    void ResetVelocity()
    {
        if(!dragActive)
            rb.velocity = Vector3.zero;
    }    

    void Drag()
    {
        if (dragActive)
        {
            targetPos = new Vector3(worldPos.x, worldPos.y, minPos);

            Vector3 difference = targetPos - transform.position;
            transform.DOMove(targetPos, 0.15f);
            rb.AddTorque((targetPos - transform.position) * 10);
        }
    }
    void Drop()
    {
        if(!dragActive && GameManager.Instance.waitingForRoll)
        {
            if (velMag < 0.1f)
                GameManager.Instance.RollDice(diceResult);
        }
    }

    void FakeDistance()
    {
        float InverseLerp = Mathf.InverseLerp(maxPos, minPos, transform.position.z);
        Vector3 targetScale = new Vector3(InverseLerp * 2, InverseLerp * 2, InverseLerp * 2);
        canvas.transform.position = transform.position;
        mesh.localScale = targetScale;
        canvas.transform.localScale = targetScale;
    }


    private void OnDrawGizmos()
    {
        if(drawGizmos == true)
        {
            for (int i = 0; i < boundsWorld.Count; i++)
            {
                Gizmos.color = (i == diceResult - 1) ? Color.green : Color.red;
                Gizmos.DrawSphere(boundsWorld[i], 0.2f);
            }
        }
    }

}
