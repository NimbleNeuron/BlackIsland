using UnityEngine;
using UnityEngine.UI;

namespace Vuplex.WebView
{
	
	public class CanvasViewportMaterialView : ViewportMaterialView
	{
		
		
		
		public override Material Material
		{
			get
			{
				return base.GetComponent<RawImage>().material;
			}
			set
			{
				base.GetComponent<RawImage>().material = value;
			}
		}

		
		
		
		public override Texture2D Texture
		{
			get
			{
				return (Texture2D)base.GetComponent<RawImage>().material.mainTexture;
			}
			set
			{
				base.GetComponent<RawImage>().material.mainTexture = value;
			}
		}

		
		
		
		public override bool Visible
		{
			get
			{
				return base.GetComponent<RawImage>().enabled;
			}
			set
			{
				base.GetComponent<RawImage>().enabled = value;
			}
		}

		
		public override void SetCropRect(Rect rect)
		{
			base.GetComponent<RawImage>().material.SetVector("_CropRect", this._rectToVector(rect));
		}

		
		public override void SetCutoutRect(Rect rect)
		{
			Vector4 vector = this._rectToVector(rect);
			if (rect != new Rect(0f, 0f, 1f, 1f))
			{
				float num = rect.width * 0.01f;
				float num2 = rect.height * 0.01f;
				vector = new Vector4(vector.x + num, vector.y + num2, vector.z - 2f * num, vector.w - 2f * num2);
			}
			base.GetComponent<RawImage>().material.SetVector("_VideoCutoutRect", vector);
		}

		
		public override void SetStereoToMonoOverride(bool overrideStereoToMono)
		{
			base.GetComponent<RawImage>().material.SetFloat("_OverrideStereoToMono", overrideStereoToMono ? 1f : 0f);
		}

		
		private Vector4 _rectToVector(Rect rect)
		{
			return new Vector4(rect.x, rect.y, rect.width, rect.height);
		}
	}
}
