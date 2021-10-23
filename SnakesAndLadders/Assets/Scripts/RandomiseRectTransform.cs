using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseRectTransform : MonoBehaviour
{
    RectTransform rTransform;

    [SerializeField] float horizontalMax;
    [SerializeField] float verticalMax;

    [SerializeField] Vector3 randomTargetPosition;
    [SerializeField] Vector3 refVel;
    [SerializeField] float smoothTime;

    [SerializeField] float refreshAfter;

    Vector3 startAnchoredPosition;
    float t;
    // Start is called before the first frame update

    private void Awake()
    {
        rTransform = GetComponent<RectTransform>();
        t = 0;
        startAnchoredPosition = rTransform.anchoredPosition;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (t >= refreshAfter)
            GetRandom();


        Vector3 currentPos = Vector3.SmoothDamp(rTransform.anchoredPosition, VectorAddition(startAnchoredPosition, randomTargetPosition), ref refVel, smoothTime);
        rTransform.localPosition = currentPos;
    }

    void GetRandom()
    {
        t = 0;
        randomTargetPosition = new Vector3(Random.Range(-horizontalMax, horizontalMax), Random.Range(-verticalMax, verticalMax), 0);
    }


    Vector3 VectorAddition(Vector3 a, Vector3 b)
    {
        Vector3 result = new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        return result;
    }
}
