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

	private int blinksRemaining = 3;

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
			blinksRemainingCircle.color = new Color(1, 1, 1);
        }
		//change the blink panel and symbol back to their colors and fade in the shift panel and text
		blinkPanel.fillCenter = true;			//the panel is filled
		blinkPanel.color = new Color(1, 1, 1);  //the panel is white
		blinkSymbol.color = new Color(0, 0, 0); //the symbol is black
		shiftPanel.color = new Color(shiftPanel.color.r, shiftPanel.color.g, shiftPanel.color.b, 0.78f);
		shiftText.color = new Color(shiftText.color.r, shiftText.color.g, shiftText.color.b, 0.78f);
	}

	//called by TracerBlink to update the UI on Tracer losing a blink
	public void TakeBlink() {
		for(int i = 0; i < blinkIndicators.Length; i++) {		//iterate through the blink indicator array forwards
			if(blinkIndicators[i].activeInHierarchy == true) {	//find the first indicator from the front which is currently active
				blinkIndicators[i].SetActive(false);			//deactivate that indicator
				break;
			}
		}
		//update the blinks reamining and the text
		blinksRemaining -= 1;
		blinksRemainingText.text = blinksRemaining.ToString();
		//change the circle to orange
		blinksRemainingCircle.color = new Color(1, 0.58f, 0);
		//if blinks remaining is now 0, then change the panel and symbol to empty and red, and fade out the lshift panel and text
		if(blinksRemaining == 0) {
			blinkPanel.fillCenter = false;
			blinkPanel.color = new Color(0.45f, 0.13f, 0.13f);
			blinkSymbol.color = new Color(0.45f, 0.13f, 0.13f);
			shiftPanel.color = new Color(shiftPanel.color.r, shiftPanel.color.g, shiftPanel.color.b, 0.39f);
			shiftText.color = new Color(shiftText.color.r, shiftText.color.g, shiftText.color.b, 0.39f);
		}
	}
}