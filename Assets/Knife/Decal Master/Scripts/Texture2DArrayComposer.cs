///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.DeferredDecals
{
    public class Texture2DArrayComposer
    {
        public bool alwaysGenerateOnUpdate;

        private readonly bool _linear;
        private readonly TextureFormat _requiredTextureFormat;
        private readonly List<Texture> _texturesList;
        private int _requiredSizeX;
        private int _requiredSizeY;

        public event EventHandler OnTextureUpdated;

        public Texture2DArrayComposer(int sizeX, int sizeY, TextureFormat format, bool bypassSrgb)
        {
            _texturesList = new List<Texture>();
            RequiredSizeX = sizeX;
            RequiredSizeY = sizeY;
            _requiredTextureFormat = format;
            _linear = bypassSrgb;
        }

        public int TextureCount
        {
            get
            {
                return _texturesList.Count;
            }
        }

        public Texture2DArray Texture
        {
            get;
            private set;
        }

        public bool HasTexture
        {
            get;
            private set;
        }

        public bool NeedsToUpdateTexture
        {
            get;
            private set;
        }

        public int RequiredSizeX
        {
            get
            {
                return _requiredSizeX;
            }

            private set
            {
                _requiredSizeX = value;
            }
        }

        public int RequiredSizeY
        {
            get
            {
                return _requiredSizeY;
            }

            private set
            {
                _requiredSizeY = value;
            }
        }

        public void RaiseTextureUpdatedEvent()
        {
            if(OnTextureUpdated != null)
            {
                OnTextureUpdated(this, new EventArgs());
            }
        }

        public bool AddTexture(Texture texture)
        {
            if(texture != null)
            {
                if(texture.height != RequiredSizeY || texture.width != RequiredSizeX)
                {
                    Debug.LogError("Pixel sizes of texture \"" + texture + "\" does not match the required size of " + RequiredSizeX + "pixels for width and " + RequiredSizeY + "pixels for height.", texture);
                    return false;
                }

                if(!_texturesList.Contains(texture))
                {
                    _texturesList.Add(texture);
                    NeedsToUpdateTexture = true;
                    return true;
                }
            }

            return false;
        }

        public bool RemoveTexture(Texture texture)
        {
            if(_texturesList.Contains(texture))
            {
                _texturesList.Remove(texture);
                NeedsToUpdateTexture = true;

                return true;
            }

            return false;
        }

        public void Generate()
        {
            if(NeedsToUpdateTexture || alwaysGenerateOnUpdate)
            {
                if(_texturesList.Count > 0)
                {
                    if(NeedsToUpdateTexture)
                    {
                        Texture = new Texture2DArray(RequiredSizeX, RequiredSizeY, _texturesList.Count, _requiredTextureFormat, false, _linear);
                    }

                    for(int i = 0; i < _texturesList.Count; ++i)
                    {
                        Graphics.CopyTexture(_texturesList[i], 0, 0, 0, 0, RequiredSizeX, RequiredSizeY, Texture, i, 0, 0, 0);
                    }

                    HasTexture = true;
                }
                else
                {
                    Texture = null;
                    HasTexture = false;
                }

                NeedsToUpdateTexture = false;

                RaiseTextureUpdatedEvent();
            }
        }

        public int GetTextureIndex(Texture texture)
        {
            return _texturesList.IndexOf(texture);
        }

        public void ClearTexturesList()
        {
            _texturesList.Clear();
        }

        public void Resize(int sizeX, int sizeY)
        {
            RequiredSizeX = sizeX;
            RequiredSizeY = sizeY;
            ClearTexturesList();
            NeedsToUpdateTexture = true;
        }
    }
}
