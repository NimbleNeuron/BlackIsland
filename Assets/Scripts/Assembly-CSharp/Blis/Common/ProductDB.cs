using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class ProductDB
	{
		private List<ProductAssetData> productAssetDataList;


		private List<ProductCharacterData> productCharacterDataList;


		private List<ProductInstantData> productInstantDataList;


		private Dictionary<string, Product> productMap;


		private List<ShopCharacterData> shopCharacterDataList;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(ProductAssetData))
			{
				productAssetDataList = data.Cast<ProductAssetData>().ToList<ProductAssetData>();
				return;
			}

			if (typeFromHandle == typeof(ProductCharacterData))
			{
				productCharacterDataList = data.Cast<ProductCharacterData>().ToList<ProductCharacterData>();
				return;
			}

			if (typeFromHandle == typeof(ProductInstantData))
			{
				productInstantDataList = data.Cast<ProductInstantData>().ToList<ProductInstantData>();
			}
		}


		public void PostInitialize()
		{
			Dictionary<string, Product> dictionary = new Dictionary<string, Product>();
			AddToMap<ProductAssetData>(dictionary, typeof(ProductAssetData), productAssetDataList);
			AddToMap<ProductCharacterData>(dictionary, typeof(ProductCharacterData), productCharacterDataList);
			productMap = new Dictionary<string, Product>(dictionary);
			shopCharacterDataList = new List<ShopCharacterData>();
			using (List<CharacterData>.Enumerator enumerator = GameDB.character.GetAllCharacterData().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CharacterData characterData = enumerator.Current;
					List<ProductCharacterData> list =
						productCharacterDataList.FindAll(p => p.characterCode == characterData.code);
					if (list.Count >= 2)
					{
						ShopCharacterData item = new ShopCharacterData(list[0], list[1]);
						shopCharacterDataList.Add(item);
					}
				}
			}
		}


		public ProductAssetData FindProductAsset(string productId)
		{
			return productAssetDataList.Find(p => p.productId == productId);
		}


		public ProductCharacterData FindProductCharacter(string productId)
		{
			return productCharacterDataList.Find(p => p.productId == productId);
		}


		public Product FindProduct(int characterCode, PurchaseMethod purchaseMethod)
		{
			return productMap.First(p =>
				p.Value.goods.goodsType == GoodsType.CHARACTER && int.Parse(p.Value.goods.subType) == characterCode &&
				p.Value.purchaseMethod == purchaseMethod).Value;
		}


		public List<Product> FindProduct(GoodsType goodsType)
		{
			List<Product> list = new List<Product>();
			foreach (KeyValuePair<string, Product> keyValuePair in productMap)
			{
				if (keyValuePair.Value.goods.goodsType == goodsType)
				{
					list.Add(keyValuePair.Value);
				}
			}

			return list;
		}


		public ShopCharacterData FindShopCharacter(int characterCode)
		{
			return shopCharacterDataList.Find(s => s.characterCode == characterCode);
		}


		public List<ProductInstantData> FindProductInstant()
		{
			return productInstantDataList;
		}


		public List<ShopCharacterData> GetAllShopCharacter()
		{
			return shopCharacterDataList;
		}


		private void AddToMap<T>(Dictionary<string, Product> map, Type type, List<T> dataList)
		{
			foreach (T t in dataList)
			{
				try
				{
					Product product = (Product) type.GetField("product").GetValue(t);
					map.Add(product.productId, product);
				}
				catch (Exception) { }
			}
		}
	}
}