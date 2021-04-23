using System.Collections.Generic;

namespace Blis.Common
{
	public class ShopProduct
	{
		public int code;


		public string content;


		public string iapProductId;


		public int id;


		public string img;


		public int price;


		public int PriceACoin = -1;


		public int PriceNP = -1;


		public string ProductACoinId;


		public string productId;


		public int productItemId;


		public string ProductNPId;


		public PurchaseCondition purchaseCondition;


		public PurchaseType purchaseType;


		public List<ShopProductItem> shopProductItems;


		public ShopType shopType;


		public string title;

		public ShopProduct() { }


		public ShopProduct(ShopProduct data)
		{
			id = data.id;
			productId = data.productId;
			shopType = data.shopType;
			purchaseType = data.purchaseType;
			iapProductId = data.iapProductId;
			purchaseCondition = data.purchaseCondition;
			price = data.price;
			productItemId = data.productItemId;
			title = data.title;
			content = data.content;
			code = data.code;
			img = data.img;
			shopProductItems = data.shopProductItems;
			if (data.purchaseType == PurchaseType.NP)
			{
				PriceNP = data.price;
				ProductNPId = data.productId;
			}

			if (data.purchaseType == PurchaseType.ACOIN)
			{
				PriceACoin = data.price;
				ProductACoinId = data.productId;
			}
		}


		public ShopProduct(ShopProduct data1, ShopProduct data2)
		{
			id = data1.id;
			productId = data1.productId;
			shopType = data1.shopType;
			purchaseType = data1.purchaseType;
			iapProductId = data1.iapProductId;
			purchaseCondition = data1.purchaseCondition;
			price = data1.price;
			productItemId = data1.productItemId;
			title = data1.title;
			content = data1.content;
			code = data1.code;
			img = data1.img;
			shopProductItems = data1.shopProductItems;
			if (data1.purchaseType == PurchaseType.NP)
			{
				PriceNP = data1.price;
				PriceACoin = data2.price;
				ProductNPId = data1.productId;
				ProductACoinId = data2.productId;
			}

			if (data1.purchaseType == PurchaseType.ACOIN)
			{
				PriceNP = data2.price;
				PriceACoin = data1.price;
				ProductNPId = data2.productId;
				ProductACoinId = data1.productId;
			}
		}
	}
}