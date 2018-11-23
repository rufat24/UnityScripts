  //here are the sample of the sound. The frequency of this sound was 44100 hz and for every frame we got 64 samples
//therefore 441000/64=690hz. the "aplitude" of sounds of frequencies between 0hz-690hz will be averages on sample[0]
//690-1380hz in samples[1] etc.
//For noise canselation I suggest to parse the sound of the noise and original sound with noise simultaneously. And try to decrease the values 
//of the samples in original with noise sound, correspondingly to high values in the noise. 
for(int i=;i<1024/*or whatever the samples overall number*/;i++)
originsample[i]=originsample[i]<noisesample[i] ? 0 : originsample[i]-noisesample[i];

//If the noise in the noise sound is not the same as the noise in the "original sound with noise", isuggest you firstly to parse the sound with noise
//find the average values of the sample each sample during all sound. Using this information, proportionally to the values of the averages of the samples
float [] averageofnoisesample =new float [1024]
long n how many sets of sample we get for this peice of sound
for all period of sound{
n++;
for(int i..i<1024...){
  averageofnoisesample[i]+=noisesample;
}
}
for(int i i<1024...)
averagenoisesample[i]/=n;
//now you will have in average the most active frequencies in the noise, you can proportionally decrease the values of th samples in original sound with noise











using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	float camAverage;
	float lightAverage;
	FastMobileBloom fmb;
	Camera cam;
	AudioSource aud;
  float[] samples = new float[64];
	void Start() {
		fmb = gameObject.GetComponent<FastMobileBloom> ();
		cam = gameObject.GetComponent<Camera> ();
	}
	void Update () {
		camAverage = 0f;
		AudioListener.GetSpectrumData (samples, 0, FFTWindow.Blackman);
		camAverage = (samples [0] + samples [1])*1.5f;
		for (int i = 2; i <= 32; i++) {
			lightAverage += samples [i];
		}
		lightAverage /= 12;
		cam.fieldOfView = 50f + camAverage;
		fmb.threshold = 0.5f - lightAverage;
	}
}
