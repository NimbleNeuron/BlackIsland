using UnityEngine;
using System.Collections;

namespace Knife.DeferredDecals.Spawn
{
    public abstract class DecalSpawner : MonoBehaviour
    {
        public abstract Decal Spawn();
        public abstract void DestroyDecal(Decal decal);
        public void DestroyDecal(Decal decal, float delay)
        {
            StartCoroutine(DestroyDecalWithDelay(decal, delay));
        }

        IEnumerator DestroyDecalWithDelay(Decal decal, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (decal == null)
                yield break;

            DestroyDecal(decal);
            decal.gameObject.SetActive(false);
        }
    }
}