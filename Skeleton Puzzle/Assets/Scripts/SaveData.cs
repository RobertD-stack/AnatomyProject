using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
/* *** SUMMARY *** 

*/

public enum QuestionnaireType
{
    Demographic, Layer, Post
}

// Check if we are appending to existing data or not
public enum AppendExistingData
{
    True, False
}

public class SaveData : MonoBehaviour
{
    public List<string> questions = new List<string>();
    public List<string> answers = new List<string>();

    public QuestionnaireType questionnaireType;
    public AppendExistingData appendExistingData;


    public void Start()
    {


        questions = new List<string>();
        answers = new List<string>();

    }
    public void SaveIntoJSON()
    {

        GameObject[] questionsArray = GameObject.FindGameObjectsWithTag("Question");
        GameObject[] answerArray = GameObject.FindGameObjectsWithTag("Answer");
        AnswerData newEntry = new AnswerData();

        string path = "C:/Users/super/Skeleton Puzzle/Assets/QuestionnaireData/QuestionnaireData.json";
        // Check if we are editing existing data or overwriting existing data
        if (appendExistingData == AppendExistingData.False)
        {
            File.Delete(path);
        }
        AnswerDataList dataList = new AnswerDataList();



        // Add questions and answers to new entry
        foreach (GameObject question in questionsArray)
        {
            newEntry.questions.Add(question.GetComponent<TMP_Text>().text);
        }

        foreach (GameObject answer in answerArray)
        {
            newEntry.answers.Add(answer.GetComponent<TMP_Dropdown>().options[answer.GetComponent<TMP_Dropdown>().value].text);
        }

        //Reverse Lists since data is added in an opposite direction to what we want it to be
        newEntry.questions.Reverse();
        newEntry.answers.Reverse();


        // Load existing data
        if (System.IO.File.Exists(path))
        {
            if (System.IO.File.ReadAllText(path).Length > 0)
            {
                string existingJson = System.IO.File.ReadAllText(path);
                dataList = JsonUtility.FromJson<AnswerDataList>(existingJson);
                if (dataList.demographicEntries == null) dataList.demographicEntries = new List<AnswerData>();
                if (dataList.knowledgeTests == null) dataList.knowledgeTests = new List<AnswerData>();
                if (dataList.postSurvey == null) dataList.postSurvey = new List<AnswerData>();
            }
            else if (System.IO.File.ReadAllText(path).Length == 0)
            {
                Debug.Log("Found an empty filepath at " + path + " - reinitializing");
                File.Delete(path);
                dataList = new AnswerDataList();
                dataList.userId = Random.Range(0, 9999);
            }

        }
        else if (!System.IO.File.Exists(path))
        {
            dataList = new AnswerDataList();
            dataList.userId = Random.Range(0, 9999);
        }





        // Add a timestamp
        newEntry.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");




        // Append and save data
        if (questionnaireType == QuestionnaireType.Demographic)
        {
            dataList.demographicEntries.Clear();
            dataList.demographicEntries.Add(newEntry);

        }
        else if (questionnaireType == QuestionnaireType.Layer)
        {
            dataList.knowledgeTests.Add(newEntry);
        }
        else
        {
            dataList.postSurvey.Clear();
            dataList.postSurvey.Add(newEntry);
        }
        string newJson = JsonUtility.ToJson(dataList, true);
        System.IO.File.WriteAllText(path, newJson);
        Debug.Log("Saved to: " + path);

    }


}
[System.Serializable]
public class AnswerDataList
{
    public int userId = Random.Range(0, 9999);
    public List<AnswerData> demographicEntries = new List<AnswerData>();
    public List<AnswerData> knowledgeTests = new List<AnswerData>();
    public List<AnswerData> postSurvey = new List<AnswerData>();


}
[System.Serializable]
public class AnswerData
{
    public List<string> questions = new List<string>();
    public List<string> answers = new List<string>();
    public string timestamp;
}


