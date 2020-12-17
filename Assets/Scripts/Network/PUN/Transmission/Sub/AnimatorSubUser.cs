using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Photon.Pun.PhotonAnimatorView;

/// <summary>
/// this attach to local with Animator
/// </summary>
[RequireComponent(typeof (Animator))]
public class AnimatorSubUser : MonoBehaviour
{
    public Animator m_Animator;
    public AnimatorSubAdditive asAssitive;

    /// <summary>
    /// Cached raised triggers that are set to be synchronized in discrete mode. since a Trigger only stay up for less than a frame,
    /// We need to cache it until the next discrete serialization call.
    /// </summary>
    public List<string> m_raisedDiscreteTriggersCache = new List<string>();

    [SerializeField]
    private List<SynchronizedParameter> m_SynchronizeParameters = new List<SynchronizedParameter>();
    public List<SynchronizedParameter> SynchronizeParameters
    {
        get
        {
            return m_SynchronizeParameters;
        }

        set
        {
            m_SynchronizeParameters = value;
        }
    }

    [SerializeField]
    private List<SynchronizedLayer> m_SynchronizeLayers = new List<SynchronizedLayer>();
    public List<SynchronizedLayer> SynchronizeLayers
    {
        get {
            return m_SynchronizeLayers;
        }

        set {
            m_SynchronizeLayers = value;
        }
    }


    #region Setup Synchronizing Methods
    /// <summary>
    /// Caches the discrete triggers values for keeping track of raised triggers, and will be reseted after the sync routine got performed
    /// </summary>
    public void CacheDiscreteTriggers()
    {
        for (int i = 0; i < SynchronizeParameters.Count; ++i)
        {
            SynchronizedParameter parameter = SynchronizeParameters[i];

            if (parameter.SynchronizeType == SynchronizeType.Discrete && parameter.Type == ParameterType.Trigger && this.m_Animator.GetBool(parameter.Name))
            {
                if (parameter.Type == ParameterType.Trigger)
                {
                    asAssitive.m_raisedDiscreteTriggersCache.Add(parameter.Name);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Check if a specific layer is configured to be synchronize
    /// </summary>
    /// <param name="layerIndex">Index of the layer.</param>
    /// <returns>True if the layer is synchronized</returns>
    public bool DoesLayerSynchronizeTypeExist(int layerIndex)
    {
        return SynchronizeLayers.FindIndex(item => item.LayerIndex == layerIndex) != -1;
    }

    /// <summary>
    /// Check if the specified parameter is configured to be synchronized
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>True if the parameter is synchronized</returns>
    public bool DoesParameterSynchronizeTypeExist(string name)
    {
        return SynchronizeParameters.FindIndex(item => item.Name == name) != -1;
    }

    /// <summary>
    /// Get a list of all synchronized layers
    /// </summary>
    /// <returns>List of SynchronizedLayer objects</returns>
    public List<SynchronizedLayer> GetSynchronizedLayers()
    {
        return SynchronizeLayers;
    }

    /// <summary>
    /// Get a list of all synchronized parameters
    /// </summary>
    /// <returns>List of SynchronizedParameter objects</returns>
    public List<SynchronizedParameter> GetSynchronizedParameters()
    {
        return SynchronizeParameters;
    }

    /// <summary>
    /// Gets the type how the layer is synchronized
    /// </summary>
    /// <param name="layerIndex">Index of the layer.</param>
    /// <returns>Disabled/Discrete/Continuous</returns>
    public SynchronizeType GetLayerSynchronizeType(int layerIndex)
    {
        int index = SynchronizeLayers.FindIndex(item => item.LayerIndex == layerIndex);

        if (index == -1)
        {
            return SynchronizeType.Disabled;
        }

        return SynchronizeLayers[index].SynchronizeType;
    }

    /// <summary>
    /// Gets the type how the parameter is synchronized
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>Disabled/Discrete/Continuous</returns>
    public SynchronizeType GetParameterSynchronizeType(string name)
    {
        int index = SynchronizeParameters.FindIndex(item => item.Name == name);

        if (index == -1)
        {
            return SynchronizeType.Disabled;
        }

        return SynchronizeParameters[index].SynchronizeType;
    }

    /// <summary>
    /// Sets the how a layer should be synchronized
    /// </summary>
    /// <param name="layerIndex">Index of the layer.</param>
    /// <param name="synchronizeType">Disabled/Discrete/Continuous</param>
    public void SetLayerSynchronized(int layerIndex, SynchronizeType synchronizeType)
    {
        if (Application.isPlaying == true)
        {
            asAssitive.m_WasSynchronizeTypeChanged = true;
        }

        int index = SynchronizeLayers.FindIndex(item => item.LayerIndex == layerIndex);

        if (index == -1)
        {
            SynchronizeLayers.Add(new SynchronizedLayer { LayerIndex = layerIndex, SynchronizeType = synchronizeType });
        }
        else
        {
            SynchronizeLayers[index].SynchronizeType = synchronizeType;
        }
    }

    /// <summary>
    /// Sets the how a parameter should be synchronized
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="type">The type of the parameter.</param>
    /// <param name="synchronizeType">Disabled/Discrete/Continuous</param>
    public void SetParameterSynchronized(string name, ParameterType type, SynchronizeType synchronizeType)
    {
        if (Application.isPlaying == true)
        {
            asAssitive.m_WasSynchronizeTypeChanged = true;
        }

        int index = SynchronizeParameters.FindIndex(item => item.Name == name);

        if (index == -1)
        {
            SynchronizeParameters.Add(new SynchronizedParameter { Name = name, Type = type, SynchronizeType = synchronizeType });
        }
        else
        {
            SynchronizeParameters[index].SynchronizeType = synchronizeType;
        }
    }

    #endregion
}
