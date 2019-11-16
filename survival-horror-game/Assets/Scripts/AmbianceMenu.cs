using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceMenu : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Lightning());
    }
    
    private IEnumerator Lightning()
    {
        yield return new WaitForSeconds(1);
        thunder.StartLoopStrike(8, 12);
    }

    public Thunder thunder;
}