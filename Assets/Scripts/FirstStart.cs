using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStart : MonoBehaviour
{
    public IEnumerator BirthFields()
    {
        Color OptionColor;
        float t = 0;
        gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        while (gameObject.GetComponent<MeshRenderer>().material.color.r != 1f)
        {
            OptionColor = gameObject.GetComponent<MeshRenderer>().material.color;
            OptionColor.r = Mathf.Lerp(0f, 1f, t);
            OptionColor.g = Mathf.Lerp(0f, 1f, t);
            OptionColor.b = Mathf.Lerp(0f, 1f, t);
            gameObject.GetComponent<MeshRenderer>().material.color = OptionColor;
            OptionColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
            OptionColor.r = Mathf.Lerp(0f, 0.2f, t);
            OptionColor.g = Mathf.Lerp(0f, 0.2f, t);
            OptionColor.b = Mathf.Lerp(0f, 0.2f, t);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", OptionColor);
            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        //gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
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
