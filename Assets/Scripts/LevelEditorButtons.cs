using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEditorButtons : MonoBehaviour
{
    public void Start() {
        EchoSavePath();
    }

    public void PlaceNode()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.Place));
    }

    public void EraseNode()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.Erase));
    }

    public void PlaceGoal()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.PlaceGoal));
    }

    public void PlaceSignal()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.PlaceSignal));
    }

    public void Load() {
        LevelEditor.instance.LoadBoard();
    }

    public void Save()
    {
        LevelEditor.instance.SaveBoard();
    }

    public void SetBoardWidth(string str)
    {
        int width = int.Parse(str);
        LevelEditor.instance.ChangeWidth(width);
    }

    public void SetBoardHeight(string str)
    {
        int height = int.Parse(str);
        LevelEditor.instance.ChangeHeight(height);
    }

    public void SetStartingHealth(string str)
    {
        int health = int.Parse(str);
        LevelEditor.instance.SetStartingHealth(health);
    }

    public void SetHealthPerGoal(string str)
    {
        int health = int.Parse(str);
        LevelEditor.instance.SetHealthPerGoal(health);
    }

    public void SetGoalIndex(string str)
    {
        // subtract one to go from non-programmer numbers to array index :)
        int goalIndex = int.Parse(str) - 1;
        LevelEditor.instance.SetGoalIndex(goalIndex);
    }

    public void SetNumGoals(string str)
    {
        int numGoals = int.Parse(str);
        LevelEditor.instance.SetNumGoals(numGoals);
    }

    public void SetAutoIncrement(bool val)
    {
        LevelEditor.instance.SetAutoIncrement(val);
    }

    public void SetNumColors(string str)
    {
        int numColors = int.Parse(str);
        LevelEditor.instance.SetNumColors(numColors);
    }

    public void SetSignalIndex(string str)
    {
        // subtract one to go from non-programmer numbers to array index :)
        int signalIndex = int.Parse(str) - 1;
        LevelEditor.instance.SetSignalIndex(signalIndex);
    }

    private void EchoSavePath() {
        Text text = gameObject.GetComponent<Text>();

        if (text != null && text.text.Contains("save path"))
            text.text = text.text.Replace("%s", Application.persistentDataPath);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
