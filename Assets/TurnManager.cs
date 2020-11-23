using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public Text turnText;

    private int _turn;
    private int Turn
    {
        get { return _turn; }
        set
        {
            _turn = value;
            turnText.text = _turn.ToString();
        }
    }

    private void Start()
    {
        Turn = 1;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
