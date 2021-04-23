using UnityEngine;

namespace Vuplex.WebView
{
	
	public class ViewportMaterialView : MonoBehaviour
	{
		
		
		
		public virtual Material Material
		{
			get
			{
				return base.GetComponent<Renderer>().sharedMaterial;
			}
			set
			{
				base.GetComponent<Renderer>().sharedMaterial = value;
			}
		}

		
		
		
		public virtual Texture2D Texture
		{
			get
			{
				return (Texture2D)base.GetComponent<Renderer>().sharedMaterial.mainTexture;
			}
			set
			{
				base.GetComponent<Renderer>().sharedMaterial.mainTexture = value;
			}
		}

		
		
		
		public virtual bool Visible
		{
			get
			{
				return base.GetComponent<Renderer>().enabled;
			}
			set
			{
				base.GetComponent<Renderer>().enabled = value;
			}
		}

		
		public virtual void SetCropRect(Rect rect)
		{
			base.GetComponent<Renderer>().sharedMaterial.SetVector("_CropRect", this._rectToVector(rect));
		}

		
		public virtual void SetCutoutRect(Rect rect)
		{
			Vector4 vector = this._rectToVector(rect);
			if (rect != new Rect(0f, 0f, 1f, 1f))
			{
				float num = rect.width * 0.01f;
				float num2 = rect.height * 0.01f;
				vector = new Vector4(vector.x + num, vector.y + num2, vector.z - 2f * num, vector.w - 2f * num2);
			}
			base.GetComponent<Renderer>().sharedMaterial.SetVector("_VideoCutoutRect", vector);
		}

		
		public virtual void SetStereoToMonoOverride(bool overrideStereoToMono)
		{
			base.GetComponent<Renderer>().sharedMaterial.SetFloat("_OverrideStereoToMono", overrideStereoToMono ? 1f : 0f);
		}

		
		private Vector4 _rectToVector(Rect rect)
		{
			return new Vector4(rect.x, rect.y, rect.width, rect.height);
		}
	}
}
