using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracerHUD : MonoBehaviour {

	[Header("Blink UI")]
	[SerializeField] private GameObject[] blinkIndicators;
	[SerializeField] private Image blinkPanel;
	[SerializeField] private RawImage blinkSymbol;
	[SerializeField] private Image blinksRemainingCircle;
	[SerializeField] private Text blinksRemainingText;
	[SerializeField] private Image shiftPanel;
	[SerializeField] private Text shiftText;

	[Header("Recall UI")]
	[SerializeField] private Image recallPanel;
	[SerializeField] private RawImage recallSymbol;
	[SerializeField] private Image ePanel;
	[SerializeField] private Text eText;
	[SerializeField] private Text recallCountdown;
	[SerializeField] private Slider recallSlider;

	private int blinksRemaining = 3;
	private float recallCooldown;
	private float recallTimer = 12;
	private bool isRecalling = false;

	private readonly Color myWhite = new Color(1, 1, 1);
	private readonly Color myBlack = new Color(0, 0, 0);
	private readonly Color myOrange = new Color(1, 0.58f, 0);
	private readonly Color myRed = new Color(0.45f, 0.13f, 0.13f);
	private readonly float fadedInAlpha = 0.78f;
	private readonly float fadedOutAlpha = 0.39f;

	void Awake() {
		//add UpdateRecall to the recallAction event
		TracerRecall.RecallAction += UpdateRecall;
	}

	//helper function to change only the alpha of an image or text
	private Color ChangeAlpha(Color c, float a) {
		return new Color(c.r, c.g, c.b, a);
	}

	// -- BLINK --

	//called by TracerBlink to update the UI on Tracer losing a blink
	public void TakeBlink() {
		for(int i = 0; i < blinkIndicators.Length; i++) {       //iterate through the blink indicator array forwards
			if(blinkIndicators[i].activeInHierarchy == true) {  //find the first indicator from the front which is currently active
				blinkIndicators[i].SetActive(false);            //deactivate that indicator
				break;
			}
		}
		//update the blinks reamining and the text
		blinksRemaining -= 1;
		blinksRemainingText.text = blinksRemaining.ToString();
		//change the circle to orange
		blinksRemainingCircle.color = myOrange;
		//if blinks remaining is now 0, then change the panel and symbol to empty and red, and fade out the lshift panel and text
		if(blinksRemaining == 0) {
			DisableBlinkPanel();
		}
	}

	//helper function to change the blink panel to empty and red
	private void DisableBlinkPanel() {
		blinkPanel.fillCenter = false;
		blinkPanel.color = myRed;
		blinkSymbol.color = myRed;
		shiftPanel.color = ChangeAlpha(shiftPanel.color, fadedOutAlpha);
		shiftText.color = ChangeAlpha(shiftText.color, fadedOutAlpha);
	}

	//called by TracerBlink to update the UI on Tracer gaining a blink
	public void GiveBlink() {
		//add a blink indicator
		for(int j = blinkIndicators.Length - 1; j >= 0; j--) {  //iterate through the blink indicator array backwards
			if(blinkIndicators[j].activeInHierarchy == false) { //find the first indicator from the back which is currently inactive
				blinkIndicators[j].SetActive(true);             //activate that indicator
				break;
			}
		}
		//update the blinks reamining and the text
		blinksRemaining += 1;
		blinksRemainingText.text = blinksRemaining.ToString();
		//if blinks remaining is now max, then change the circle to white
		if(blinksRemaining == 3) {
			blinksRemainingCircle.color = myWhite;
        }
		//enable the blink panel unless Tracer is Recalling
		if(!isRecalling) {
			EnableBlinkPanel();
		}
	}

	//helper function to change the blink panel to filled and whtie and the symbol to black
	private void EnableBlinkPanel() {
		//change the blink panel and symbol back to their colors and fade in the shift panel and text
		blinkPanel.fillCenter = true;   //the panel is filled
		blinkPanel.color = myWhite;     //the panel is white
		blinkSymbol.color = myBlack;    //the symbol is black
		shiftPanel.color = ChangeAlpha(shiftPanel.color, fadedInAlpha);
		shiftText.color = ChangeAlpha(shiftText.color, fadedInAlpha);
	}

	// -- RECALL --

	//activated by the recallAction event to signal Tracer beginning or ending the Recall ability
	private void UpdateRecall(bool recalling) {
		if(recalling) {
			BeginRecall();
		} else {
			EndRecall();
		}
	}

	//called by UpdateRecall to update the UI on Tracer using Recall
	private void BeginRecall() {
		//change the panel to orange
		recallPanel.color = myOrange;
		//diable the blink panel
		DisableBlinkPanel();
		//set recalling flag to true
		isRecalling = true;
	}

	//called by UpdateRcall to update the UI on Tracer ending the Recall ability
	private void EndRecall() {
		//change the panel to empty
		recallPanel.fillCenter = false;
		//fade out the symbol and turn it white
		recallSymbol.color = ChangeAlpha(myWhite, fadedOutAlpha);
		//fade out the e panel and text
		ePanel.color = ChangeAlpha(ePanel.color, fadedOutAlpha);
		eText.color = ChangeAlpha(eText.color, fadedOutAlpha);
		//enable the blink panel unless all blinks have been used
		if(blinksRemaining > 0) {
			EnableBlinkPanel();
		}
		//start countdown coroutine
		StartCoroutine(RecallCountdown());
		//set recalling flag to false
		isRecalling = false;
	}

	//called by EndRecall to countdown the show the time remaining until Tracer can Recall again
	private IEnumerator RecallCountdown() {
		recallCountdown.gameObject.SetActive(true);         //activate the countdown text
		recallSlider.gameObject.SetActive(true);			//activate the countdown slider
		recallTimer = recallCooldown;						//initialize the countdown timer
		while(recallTimer > 0) {
			recallCountdown.text = ((int) recallTimer + 1).ToString();	//update the recall countdown text
			recallSlider.value = recallCooldown - recallTimer;			//update the recall countdown slider
			recallTimer -= Time.deltaTime;								//subtract 1 from the Recall timer
			yield return null;
		}
		recallCountdown.gameObject.SetActive(false);        //deactivate the countdown text
		recallSlider.gameObject.SetActive(false);           //deactivate the countdown slider (these should happen at the same time GiveRecall is called)
	}

	//called by TracerRecall to update the UI on when the cooldown for Recall has ended
	public void GiveRecall() {
		//change the panel to white and filled
		recallPanel.color = myWhite;
		recallPanel.fillCenter = true;
		//change the symbol to black and fade it in
		recallSymbol.color = ChangeAlpha(myBlack, fadedInAlpha);
		//fade in the e panel and text
		ePanel.color = ChangeAlpha(ePanel.color, fadedInAlpha);
		eText.color = ChangeAlpha(eText.color, fadedInAlpha);
	}

	//called by TracerRecall on awake to update this script on the recall cooldown to maintain consistency
	public void SetRecallCooldown(float cooldown) {
		recallCooldown = cooldown;
	}
}