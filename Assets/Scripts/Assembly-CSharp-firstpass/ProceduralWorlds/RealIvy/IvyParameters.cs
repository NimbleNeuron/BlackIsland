using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class IvyParameters
	{
		public float stepSize = 0.1f;


		public int randomSeed;


		public float branchProvability = 0.05f;


		public int maxBranchs = 5;


		public LayerMask layerMask = -1;


		public float minDistanceToSurface = 0.01f;


		public float maxDistanceToSurface = 0.03f;


		public float DTSFrequency = 1f;


		public float DTSRandomness = 0.2f;


		public float directionFrequency = 1f;


		public float directionAmplitude = 20f;


		public float directionRandomness = 1f;


		public Vector3 gravity;


		public float grabProvabilityOnFall = 0.1f;


		public float stiffness = 0.03f;


		public float optAngleBias = 15f;


		public int leaveEvery = 1;


		public int randomLeaveEvery = 1;


		public bool buffer32Bits;


		public bool halfgeom;


		public int sides = 3;


		public float minRadius = 0.025f;


		public float maxRadius = 0.05f;


		public float radiusVarFreq = 1f;


		public float radiusVarOffset;


		public float tipInfluence = 0.5f;


		public Vector2 uvScale = new Vector2(1f, 1f);


		public Vector2 uvOffset = new Vector2(0f, 0f);


		public float minScale = 0.7f;


		public float maxScale = 1.2f;


		public bool globalOrientation;


		public Vector3 globalRotation = -Vector3.up;


		public Vector3 rotation = Vector3.zero;


		public Vector3 randomRotation = Vector3.zero;


		public Vector3 offset = Vector3.zero;


		public float LMUVPadding = 0.002f;


		public Material branchesMaterial;


		public GameObject[] leavesPrefabs = new GameObject[0];


		public float[] leavesProb = new float[0];


		public bool generateBranches;


		public bool generateLeaves;


		public bool generateLightmapUVs;


		public Dictionary<int, Vector2> UV2IslesSizes;


		public IvyParameters() { }


		public IvyParameters(IvyParameters copyFrom)
		{
			CopyFrom(copyFrom);
		}


		public void CopyFrom(IvyPreset ivyPreset)
		{
			CopyFrom(ivyPreset.ivyParameters);
		}


		public void CopyFrom(IvyParametersGUI copyFrom)
		{
			stepSize = copyFrom.stepSize;
			branchProvability = copyFrom.branchProvability;
			maxBranchs = copyFrom.maxBranchs;
			layerMask = copyFrom.layerMask;
			minDistanceToSurface = copyFrom.minDistanceToSurface;
			maxDistanceToSurface = copyFrom.maxDistanceToSurface;
			DTSFrequency = copyFrom.DTSFrequency;
			DTSRandomness = copyFrom.DTSRandomness;
			directionFrequency = copyFrom.directionFrequency;
			directionAmplitude = copyFrom.directionAmplitude;
			directionRandomness = copyFrom.directionRandomness;
			gravity = new Vector3(copyFrom.gravityX, copyFrom.gravityY, copyFrom.gravityZ);
			grabProvabilityOnFall = copyFrom.grabProvabilityOnFall;
			stiffness = copyFrom.stiffness;
			optAngleBias = copyFrom.optAngleBias;
			leaveEvery = copyFrom.leaveEvery;
			randomLeaveEvery = copyFrom.randomLeaveEvery;
			buffer32Bits = copyFrom.buffer32Bits;
			halfgeom = copyFrom.halfgeom;
			sides = copyFrom.sides;
			minRadius = copyFrom.minRadius;
			maxRadius = copyFrom.maxRadius;
			radiusVarFreq = copyFrom.radiusVarFreq;
			radiusVarOffset = copyFrom.radiusVarOffset;
			tipInfluence = copyFrom.tipInfluence;
			uvScale = new Vector2(copyFrom.uvScaleX, copyFrom.uvScaleY);
			uvOffset = new Vector2(copyFrom.uvOffsetX, copyFrom.uvOffsetY);
			minScale = copyFrom.minScale;
			maxScale = copyFrom.maxScale;
			globalOrientation = copyFrom.globalOrientation;
			globalRotation = new Vector3(copyFrom.globalRotationX, copyFrom.globalRotationY, copyFrom.globalRotationZ);
			rotation = new Vector3(copyFrom.rotationX, copyFrom.rotationY, copyFrom.rotationZ);
			randomRotation = new Vector3(copyFrom.randomRotationX, copyFrom.randomRotationY, copyFrom.randomRotationZ);
			offset = new Vector3(copyFrom.offsetX, copyFrom.offsetY, copyFrom.offsetZ);
			LMUVPadding = copyFrom.LMUVPadding;
			generateBranches = copyFrom.generateBranches;
			generateLeaves = copyFrom.generateLeaves;
			generateLightmapUVs = copyFrom.generateLightmapUVs;
			branchesMaterial = copyFrom.branchesMaterial;
			leavesPrefabs = new GameObject[copyFrom.leavesPrefabs.Count];
			for (int i = 0; i < copyFrom.leavesPrefabs.Count; i++)
			{
				leavesPrefabs[i] = copyFrom.leavesPrefabs[i];
			}

			leavesProb = new float[copyFrom.leavesProb.Count];
			for (int j = 0; j < copyFrom.leavesProb.Count; j++)
			{
				leavesProb[j] = copyFrom.leavesProb[j];
			}
		}


		public void CopyFrom(IvyParameters copyFrom)
		{
			stepSize = copyFrom.stepSize;
			branchProvability = copyFrom.branchProvability;
			maxBranchs = copyFrom.maxBranchs;
			layerMask = copyFrom.layerMask;
			minDistanceToSurface = copyFrom.minDistanceToSurface;
			maxDistanceToSurface = copyFrom.maxDistanceToSurface;
			DTSFrequency = copyFrom.DTSFrequency;
			DTSRandomness = copyFrom.DTSRandomness;
			directionFrequency = copyFrom.directionFrequency;
			directionAmplitude = copyFrom.directionAmplitude;
			directionRandomness = copyFrom.directionRandomness;
			gravity = copyFrom.gravity;
			grabProvabilityOnFall = copyFrom.grabProvabilityOnFall;
			stiffness = copyFrom.stiffness;
			optAngleBias = copyFrom.optAngleBias;
			leaveEvery = copyFrom.leaveEvery;
			randomLeaveEvery = copyFrom.randomLeaveEvery;
			halfgeom = copyFrom.halfgeom;
			sides = copyFrom.sides;
			minRadius = copyFrom.minRadius;
			maxRadius = copyFrom.maxRadius;
			radiusVarFreq = copyFrom.radiusVarFreq;
			radiusVarOffset = copyFrom.radiusVarOffset;
			tipInfluence = copyFrom.tipInfluence;
			uvScale = copyFrom.uvScale;
			uvOffset = copyFrom.uvOffset;
			minScale = copyFrom.minScale;
			maxScale = copyFrom.maxScale;
			globalOrientation = copyFrom.globalOrientation;
			globalRotation = copyFrom.globalRotation;
			rotation = copyFrom.rotation;
			randomRotation = copyFrom.randomRotation;
			offset = copyFrom.offset;
			LMUVPadding = copyFrom.LMUVPadding;
			generateBranches = copyFrom.generateBranches;
			generateLeaves = copyFrom.generateLeaves;
			generateLightmapUVs = copyFrom.generateLightmapUVs;
			branchesMaterial = copyFrom.branchesMaterial;
			leavesPrefabs = new GameObject[copyFrom.leavesPrefabs.Length];
			for (int i = 0; i < copyFrom.leavesPrefabs.Length; i++)
			{
				leavesPrefabs[i] = copyFrom.leavesPrefabs[i];
			}

			leavesProb = new float[copyFrom.leavesProb.Length];
			for (int j = 0; j < copyFrom.leavesProb.Length; j++)
			{
				leavesProb[j] = copyFrom.leavesProb[j];
			}
		}


		public bool IsEqualTo(IvyParameters compareTo)
		{
			return stepSize == compareTo.stepSize && branchProvability == compareTo.branchProvability &&
			       maxBranchs == compareTo.maxBranchs && layerMask == compareTo.layerMask &&
			       minDistanceToSurface == compareTo.minDistanceToSurface &&
			       maxDistanceToSurface == compareTo.maxDistanceToSurface && DTSFrequency == compareTo.DTSFrequency &&
			       DTSRandomness == compareTo.DTSRandomness && directionFrequency == compareTo.directionFrequency &&
			       directionAmplitude == compareTo.directionAmplitude &&
			       directionRandomness == compareTo.directionRandomness && gravity == compareTo.gravity &&
			       grabProvabilityOnFall == compareTo.grabProvabilityOnFall && stiffness == compareTo.stiffness &&
			       optAngleBias == compareTo.optAngleBias && leaveEvery == compareTo.leaveEvery &&
			       randomLeaveEvery == compareTo.randomLeaveEvery && buffer32Bits == compareTo.buffer32Bits &&
			       halfgeom == compareTo.halfgeom && sides == compareTo.sides && minRadius == compareTo.minRadius &&
			       maxRadius == compareTo.maxRadius && radiusVarFreq == compareTo.radiusVarFreq &&
			       radiusVarOffset == compareTo.radiusVarOffset && tipInfluence == compareTo.tipInfluence &&
			       uvScale == compareTo.uvScale && uvOffset == compareTo.uvOffset && minScale == compareTo.minScale &&
			       maxScale == compareTo.maxScale && globalOrientation == compareTo.globalOrientation &&
			       globalRotation == compareTo.globalRotation && rotation == compareTo.rotation &&
			       randomRotation == compareTo.randomRotation && offset == compareTo.offset &&
			       LMUVPadding == compareTo.LMUVPadding && branchesMaterial == compareTo.branchesMaterial &&
			       leavesPrefabs.SequenceEqual(compareTo.leavesPrefabs) &&
			       leavesProb.SequenceEqual(compareTo.leavesProb) && generateBranches == compareTo.generateBranches &&
			       generateLeaves == compareTo.generateLeaves && generateLightmapUVs == compareTo.generateLightmapUVs;
		}
	}
}