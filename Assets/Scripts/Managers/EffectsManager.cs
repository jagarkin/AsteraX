﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Events;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.VFX;
using Object = UnityEngine.Object;
using Random = System.Random;

/// <summary>
/// Responsible for spawning/re-using effects.
/// <para>Requires that particle systems be prefabs with a <see cref="ParticleEffect"/> MonoBehavior.</para>
/// <para>Each <see cref="ParticleSystem"/> should have its 'StopAction' set to Callback.</para>
/// </summary>
public class EffectsManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject ParentContainer;
    private DynamicPoolGroup<ParticleEffect> _poolsGroup;
    public int StartingPoolSize = 5;

    private void Awake()
    {
        _poolsGroup = new DynamicPoolGroup<ParticleEffect>
        {
            ParentContainer = ParentContainer,
            StartingPoolSize = StartingPoolSize,
            OnSpawnedAction = OnSpawned,
        };
    }

    public ParticleEffect Spawn(GameObject prefab, Transform parentContainer)
    {
        var pool = _poolsGroup.GetPoolForPrefab(prefab);
        var effect = pool.Spawn(parentContainer.position, parentContainer.rotation);
        effect.transform.parent = parentContainer;
        return effect;
    }

    public ParticleEffect Spawn(GameObject prefab, Vector3 position, Quaternion? rotation = default, Vector3 scale = default)
    {
        var pool = _poolsGroup.GetPoolForPrefab(prefab);
        var effect = pool.Spawn(position, rotation ?? Quaternion.identity);
        if (scale != default)
        {
            effect.transform.localScale = scale;
        }
        return effect;
    }

    private void OnSpawned(ParticleEffect effect)
    {
        effect.Restart();        
    }
}