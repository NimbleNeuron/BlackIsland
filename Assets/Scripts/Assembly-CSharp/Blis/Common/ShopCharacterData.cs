namespace Blis.Common
{
	public class ShopCharacterData
	{
		public readonly int amount;


		public readonly int characterCode;


		public readonly string img;


		public readonly string searchName;


		private ProductCharacterData aCoinData;


		private ProductCharacterData npData;


		public ShopCharacterData(ProductCharacterData data1, ProductCharacterData data2)
		{
			amount = data1.amount;
			img = data1.img;
			searchName = data1.searchName;
			characterCode = data1.characterCode;
			if (data1.purchaseMethod == PurchaseMethod.NP)
			{
				SetNPData(data1);
				SetACoinData(data2);
				return;
			}

			SetNPData(data2);
			SetACoinData(data1);
		}


		public ProductCharacterData NPData => npData;


		public ProductCharacterData ACoinData => aCoinData;


		private void SetNPData(ProductCharacterData data)
		{
			npData = data;
		}


		private void SetACoinData(ProductCharacterData data)
		{
			aCoinData = data;
		}
	}
}