using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    Vector3 offset;

    void Awake() => instance = this;

    void Start() => offset = transform.position;

    int startSteps;
    public IEnumerator Follow()
    {   
        if(startSteps > 2)
        {
            Vector3 movePosition = new Vector3(offset.x, offset.y + Vector3.up.y, offset.z);

            transform.DOMove(movePosition, 0.3f);

            yield return new WaitForSeconds(0.3f);
            offset = transform.position;
        } 
        else startSteps++;
    }
}
