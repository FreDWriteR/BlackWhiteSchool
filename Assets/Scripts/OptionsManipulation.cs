using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManipulation : MonoBehaviour
{
    Color ColorOption;
    private Mesh OptionMeshInfo;
    private Vector3[] OptionVertices;
    public bool bEndMove = false, bEndChangeColor = false, bAway = false;

    public IEnumerator ChangeColor(float color1, float color2, float color3) //Изменение цвета поля после ответа игрока и перед возвращением в стартовое состояние
    {
        float c_olor1 = gameObject.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor").r;
        float c_olor2 = gameObject.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor").g;
        float c_olor3 = gameObject.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor").b;
        ColorOption = gameObject.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        yield return new WaitForSeconds(0.1f);
        float t = 0f;
        while (ColorOption.r != color1 || ColorOption.g != color2 || ColorOption.b != color3)
        {
            ColorOption.r = Mathf.Lerp(c_olor1, color1, t);
            ColorOption.g = Mathf.Lerp(c_olor2, color2, t);
            ColorOption.b = Mathf.Lerp(c_olor3, color3, t);

            gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", ColorOption);
            t += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        bEndChangeColor = true;
    }

    public IEnumerator MoveOption(Vector3 Point)
    {
        Vector3 StartPoint = gameObject.transform.position;
        Vector3 TempPosition;
        float t = 0;
        while (gameObject.transform.position != Point)
        {
            TempPosition = gameObject.transform.position;
            TempPosition.x = Mathf.Lerp(StartPoint.x, Point.x, t);
            gameObject.transform.position = TempPosition;
            t += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        bEndMove = true;
        if (Point.x != 0)
        {
            bAway = true;
        }
    }

     // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
