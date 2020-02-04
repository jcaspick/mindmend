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

    public void RedSignal()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.RedSignal));
    }

    public void BlueSignal()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.BlueSignal));
    }

    public void RedGoal(int number)
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.RedGoal, number));
    }

    public void BlueGoal(int number)
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.BlueGoal, number));
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
