using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Photon.Pun.PhotonAnimatorView;

/// <summary>
/// this attach to local with Animator
/// </summary>
public class AnimatorSubUser : MonoBehaviour
{ 
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
}
