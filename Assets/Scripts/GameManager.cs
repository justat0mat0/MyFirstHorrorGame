using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    None,
    Stage1_CheckMistake,
    Stage2_Puzzle,
    Stage3_Final,
}

public class GameManager : MonoBehaviour
{
    public GameState curState;
    public GameObject CheckListScrollView;
    [SerializeField] private float checkListFadeDuration = 1.5f;

    private Tween _checkListFadeTween;
    // Start is called before the first frame update
    void Start()
    {
        curState = GameState.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExitState(curState);
            curState = GameState.Stage1_CheckMistake;
            EnterState(curState);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ExitState(curState);
            curState = GameState.Stage2_Puzzle;
            EnterState(curState);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ExitState(curState);
            curState = GameState.Stage3_Final;
            EnterState(curState);
        }

        UpdateState(curState);
    }

    public void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Stage1_CheckMistake:
                if (CheckListScrollView != null)
                {
                    CheckListScrollView.SetActive(true);

                    var canvasGroup = CheckListScrollView.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                        canvasGroup = CheckListScrollView.AddComponent<CanvasGroup>();

                    var image = CheckListScrollView.GetComponent<Image>();
                    if (image != null)
                    {
                        Color c = image.color;
                        c.a = 1f;
                        image.color = c;
                    }

                    _checkListFadeTween?.Kill();
                    canvasGroup.alpha = 0f;
                    _checkListFadeTween = canvasGroup
                        .DOFade(1f, checkListFadeDuration)
                        .SetEase(Ease.OutQuad);
                }
                return;
            case GameState.Stage2_Puzzle:
                return;
            case GameState.Stage3_Final:
                return;
            default:
                return;
        }
    }
    public void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Stage1_CheckMistake:
                if (CheckListScrollView != null && CheckListScrollView.activeSelf)
                {
                    var canvasGroup = CheckListScrollView.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        CheckListScrollView.SetActive(false);
                        return;
                    }

                    _checkListFadeTween?.Kill();
                    _checkListFadeTween = canvasGroup
                        .DOFade(0f, checkListFadeDuration)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            CheckListScrollView.SetActive(false);
                            canvasGroup.alpha = 0f;
                        });
                }
                return;
            case GameState.Stage2_Puzzle:
                return;
            case GameState.Stage3_Final:
                return;
            default:
                return;
        }
    }

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.Stage1_CheckMistake:
                return;
            case GameState.Stage2_Puzzle:
                return;
            case GameState.Stage3_Final:
                return;
            default:
                return;
        }
    }
}
