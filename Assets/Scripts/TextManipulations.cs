using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManipulations : MonoBehaviour
{
    public GameObject OptionString;

    public IEnumerator BirthText(float a_lphaStart, float a_lphaEnd)
    {
        float delta = 0f;

        while (gameObject.GetComponent<TextMeshPro>().alpha != a_lphaEnd)
        {
            gameObject.GetComponent<TextMeshPro>().alpha = Mathf.Lerp(a_lphaStart, a_lphaEnd, delta);
            delta += 0.03f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    public void SetPosition(Vector3 Position)
    {
        Vector3 TempPos = Position;
        TempPos.z -= 0.6f;
        gameObject.transform.position = TempPos;
    }

    public TextMeshPro InitOptions(string Option, int index, int indSolution)
    {
        gameObject.name = "OptionString" + index.ToString();
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 5);
        gameObject.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
        gameObject.GetComponent<TextMeshPro>().verticalAlignment = VerticalAlignmentOptions.Middle;
        gameObject.GetComponent<TextMeshPro>().transform.localScale = new Vector3(0.2393668f, 0.2393668f, 0.2393668f);
        gameObject.GetComponent<TextMeshPro>().name = "Solution";
        gameObject.GetComponent<TextMeshPro>().alpha = 0f;
        gameObject.GetComponent<TextMeshPro>().color = new Color32(0, 0, 0, 0);
        if (indSolution != index)
        {
            OptionString = Instantiate(gameObject, gameObject.GetComponent<TextMeshPro>().transform.position, Quaternion.identity);
            OptionString.GetComponent<TextMeshPro>().name = "OptionString" + index.ToString();
            OptionString.GetComponent<TextMeshPro>().text = Option;
            return OptionString.GetComponent<TextMeshPro>();
        }
        else
        {
            return gameObject.GetComponent<TextMeshPro>();
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
