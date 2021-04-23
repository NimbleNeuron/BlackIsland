using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshPainter
{
	[Serializable]
	public class UndoManager : ISerializationCallbackReceiver
	{
		private const int MaxUndo = 20;


		[SerializeField] private int _step = -1;


		private List<State> _undoState;


		public UndoManager()
		{
			if (_undoState == null)
			{
				_undoState = new List<State>(20);
			}
		}


		
		public int Step {
			get => _step;
			set => _step = value;
		}


		
		public bool Initialized { get; set; }


		
		public bool HasUndoRedoPerformed { get; set; }


		public void OnBeforeSerialize() { }


		public void OnAfterDeserialize()
		{
			if (!Initialized)
			{
				Step = -1;
				Initialized = true;
			}
		}


		public void UndoRedoPerformed()
		{
			HasUndoRedoPerformed = true;
			RestoreTexture(Step);
		}


		private void RestoreTexture(int index)
		{
			if (index > -1 && index < _undoState.Count)
			{
				_undoState[index].Restore();
			}
		}


		public void Record(List<Texture2D> textures)
		{
			State state = new State(textures);
			if (_undoState.Count == 0 || Step > _undoState.Count)
			{
				Step = -1;
			}

			int num = Step + 1;
			Step = num;
			if (num == 20)
			{
				Step = 0;
			}

			if (_undoState.Count < 20)
			{
				_undoState.Insert(Step, state);
				return;
			}

			_undoState[Step] = state;
		}


		private class State
		{
			private readonly List<KeyValuePair<Texture2D, Color[]>> _state;


			public State(List<Texture2D> textures)
			{
				_state = new List<KeyValuePair<Texture2D, Color[]>>();
				Store(textures);
			}


			public void Store(List<Texture2D> textures)
			{
				foreach (Texture2D texture2D in textures)
				{
					_state.Add(new KeyValuePair<Texture2D, Color[]>(texture2D, texture2D.GetPixels(0)));
				}
			}


			public void Restore()
			{
				foreach (KeyValuePair<Texture2D, Color[]> keyValuePair in _state)
				{
					keyValuePair.Key.SetPixels(keyValuePair.Value);
					keyValuePair.Key.Apply();
				}
			}
		}
	}
}