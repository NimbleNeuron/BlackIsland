using UnityEngine;
using UnityEngine.EventSystems;


public class WeaponMasterBlock : MonoBehaviour
{
	
	public void Focus()
	{
		this.selected.SetActive(true);
	}

	
	public void Fill()
	{
		this.on.SetActive(true);
	}

	
	public void Clear()
	{
		this.on.SetActive(false);
		this.selected.SetActive(false);
	}

	
	public void OnPointerEnter(BaseEventData eventData)
	{
		this.rollover.SetActive(true);
	}

	
	public void OnPointerExit(BaseEventData eventData)
	{
		this.rollover.SetActive(false);
	}

	
	[SerializeField]
	private GameObject bg = default;

	
	[SerializeField]
	private GameObject on = default;

	
	[SerializeField]
	private GameObject selected = default;

	
	[SerializeField]
	private GameObject rollover = default;
	
	private void Ref()
	{
		Reference.Use(bg);
		Reference.Use(on);
		Reference.Use(selected);
		Reference.Use(rollover);
	}
}
