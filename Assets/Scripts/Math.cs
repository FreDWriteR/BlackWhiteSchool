using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using Mono.Data.Sqlite;
using System.Data;
using System.Globalization;

public class Math : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] _Option = new GameObject[4];
    IDataReader EquationReader; //��������� �� ����
    IDbConnection dbEquationConnection; //���������� � �����
    TextMeshPro[] OptionsString = new TextMeshPro[4]; //��������
    public GameObject Equation; //���� ��� ����������
    Bloom Shining; //������������� ��������� �����
    IEnumerator coroutine; //������������
    bool waitToStart = true, ShowOptions = false, getEquations = false, bLoad = true, firstStart = true, End = false; //������)
    Vector3 Position; //��������� �����e���� ��� �������������� ����������� ����� � �������
    string TempPartBefore, TempPartAfter, TempPartSolution, TempPartSolutionString, TempPartX; //��������� ���������� ��� ������������� ������� ���������
    List<int> wrongIndexes = new List<int>(); //������� ������������ ������� ������ �������
    
    int indSolution; //������ ����������� ������
    public TextMeshPro X, BeforeX, AfterX, Solution, SolutionString; //������
    float differenceWidth; //������� ����� ������� ������ ������� � ������ ��������� ��� ����������� ������ ������ ���������(��������� � �����)
    Vector3 NewEnd;

    private (IDbConnection EqConnection, IDataReader Read) SelectEquations() //����������� � ���� � ��������� ���������
    {
        // Open a connection to the database.
        string dbUri = "URI=file:Equations.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();

        dbCommandReadValues.CommandText = "SELECT * FROM Equation";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        return (dbConnection, dataReader);
    }

    void Start()
    {
        GameObject.Find("PostProcessing").GetComponent<PostProcessVolume>().profile.TryGetSettings(out Shining);
        if (bLoad)
        {
            LoadEquation(); //�������� ���������
        }
        else
        {
            coroutine = GetStartState(); //��������� ��������� ������� ����� ������� ���������
            StartCoroutine(coroutine);
        }
    }

    IEnumerator EndGame() //����� ����
    {
        float t = 0f;
        while (Shining.intensity.value != 25)
        {
            Shining.intensity.value = Mathf.Lerp(8, 25, t);
            yield return new WaitForSeconds(0.01f);
            t += 0.01f;
        }
    }

    void LoadEquation() //�������� ��������� � ���������
    {
        waitToStart = true;
        ShowOptions = false;
        if (!getEquations)
        {
            (dbEquationConnection, EquationReader) = SelectEquations(); //�������� ���� ���������
        }
        if (!EquationReader.Read()) //��������� ���� ���������. ���� ������ ��� ��������� ����������� ����.
        {
            dbEquationConnection.Close();
            coroutine = EndGame();
            StartCoroutine(coroutine);
            End = true;
            return;
        }
        System.Random rand = new System.Random();

        X.text = "X";
        TempPartX = X.text;

        Equation = GameObject.Find("Equation");
        BeforeX.text = EquationReader.GetString(1);
        TempPartBefore = BeforeX.text;

        AfterX.text = EquationReader.GetString(3);
        TempPartAfter = AfterX.text;

        Solution.text = EquationReader.GetString(2);
        TempPartSolution = Solution.text;

        SolutionString.text = BeforeX.text + Solution.text + AfterX.text;
        TempPartSolutionString = SolutionString.text;

        BeforeX.text = TempPartBefore + TempPartSolution + TempPartAfter;
        AfterX.text = TempPartBefore + TempPartSolution + TempPartAfter;
        X.text = TempPartBefore + TempPartX + TempPartAfter;

        differenceWidth = AfterX.preferredWidth - X.preferredWidth;
        if (X.preferredWidth > AfterX.preferredWidth)
        {
            X.text = "<alpha=#00>" + TempPartBefore + "</color>" + "<color=#000000>" + TempPartX + "</color>" + "<alpha=#00>" + TempPartAfter + "</color>";
        }
        while (X.preferredWidth < AfterX.preferredWidth)
        {
            X.text = "<alpha=#00>" + TempPartBefore + "</color>" + "<color=#000000>" + "<space=" + (differenceWidth / 2).ToString() + "px>" + TempPartX + "<space=" + (differenceWidth / 2).ToString() + "px>" + "</color>" + "<alpha=#00>" + TempPartAfter + "</color>";
            differenceWidth += 0.1f;
        }
        
        Position = Equation.transform.position;
        Position.z -= 1.0f;
        X.gameObject.GetComponent<TextManipulations>().SetPosition(Position);
        X.alpha = 0f;
        X.alignment = TextAlignmentOptions.Center;
        X.verticalAlignment = VerticalAlignmentOptions.Middle;

        Position = Equation.transform.position;
        Position.z -= 0.9f;
        BeforeX.gameObject.GetComponent<TextManipulations>().SetPosition(Position);

        Position = Equation.transform.position;
        Position.z -= 0.8f;
        AfterX.gameObject.GetComponent<TextManipulations>().SetPosition(Position);

        Position = Equation.transform.position;
        Position.z -= 0.6f;
        SolutionString.gameObject.GetComponent<TextManipulations>().SetPosition(Position);

        BeforeX.text = "<color=#000000>" + TempPartBefore + "</color>" + "<alpha=#00>" + TempPartSolution + TempPartAfter + "</color>";
        BeforeX.alpha = 0f;

        AfterX.text = "<alpha=#00>" + TempPartBefore + TempPartSolution + "</color>" + "<color=#000000>" + TempPartAfter + "</color>";
        AfterX.alpha = 0f;

        SolutionString.text = "<alpha=#00>" + TempPartSolutionString + "</color>";

        /////////�������� ����� ��� ��������� � ���������� ��������� ���������� ���������� ��� �� ����/////////////
        ////////////////////////////� ����������� ��������� � ��������� �������////////////////////////////////////
        List<int> listOptionS = new List<int>();
        List<int> Indexlist = new List<int>();
        int optionString, PlaseForSolution = 0;
        string Option;

        while (Indexlist.Count < 4)
        { //�������� 4 ������� �� 0 �� 3 � ��������� �������
            do
            {
                optionString = rand.Next(4);
            } while (Indexlist.Contains(optionString));
            Indexlist.Add(optionString);
        }
        foreach (int i in Indexlist)
        {
            if (PlaseForSolution < 3)
            { //����������� ������������ ���������
                if (EquationReader.GetInt32(4) == 0) //���� �������� ������������ ������� ������� �� ����
                {
                    Option = EquationReader.GetString(PlaseForSolution + 5); //��������� ���� ������� �� ����
                }
                else //���� �������� ������������ ������� ����������� ��������
                {
                    do //�������� ��������� ����� �� -5 �� 5 ��� ���������� � �� ������ ����������� ��������
                    {
                        optionString = rand.Next(-5, 5);
                    } while (listOptionS.Contains(optionString) || optionString == 0);
                    listOptionS.Add(optionString);
                    if (System.Math.Truncate(float.Parse(Solution.text, CultureInfo.InvariantCulture.NumberFormat)) == float.Parse(Solution.text, CultureInfo.InvariantCulture.NumberFormat)) //���������� ���������� ��������� ����� � ����������� ��������
                    {
                        Option = (float.Parse(Solution.text) + optionString).ToString();
                    }
                    else
                    {
                        Option = (float.Parse(Solution.text, CultureInfo.InvariantCulture.NumberFormat) + optionString).ToString("F1", new CultureInfo("en-US").NumberFormat); 
                    }
                }
                OptionsString[i] = Solution.GetComponent<TextManipulations>().InitOptions(Option, i, Indexlist[3]);
            }
            else //������ ��� ����������� ��������
            {
                OptionsString[i] = Solution.GetComponent<TextManipulations>().InitOptions(Solution.text, i, Indexlist[3]);
                indSolution = i;
            }
            PlaseForSolution++;
        }
        getEquations = true;
        ///////////////////////////////////////////////////////////////////////
    }

    void destroyOptionsString() //�������� ��������� ����� ������� ���������
    {
        for (int i = 0; i < 4; i++)
        {
            if (OptionsString[i].name != "Solution")
            {
                Destroy(OptionsString[i].gameObject);
            }
        }
    }

    IEnumerator GetStartState() //��������� ��������� ������� ����� ������� ���������
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 4; i++)
        {
            _Option[i].GetComponent<OptionsManipulation>().bEndMove = false;
            _Option[i].GetComponent<OptionsManipulation>().bEndChangeColor = false;
            _Option[i].GetComponent<ResizeField>().bEndResize = false;
            _Option[i].GetComponent<OptionsManipulation>().bAway = false;
        }

        foreach (int i in wrongIndexes)
        {
            coroutine = _Option[i].GetComponent<OptionsManipulation>().ChangeColor(0.2f, 0.2f, 0.2f);
            StartCoroutine(coroutine);
        }
        coroutine = _Option[indSolution].GetComponent<OptionsManipulation>().ChangeColor(0.2f, 0.2f, 0.2f);
        StartCoroutine(coroutine);
        Birth(1f, 0f);
        if (Solution.gameObject.transform.position == X.gameObject.transform.position)
        {
            yield return new WaitForSeconds(2f);
            float delta = ((Equation.GetComponent<MeshRenderer>().bounds.size.x - 1.827262f) / 2f) / 50f;
            coroutine = Equation.GetComponent<ResizeField>().FieldResize(1.827262f, Equation.GetComponent<MeshRenderer>().bounds.size.x, -delta);
            StartCoroutine(coroutine);
            for (int i = 0; i < 4; i++)
            {
                delta = ((_Option[i].GetComponent<MeshRenderer>().bounds.size.x - 1.827262f) / 2f) / 50f;
                coroutine = _Option[i].GetComponent<ResizeField>().FieldResize(1.827262f, _Option[i].GetComponent<MeshRenderer>().bounds.size.x, -delta);
                StartCoroutine(coroutine);
                Vector3 ZeroPosition = _Option[0].transform.position;
                ZeroPosition.x = 0f;
                coroutine = _Option[i].GetComponent<OptionsManipulation>().MoveOption(ZeroPosition);
                StartCoroutine(coroutine);
            }
        }
        yield return new WaitForSeconds(0.5f);
        destroyOptionsString();
        yield return new WaitForSeconds(0.5f);
        Start();
    }

    void LoadGame() //���������� � ����������� ���� �����
    {
        //��������� ���� ��� ���������
        Vector3 EndPoint0 = _Option[0].transform.position;
        float[] SizeX = new float[4];
        int i;
        for (i = 0; i < 4; i++)
        {
            SizeX[i] = _Option[i].GetComponent<MeshRenderer>().bounds.size.x;
            if (OptionsString[i].renderer.bounds.size.x + 0.4f > _Option[i].GetComponent<MeshRenderer>().bounds.size.x)
            {
                SizeX[i] = OptionsString[i].renderer.bounds.size.x + 0.4f;
            }
        }
        EndPoint0.x = 0f - (SizeX[0] + 1.5f +
                            SizeX[1] + 1.5f +
                            SizeX[2] + 1.5f +
                            SizeX[3]) / 2f;
        EndPoint0.x += SizeX[0] / 2f;
        
        for (i = 0; i < 4; i++)
        {
            coroutine = _Option[i].GetComponent<ResizeField>().FieldResize(1.827262f, OptionsString[i].renderer.bounds.size.x + 0.4f, 0.1f);
            StartCoroutine(coroutine);
            coroutine = _Option[i].GetComponent<OptionsManipulation>().MoveOption(EndPoint0);
            StartCoroutine(coroutine);
            if (i < 3) 
            {
                EndPoint0.x += SizeX[i] / 2f + 1.5f + SizeX[i + 1] / 2f;
            }
        }

        //����������� ���� ��� ���������
        float delta = (((X.GetComponent<MeshRenderer>().bounds.size.x + 0.6f) - 1.827262f) / 2f) / 50f;
        coroutine = Equation.GetComponent<ResizeField>().FieldResize(Equation.GetComponent<MeshRenderer>().bounds.size.x, X.renderer.bounds.size.x + 0.6f, delta);
        StartCoroutine(coroutine);
    }


    IEnumerator BirthFirstStart() //������ ����� ����. 5 ������ ������ ����� ����� ��������� ���� �����
    {
        yield return new WaitForSeconds(5f);
        float t = 0;
        while (GameObject.Find("Sun").GetComponent<Light>().intensity != 1)
        {
            GameObject.Find("Sun").GetComponent<Light>().intensity = Mathf.Lerp(0f, 1f, t);
            t += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 4; i++)
        {
            coroutine = _Option[i].GetComponent<FirstStart>().BirthFields();
            StartCoroutine(coroutine);
        }
        coroutine = Equation.GetComponent<FirstStart>().BirthFields();
        StartCoroutine(coroutine);
    }

    void Birth(float a_lphaStart, float a_lphaEnd) //��������� �������
    {
        for (int i = 0; i < 4; i++)
        {
            coroutine = OptionsString[i].gameObject.GetComponent<TextManipulations>().BirthText(a_lphaStart, a_lphaEnd);
            StartCoroutine(coroutine);
        }
        coroutine = BeforeX.gameObject.GetComponent<TextManipulations>().BirthText(a_lphaStart, a_lphaEnd);
        StartCoroutine(coroutine);
        coroutine = AfterX.gameObject.GetComponent<TextManipulations>().BirthText(a_lphaStart, a_lphaEnd);
        StartCoroutine(coroutine);
        coroutine = X.gameObject.GetComponent<TextManipulations>().BirthText(a_lphaStart, a_lphaEnd);
        StartCoroutine(coroutine);
        if (bLoad) { 
            for (int i = 0; i < 4; i++)
            {
                OptionsString[i].gameObject.GetComponent<BoxCollider>().size = new Vector3(9f, 9f, 9f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!End)
        {
            if (waitToStart)
            {
                if (firstStart)
                {
                    coroutine = BirthFirstStart();
                    StartCoroutine(coroutine);
                    firstStart = false;
                }
                else if (_Option[0].GetComponent<MeshRenderer>().material.color.g == 1f)
                {
                    LoadGame();
                    waitToStart = false;
                }
            }
            else if (!ShowOptions && _Option[0].GetComponent<OptionsManipulation>().bEndMove && _Option[0].GetComponent<ResizeField>().bEndResize && _Option[0].GetComponent<OptionsManipulation>().bAway)
            {
                ShowOptions = true;
                Vector3 TempPos = new Vector3();
                for (int i = 0; i < 4; i++)
                {
                    TempPos.z -= 0.6f;
                    TempPos.x = _Option[i].transform.position.x;
                    TempPos.y = _Option[i].transform.position.y;
                    OptionsString[i].transform.position = TempPos;
                }
                NewEnd = new Vector3((OptionsString[indSolution].gameObject.transform.position.x + X.gameObject.transform.position.x) * 0.5f,
                                     (OptionsString[indSolution].gameObject.transform.position.y + X.gameObject.transform.position.y) * 0.5f,
                                     (OptionsString[indSolution].gameObject.transform.position.z + X.gameObject.transform.position.z) * 0.5f);
                Birth(0f, 1f);
            }
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.name == "Solution")
                    {
                        X.alpha = 0f;
                        Solution.gameObject.transform.localScale *= 2f;
                        coroutine = _Option[indSolution].GetComponent<OptionsManipulation>().ChangeColor(0f, 0.5f, 0f);
                        StartCoroutine(coroutine);
                        coroutine = Solution.GetComponent<EvaluateSolution>().EvaluateXPoint(hit.collider.gameObject.transform.position, NewEnd, X.gameObject.transform.position.y - hit.collider.gameObject.transform.position.y);
                        StartCoroutine(coroutine);
                        return;
                    }
                    else if (hit.collider.gameObject.name == "OptionString0" || hit.collider.gameObject.name == "OptionString1" || hit.collider.gameObject.name == "OptionString2" || hit.collider.gameObject.name == "OptionString3")
                    {
                        coroutine = _Option[int.Parse(hit.collider.gameObject.name.Substring(12))].GetComponent<OptionsManipulation>().ChangeColor(0.5f, 0f, 0f);
                        StartCoroutine(coroutine);
                        OptionsString[int.Parse(hit.collider.gameObject.name.Substring(12, 1))].alpha = 0;
                        wrongIndexes.Add(int.Parse(hit.collider.gameObject.name.Substring(12, 1)));
                    }
                }
            }
            if (Solution.gameObject.transform.position == NewEnd)
            {
                NewEnd = X.gameObject.transform.position;
                Solution.text = "<alpha=#00>" + TempPartBefore + "</color>" + "<color=#000000>" + TempPartSolution + "</color>" + "<alpha=#00>" + TempPartAfter + "</color>";
                coroutine = Solution.GetComponent<EvaluateSolution>().EvaluateXPoint(Solution.gameObject.transform.position, NewEnd, X.gameObject.transform.position.y - Solution.gameObject.transform.position.y);
                StartCoroutine(coroutine);
                NewEnd = new Vector3(0f, 0f, 0f);
                bLoad = false;
            }
            if (!bLoad)
            {
                Start();
                bLoad = true;
            }
        }
    }
}