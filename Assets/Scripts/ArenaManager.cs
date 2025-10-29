using UnityEngine;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    public Player_Movement player1;
    public Player2_Movement player2;
    public Camera mainCam;

    private bool roundActive = false;

    void Start()
    {
        player1.opponent = player2.transform;
        player2.opponent = player1.transform;

        StartCoroutine(RoundIntro());
    }

    IEnumerator RoundIntro()
    {
        FightUIManager.Instance.ShowMessage("ROUND 1");
        yield return new WaitForSeconds(2f);
        FightUIManager.Instance.ShowMessage("FIGHT!");
        roundActive = true;
    }

    public void RoundOver(int loserIndex)
    {
        if (!roundActive) return;
        roundActive = false;

        int winner = loserIndex == 1 ? 2 : 1;
        FightUIManager.Instance.ShowMessage($"Player {winner} Wins!");
        StartCoroutine(RestartRound());
    }

    IEnumerator RestartRound()
    {
        yield return new WaitForSeconds(4f);

        // Reset positions
        player1.transform.position = new Vector3(-2, 0, 0);
        player2.transform.position = new Vector3(2, 0, 0);

        player1.GetComponent<HealthSystem>().Start();
        player2.GetComponent<HealthSystem>().Start();

        StartCoroutine(RoundIntro());
    }

    void Update()
    {
        if (mainCam)
        {
            Vector3 midpoint = (player1.transform.position + player2.transform.position) / 2f;
            //mainCam.transform.position = new Vector3(midpoint.x, 2.5f, -8f);
        }
    }
}
