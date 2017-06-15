using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public SimpleObjectPool answerButtonObjectPool;
	public Text QuestionText;
	public Text ScoreDisplayText;
	public Text TimeRemainingDisplayText;
	public Transform answerButtonParent;
	public GameObject questionPanel;
	public GameObject roundEndPanel;

	private DataController dataController;
	private RoundData currentRoundData;
	private QuestionData[] questionPool;

	private bool isRoundActive;
	private float timeRemaining;
	private int questionIndex;
	private int playerScore;
	private List<GameObject> answerButtonGameObjects = new List<GameObject>();

	// Use this for initialization
	void Start () 
	{
		dataController = FindObjectOfType<DataController> ();	
		currentRoundData = dataController.GetCurrentRoundData ();
		questionPool = currentRoundData.questions;
		timeRemaining = currentRoundData.timeLimitInSeconds;
		UpdateTimeRemainingDisplay ();

		playerScore = 0;
		questionIndex = 0;

		ShowQuestion ();
		isRoundActive = true;
	}

	private void ShowQuestion()
	{
		// remove existing answer buttons
		RemoveAnswerButtons();
		QuestionData questionData = questionPool [questionIndex];

		QuestionText.text = questionData.questionText;

		for (int i = 0; i < questionData.answers.Length; i++) 
		{
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject ();
			answerButtonGameObject.transform.SetParent (answerButtonParent);
			answerButtonGameObjects.Add (answerButtonGameObject);

			AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton> ();
			answerButton.Setup (questionData.answers [i]);
		}
	}

	private void RemoveAnswerButtons()
	{
		while (answerButtonGameObjects.Count > 0) 
		{
			answerButtonObjectPool.ReturnObject (answerButtonGameObjects [0]);
			answerButtonGameObjects.RemoveAt (0);
		}
	}

	public void AnswerButtonClicked(bool isCorrect)
	{
		if (isCorrect) {
			playerScore += currentRoundData.pointsAddedForCorrectAnswer;
			ScoreDisplayText.text = "Score: " + playerScore;
		}

		if (questionPool.Length > questionIndex + 1) 
		{
			questionIndex++;
			ShowQuestion ();
		} 
		else
		{
			EndRound ();
		}
	}

	public void EndRound()
	{
		isRoundActive = false;
		questionPanel.SetActive (false);
		roundEndPanel.SetActive (true);
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene ("MenuScreen");
	}

	private void UpdateTimeRemainingDisplay()
	{
		TimeRemainingDisplayText.text = "Time: " + Mathf.Round (timeRemaining).ToString ();
	}

	// Update is called once per frame
	void Update () {
		// update timer
		if (isRoundActive) 
		{
			timeRemaining -= Time.deltaTime;
			UpdateTimeRemainingDisplay ();

			if (timeRemaining <= 0f) 
			{
				EndRound ();
			}
		}
	}
}
