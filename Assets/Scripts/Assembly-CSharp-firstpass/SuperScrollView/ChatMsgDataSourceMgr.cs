using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
	public class ChatMsgDataSourceMgr : MonoBehaviour
	{
		private static ChatMsgDataSourceMgr instance = null;


		private static readonly string[] mChatDemoStrList =
		{
			"Support ListView and GridView.",
			"Support Infinity Vertical and Horizontal ScrollView.",
			"Support items in different sizes such as widths or heights. Support items with unknown size at init time.",
			"Support changing item count and item size at runtime. Support looping items such as spinners. Support item padding.",
			"Use only one C# script to help the UGUI ScrollRect to support any count items with high performance."
		};


		private static readonly string[] mChatDemoPicList =
		{
			"grid_pencil_128_g2",
			"grid_flower_200_3",
			"grid_pencil_128_g3",
			"grid_flower_200_7"
		};


		private readonly List<ChatMsg> mChatMsgList = new List<ChatMsg>();


		private readonly Dictionary<int, PersonInfo> mPersonInfoDict = new Dictionary<int, PersonInfo>();


		public static ChatMsgDataSourceMgr Get {
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<ChatMsgDataSourceMgr>();
				}

				return instance;
			}
		}


		public int TotalItemCount => mChatMsgList.Count;


		private void Awake()
		{
			Init();
		}


		public PersonInfo GetPersonInfo(int personId)
		{
			PersonInfo result = null;
			if (mPersonInfoDict.TryGetValue(personId, out result))
			{
				return result;
			}

			return null;
		}


		public void Init()
		{
			mPersonInfoDict.Clear();
			PersonInfo personInfo = new PersonInfo();
			personInfo.mHeadIcon = "grid_pencil_128_g8";
			personInfo.mId = 0;
			personInfo.mName = "Jaci";
			mPersonInfoDict.Add(personInfo.mId, personInfo);
			personInfo = new PersonInfo();
			personInfo.mHeadIcon = "grid_pencil_128_g5";
			personInfo.mId = 1;
			personInfo.mName = "Toc";
			mPersonInfoDict.Add(personInfo.mId, personInfo);
			InitChatDataSource();
		}


		public ChatMsg GetChatMsgByIndex(int index)
		{
			if (index < 0 || index >= mChatMsgList.Count)
			{
				return null;
			}

			return mChatMsgList[index];
		}


		private void InitChatDataSource()
		{
			mChatMsgList.Clear();
			int num = mChatDemoStrList.Length;
			int num2 = mChatDemoPicList.Length;
			for (int i = 0; i < 100; i++)
			{
				ChatMsg chatMsg = new ChatMsg();
				chatMsg.mMsgType = (MsgTypeEnum) (Random.Range(0, 99) % 2);
				chatMsg.mPersonId = Random.Range(0, 99) % 2;
				chatMsg.mSrtMsg = mChatDemoStrList[Random.Range(0, 99) % num];
				chatMsg.mPicMsgSpriteName = mChatDemoPicList[Random.Range(0, 99) % num2];
				mChatMsgList.Add(chatMsg);
			}
		}


		public void AppendOneMsg()
		{
			int num = mChatDemoStrList.Length;
			int num2 = mChatDemoPicList.Length;
			ChatMsg chatMsg = new ChatMsg();
			chatMsg.mMsgType = (MsgTypeEnum) (Random.Range(0, 99) % 2);
			chatMsg.mPersonId = Random.Range(0, 99) % 2;
			chatMsg.mSrtMsg = mChatDemoStrList[Random.Range(0, 99) % num];
			chatMsg.mPicMsgSpriteName = mChatDemoPicList[Random.Range(0, 99) % num2];
			mChatMsgList.Add(chatMsg);
		}
	}
}