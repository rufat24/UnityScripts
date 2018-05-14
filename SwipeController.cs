using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class SwipeController : MonoBehaviour {
	public GameObject continueButton;
	public Text bestScore;
	Touch touch;
	float screenWidth;
	float destination=0.0f;
	Vector3 camPos;
	Vector3 change;
	float direction;
	public float forwardSpeed=8.0f;
	public GameObject fadeOutImage;
	Animator anim;
	AudioSource[] asrc;
	float whispers = 0.01f;
	MobilePostProcessing ml;
	public AudioClip endSound;
	GameObject eg;
	Text score;
	public GameObject menuPanel;
	void Start(){
		Advertisement.Initialize ("1636337");
		screenWidth = Screen.width/2;
		anim = GetComponentInChildren<Animator>(); 
		StartCoroutine(FadeOut(fadeOutImage.GetComponent<Image> ()));
		asrc=gameObject.GetComponents<AudioSource> ();
		ml = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<MobilePostProcessing> ();
		eg=	GameObject.FindGameObjectWithTag ("EnemyGenerator");
		score = GameObject.FindGameObjectWithTag ("Score").GetComponent<Text>();
		if (!Social.localUser.authenticated) {
			Social.localUser.Authenticate ((bool success) => {
			});
		} else {
			string temp = PlayGamesPlatform.Instance.localUser.id;
			PlayGamesPlatform.Instance.LoadScores(
				GPGSIds.leaderboard_survivorsboard,
				LeaderboardStart.TopScores,
				100,
				LeaderboardCollection.Social,
				LeaderboardTimeSpan.AllTime,
				(data) =>
				{
					foreach(IScore score in data.Scores){
						if(score.userID==temp){
							bestScore.text=score.value.ToString();
						}
					}
				});
		}
	}
	void Update () {
		//acceleration
		if (forwardSpeed < 15.0f &&forwardSpeed!=0f) {
			forwardSpeed += 0.001f;
			if (ml.BlurAmount != Mathf.RoundToInt (forwardSpeed)) {
				ml.BlurAmount = Mathf.RoundToInt (forwardSpeed);
			}
		}
		//camera controls
		camPos = transform.position;
		int numOfTouches = Input.touches.Length;
		if (numOfTouches>0 && forwardSpeed>4.0f) {
			touch = Input.GetTouch (numOfTouches-1);
			if (touch.phase == TouchPhase.Began) {
				FindDestination (touch.position);
			}
		}
		/*if (Input.GetMouseButtonDown(0)) {
			FindDestination (Input.mousePosition);
		}
		*/
		score.text = Mathf.RoundToInt (camPos.z / 10).ToString();
		if (camPos.x != destination) {
			change = new Vector3 (direction	*0.5f,0f,forwardSpeed*Time.deltaTime);
		}
		else change = new Vector3 (0f, 0f,forwardSpeed*Time.deltaTime);
		transform.position += change;
		//audio binaural effect
		if (asrc [3].panStereo == 1f || asrc [3].panStereo == -1f) {
			whispers *= -1f;
		}
		asrc [3].panStereo += whispers;
		checkScore ();
	}
	void FindDestination(Vector3 tPos){
		if (tPos.x > screenWidth) {
			if (destination != 2.5f) {
				anim.SetTrigger ("Turnright");
				destination += 2.5f;
				direction = 1.0f;
			} else
				direction = 0.0f;
		}
		else if (tPos.x < screenWidth) {
			if (destination != -2.5f) {
				anim.SetTrigger ("Turnleft");
				destination -= 2.5f;
				direction = -1.0f;
			} else
				direction = 0.0f;
		}
	}
	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Ghost") {
			anim.SetInteger ("Enemy", 1);
			forwardSpeed = 0.0f;
			ml.BlurAmount = 0f;
			eg.SetActive (false);
			asrc [2].clip = endSound;
			asrc [2].Play ();
			asrc [0].Stop();
			StartCoroutine (OpenPause ());
		}
	}
	IEnumerator FadeOut(Image im){
		float op = 1f;
		while (im.color.a!=0.0f){
			op-=0.01f;
			im.color = new Color(0f,0f,0f,op);
			yield return null;
		}

	}
	IEnumerator OpenPause(){
		yield return new WaitForSeconds (2f);
		if (int.Parse (score.text) > GetPoint ()) {
			if (int.Parse (score.text) < 100) {
				SetPoint (100);
			} else
				SetPoint (500);
			SceneManager.LoadScene (2);
		} else {
			menuPanel.SetActive (true);
			Social.ReportScore(int.Parse (score.text) , "CgkI5aWmsr0QEAIQDQ", (bool success) => {
			});
		}
	}

	public void Continue(){
		const string rewardedAd = "rewardedVideo";
		if (PlayerPrefs.GetInt ("noads") == 1) {
			HandleAds (ShowResult.Finished);
			return;
		}
		#if UNITY_ADS
		if (!Advertisement.IsReady (rewardedAd)) {
			continueButton.SetActive (false);
		}
		Advertisement.Show ("rewardedVideo", new ShowOptions (){ resultCallback = HandleAds });
		#endif
	}
	#if UNITY_ADS
	private void HandleAds(ShowResult result){
		if (result.Equals (ShowResult.Finished)) {
			anim.SetInteger ("Enemy", 0);
			menuPanel.SetActive (false);
			forwardSpeed = 8f;
			ml.BlurAmount = 10f;
			eg.SetActive (true);
			asrc [0].Play();
			asrc [2].Stop();
		}
	}
	#endif
	public void SetPoint(int ind){
		PlayerPrefs.SetInt ("yomama", ind);
	}
	public int GetPoint(){
		if (PlayerPrefs.HasKey ("yomama")) {
			return PlayerPrefs.GetInt ("yomama");
		} else
			return 10;
	}
	void checkScore(){
		int scoreVal = int.Parse (score.text);
		if (scoreVal == 10) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQAg", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 15) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQDg", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 25) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQCQ", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 50) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQBA", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 100) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQAw", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 150) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQBQ", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 250) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQBg", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 350) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQBw", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 450) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQCA", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 650) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQCg", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 750) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQCw", 100.0f, (bool success) => {
			});
		} else if (scoreVal == 1000) {
			Social.ReportProgress("CgkI5aWmsr0QEAIQDA", 100.0f, (bool success) => {
			});
		}
	}
}
