using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer
{

    public void TransferToLoadingScene()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void TransferToSelectField()
    {
        SceneManager.LoadScene("SelectField");
    }

    public void TransferToBattleField()
    {
        SceneManager.LoadScene("BattleField");
    }
}
