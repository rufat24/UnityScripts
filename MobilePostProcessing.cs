using System;
using UnityEngine;

public class MobilePostProcessing : MonoBehaviour{
	[Range(0, 1)]
	public float Amount = 1.0f;
	[Range(0, 20)]
	public float BlurAmount = 2f;

	static readonly int lutTextureString = Shader.PropertyToID ("_LutTex");
	static readonly int amountString = Shader.PropertyToID ("_Amount");
	static readonly int maskTextureString = Shader.PropertyToID ("_MaskTex");
	static readonly int dimString= Shader.PropertyToID ("_Dim");
	static readonly int scaleString = Shader.PropertyToID ("_ScaleRG");
	static readonly int offsetString= Shader.PropertyToID ("_Offset");
	static readonly int bAmountString = Shader.PropertyToID ("_BlurAmount");
	static readonly int scrResString= Shader.PropertyToID ("_ScrRes");
	static readonly int blurTexString= Shader.PropertyToID ("_BlurTex");
	static readonly int scrWidth=Screen.width;
	static readonly int scrHeight=Screen.height;

	public Shader Shader;
	private Shader previousShader;
	private Material material;
	public Texture2D sourceLut = null;
	private Texture2D prevSorceLut;
	private Texture2D converted2DLut = null;
	public Texture2D maskText;
	public string basedOnTempTex = "";
    private int lutSize;
	public void Awake(){
	}
    private void CreateMaterial()
    {
        if (Shader == null)
        {
            material = null;
            Debug.LogError("Must set a shader to use LUT");
            return;
        }

        material = new Material(Shader);
        material.hideFlags = HideFlags.DontSave;
    }

    private void OnEnable()
    {
        /*if (GetComponent<Camera>() == null)
        {
            Debug.LogError("This script must be attached to a Camera");
        }*/
    }

    private void Update()
    {
        if (Shader != previousShader)
        {
            previousShader = Shader;
            CreateMaterial();
        }

		if (sourceLut != prevSorceLut)
        {
			prevSorceLut = sourceLut;
			Convert(sourceLut,"");
        }
    }

    private void OnDestroy()
    {
        if (converted2DLut != null)
        {
            DestroyImmediate(converted2DLut);
        }
        converted2DLut = null;
    }

	public void  SetIdentityLut (){
		int dim = 16;
		Color[] newC = new Color[dim*dim*dim*dim];
		float oneOverDim = 1.0f / (1.0f * dim - 1.0f);

		for(int i = 0; i < dim; i++) {
			for(int j = 0; j < dim; j++) {
				for(int x = 0; x < dim; x++) {
					for(int y = 0; y < dim; y++) 
					{
						newC[x + i * dim + y * dim * dim + j * dim * dim * dim] = 
							new Color(x * oneOverDim, y * oneOverDim, (j * dim + i) / (dim * dim - 1.0f), 1);
					}
				}
			}
		}

		if (converted2DLut)
			DestroyImmediate (converted2DLut);
		converted2DLut = new Texture2D (dim * dim, dim * dim, TextureFormat.ARGB32, false);
		converted2DLut.SetPixels (newC);
		converted2DLut.Apply ();
		basedOnTempTex = "";		
	}
	public bool ValidDimensions ( Texture2D tex2d  ){
		if (!tex2d) return false;
		int h = tex2d.height;
		if (h != Mathf.FloorToInt(Mathf.Sqrt(tex2d.width))) {
			return false;				
		}
		// we do not support other sizes than 256x16 
		if (h != 16) {
			return false;				
		}
		return true;
	}
	public void  Convert ( Texture2D temp2DTex ,   string path  ){
		if (temp2DTex) {
			int dim = temp2DTex.width * temp2DTex.height;
			dim = temp2DTex.height;
			if (!ValidDimensions(temp2DTex)) {
				Debug.LogWarning ("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");				
				basedOnTempTex = "";
				return;				
			}

			Color[] c = temp2DTex.GetPixels();
			Color[] newC = new Color[dim * dim * dim * dim];

			for(int i = 0; i < dim; i++) {
				for(int j = 0; j < dim; j++) 
				{
					for(int x = 0; x < dim; x++) {
						for(int y = 0; y < dim; y++) 
						{
							float b = (i + j * dim * 1.0f) / dim;
							int bi0 = Mathf.FloorToInt(b);
							int bi1 = Mathf.Min(bi0 + 1, dim - 1);
							float f = b - bi0;

							int index = x + (dim - y - 1) * dim * dim;
							// perform filtering of B channel in code
							Color col1 = c[index + bi0 * dim];
							Color col2 = c[index + bi1 * dim];

							newC[x + i * dim + y * dim * dim + j * dim * dim * dim] = 
								Color.Lerp(col1, col2, f);
						}
					}
				}
			}

			if (converted2DLut)
				DestroyImmediate (converted2DLut);
			converted2DLut = new Texture2D (dim * dim, dim * dim, TextureFormat.ARGB32, false);
			converted2DLut.SetPixels (newC);
			converted2DLut.Apply ();
			basedOnTempTex = path;
		}		
		else {
			// error, something went terribly wrong
			Debug.LogError ("Couldn't color correct with 2D LUT texture. Image Effect will be disabled.");
		}		
	}


	void  OnRenderImage ( RenderTexture source ,   RenderTexture destination  ){	
		/*if(converted2DLut==null||material==null) {
			Graphics.Blit (source, destination);
			return;
		}

		if (converted2DLut == null) 
		{
			if (sourceLut == null)
				SetIdentityLut ();
			else
				Convert(sourceLut, "");
		}*/

		float lutSize = converted2DLut.width;
		float lutSquare = Mathf.Sqrt(lutSize);
		converted2DLut.wrapMode = TextureWrapMode.Clamp;
		material.SetFloat(scaleString, (lutSquare - 1) / lutSize);
		material.SetFloat(dimString, lutSquare);
		material.SetFloat(offsetString, 1 / (2 * lutSize));		
		material.SetTexture(lutTextureString, converted2DLut);
		material.SetFloat(amountString, Amount);
		material.SetFloat(bAmountString, BlurAmount);
		material.SetVector(scrResString,new Vector2(BlurAmount/scrWidth,BlurAmount/scrHeight));
		material.SetTexture (maskTextureString, maskText);
		RenderTexture buffer = RenderTexture.GetTemporary(scrWidth/4, scrHeight/2, 0);
		Graphics.Blit (source, buffer, material,0);
		material.SetTexture (blurTexString, buffer);
		RenderTexture.ReleaseTemporary(buffer);
		Graphics.Blit (source, destination, material, 1);
	}	
}
