using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeField : MonoBehaviour
{
    private Mesh EquationMeshInfo;
    private Vector3[] EquationVertices;
    public bool bEndResize = false;
    public IEnumerator FieldResize(float SmallerSize, float LargerSize, float delta) //Изменение размера поля
    {
        Vector3 point;
        EquationMeshInfo = gameObject.GetComponent<MeshFilter>().mesh;
        EquationVertices = EquationMeshInfo.vertices;
        Transform EqTransform = gameObject.transform;

        while (SmallerSize < LargerSize)
        {
            if (LargerSize - SmallerSize < 0.02f && gameObject.name == "Equation" && System.Math.Sign(delta) == -1)
            {
                delta /= 3f;
            }
            if (System.Math.Abs(delta) < 0.001 && gameObject.name == "Equation" && System.Math.Sign(delta) == -1)
            {
                yield break;
            }
            for (int i = 0; i < EquationMeshInfo.vertexCount; i++)
            {
                point = EqTransform.TransformPoint(EquationVertices[i]);
                if (point.x < EqTransform.transform.position.x)
                {
                    point.x -= delta;
                    EquationVertices[i] = EqTransform.InverseTransformPoint(point);
                }
                else
                {
                    point.x += delta;
                    EquationVertices[i] = EqTransform.InverseTransformPoint(point);
                }
            }
            LargerSize -= System.Math.Abs(delta) * 2f;
            EquationMeshInfo.vertices = EquationVertices;
            gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            yield return new WaitForSeconds(0.01f);
        }
        bEndResize = true;
        
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
