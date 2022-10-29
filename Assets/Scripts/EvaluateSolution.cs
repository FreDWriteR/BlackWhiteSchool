using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateSolution : MonoBehaviour
{
    // Start is called before the first frame update

    public IEnumerator EvaluateXPoint(Vector3 StartX, Vector3 EndX, float centerOffset)
    { //Движение правильного варианта


        var center = (StartX + EndX) * 0.5f;
        center -= new Vector3(0, centerOffset);

        var StartRelativeX = StartX - center;
        var EndRelatixeX = EndX - center;

        var f = 1f / 50f;

        for (var i = 0f; i < 1 + f; i += f)
        {
            gameObject.transform.position = Vector3.Slerp(StartRelativeX, EndRelatixeX, i) + center;
            yield return new WaitForSeconds(0.0005f);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
