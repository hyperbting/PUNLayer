using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Photon.Pun.PhotonAnimatorView;

/// <summary>
/// based on PhotonAnimatorView
/// can assign Target Animator;
/// </summary>
public class AnimatorSubAdditive : MonoBehaviourPun, IPunObservable
{
    #region Properties

#if PHOTON_DEVELOP
        public PhotonAnimatorView ReceivingSender;
#endif

    #endregion

    #region Members

    private bool TriggerUsageWarningDone;

    public AnimatorSubUser asUser;

    private PhotonStreamQueue m_StreamQueue = new PhotonStreamQueue(120);

    //These fields are only used in the CustomEditor for this script and would trigger a
    //"this variable is never used" warning, which I am suppressing here
#pragma warning disable 0414

    [HideInInspector]
    [SerializeField]
    private bool ShowLayerWeightsInspector = true;

    [HideInInspector]
    [SerializeField]
    private bool ShowParameterInspector = true;

#pragma warning restore 0414

    //[SerializeField]
    //private List<SynchronizedParameter> m_SynchronizeParameters = new List<SynchronizedParameter>();

    //[SerializeField]
    //private List<SynchronizedLayer> m_SynchronizeLayers = new List<SynchronizedLayer>();

    private Vector3 m_ReceiverPosition;
    private float m_LastDeserializeTime;
    public bool m_WasSynchronizeTypeChanged = true;
    #endregion

    #region Unity
    private void Update()
    {
        if (asUser==null || asUser.m_Animator == null)
            return;

        if (asUser.m_Animator.applyRootMotion && this.photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            asUser.m_Animator.applyRootMotion = false;
        }

        if (PhotonNetwork.InRoom == false || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            this.m_StreamQueue.Reset();
            return;
        }

        if (this.photonView.IsMine == true)
        {
            this.SerializeDataContinuously();

            asUser.CacheDiscreteTriggers();
        }
        else
        {
            this.DeserializeDataContinuously();
        }
    }

    #endregion
    

    #region Serialization
    private void SerializeDataContinuously()
    {
        if (asUser.m_Animator == null)
        {
            return;
        }

        var layer = asUser.SynchronizeLayers;
        for (int i = 0; i < layer.Count; ++i)
        {
            if (layer[i].SynchronizeType == SynchronizeType.Continuous)
            {
                this.m_StreamQueue.SendNext(asUser.m_Animator.GetLayerWeight(layer[i].LayerIndex));
            }
        }

        var para = asUser.SynchronizeParameters;
        for (int i = 0; i < para.Count; ++i)
        {
            SynchronizedParameter parameter = para[i];

            if (parameter.SynchronizeType == SynchronizeType.Continuous)
            {
                switch (parameter.Type)
                {
                    case ParameterType.Bool:
                        this.m_StreamQueue.SendNext(asUser.m_Animator.GetBool(parameter.Name));
                        break;
                    case ParameterType.Float:
                        this.m_StreamQueue.SendNext(asUser.m_Animator.GetFloat(parameter.Name));
                        break;
                    case ParameterType.Int:
                        this.m_StreamQueue.SendNext(asUser.m_Animator.GetInteger(parameter.Name));
                        break;
                    case ParameterType.Trigger:
                        if (!TriggerUsageWarningDone)
                        {
                            TriggerUsageWarningDone = true;
                            Debug.Log("PhotonAnimatorView: When using triggers, make sure this component is last in the stack.\n" +
                                      "If you still experience issues, implement triggers as a regular RPC \n" +
                                      "or in custom IPunObservable component instead", this);

                        }
                        this.m_StreamQueue.SendNext(asUser.m_Animator.GetBool(parameter.Name));
                        break;
                }
            }
        }
    }


    private void DeserializeDataContinuously()
    {
        if (this.m_StreamQueue.HasQueuedObjects() == false)
        {
            return;
        }
        var layer = asUser.SynchronizeLayers;
        for (int i = 0; i < layer.Count; ++i)
        {
            if (layer[i].SynchronizeType == SynchronizeType.Continuous)
            {
                asUser.m_Animator.SetLayerWeight(layer[i].LayerIndex, (float)this.m_StreamQueue.ReceiveNext());
            }
        }

        var para = asUser.SynchronizeParameters;
        for (int i = 0; i < para.Count; ++i)
        {
            SynchronizedParameter parameter = para[i];

            if (parameter.SynchronizeType == SynchronizeType.Continuous)
            {
                switch (parameter.Type)
                {
                    case ParameterType.Bool:
                        asUser.m_Animator.SetBool(parameter.Name, (bool)this.m_StreamQueue.ReceiveNext());
                        break;
                    case ParameterType.Float:
                        asUser.m_Animator.SetFloat(parameter.Name, (float)this.m_StreamQueue.ReceiveNext());
                        break;
                    case ParameterType.Int:
                        asUser.m_Animator.SetInteger(parameter.Name, (int)this.m_StreamQueue.ReceiveNext());
                        break;
                    case ParameterType.Trigger:
                        asUser.m_Animator.SetBool(parameter.Name, (bool)this.m_StreamQueue.ReceiveNext());
                        break;
                }
            }
        }
    }

    private void SerializeDataDiscretly(PhotonStream stream)
    {
        var layer = asUser.SynchronizeLayers;
        for (int i = 0; i < layer.Count; ++i)
        {
            if (layer[i].SynchronizeType == SynchronizeType.Discrete)
            {
                stream.SendNext(asUser.m_Animator.GetLayerWeight(layer[i].LayerIndex));
            }
        }

        var para = asUser.SynchronizeParameters;
        for (int i = 0; i < para.Count; ++i)
        {

            SynchronizedParameter parameter = para[i];

            if (parameter.SynchronizeType == SynchronizeType.Discrete)
            {
                switch (parameter.Type)
                {
                    case ParameterType.Bool:
                        stream.SendNext(asUser.m_Animator.GetBool(parameter.Name));
                        break;
                    case ParameterType.Float:
                        stream.SendNext(asUser.m_Animator.GetFloat(parameter.Name));
                        break;
                    case ParameterType.Int:
                        stream.SendNext(asUser.m_Animator.GetInteger(parameter.Name));
                        break;
                    case ParameterType.Trigger:
                        if (!TriggerUsageWarningDone)
                        {
                            TriggerUsageWarningDone = true;
                            Debug.Log("PhotonAnimatorView: When using triggers, make sure this component is last in the stack.\n" +
                                      "If you still experience issues, implement triggers as a regular RPC \n" +
                                      "or in custom IPunObservable component instead", this);

                        }
                        // here we can't rely on the current real state of the trigger, we might have missed its raise
                        stream.SendNext(asUser.m_raisedDiscreteTriggersCache.Contains(parameter.Name));
                        break;
                }
            }
        }

        // reset the cache, we've synchronized.
        asUser.m_raisedDiscreteTriggersCache.Clear();
    }

    private void DeserializeDataDiscretly(PhotonStream stream)
    {
        var layer = asUser.SynchronizeLayers;
        for (int i = 0; i < layer.Count; ++i)
        {
            if (layer[i].SynchronizeType == SynchronizeType.Discrete)
            {
                asUser.m_Animator.SetLayerWeight(layer[i].LayerIndex, (float)stream.ReceiveNext());
            }
        }

        var para = asUser.SynchronizeParameters;
        for (int i = 0; i < para.Count; ++i)
        {
            SynchronizedParameter parameter = para[i];

            if (parameter.SynchronizeType == SynchronizeType.Discrete)
            {
                switch (parameter.Type)
                {
                    case ParameterType.Bool:
                        if (stream.PeekNext() is bool == false)
                        {
                            return;
                        }
                        asUser.m_Animator.SetBool(parameter.Name, (bool)stream.ReceiveNext());
                        break;
                    case ParameterType.Float:
                        if (stream.PeekNext() is float == false)
                        {
                            return;
                        }

                        asUser.m_Animator.SetFloat(parameter.Name, (float)stream.ReceiveNext());
                        break;
                    case ParameterType.Int:
                        if (stream.PeekNext() is int == false)
                        {
                            return;
                        }

                        asUser.m_Animator.SetInteger(parameter.Name, (int)stream.ReceiveNext());
                        break;
                    case ParameterType.Trigger:
                        if (stream.PeekNext() is bool == false)
                        {
                            return;
                        }

                        if ((bool)stream.ReceiveNext())
                        {
                            asUser.m_Animator.SetTrigger(parameter.Name);
                        }
                        break;
                }
            }
        }
    }

    private void SerializeSynchronizationTypeState(PhotonStream stream)
    {
        var layer = asUser.SynchronizeLayers;
        var para = asUser.SynchronizeParameters;

        byte[] states = new byte[layer.Count + para.Count];

        for (int i = 0; i < layer.Count; ++i)
        {
            states[i] = (byte)layer[i].SynchronizeType;
        }

        for (int i = 0; i < para.Count; ++i)
        {
            states[para.Count + i] = (byte)para[i].SynchronizeType;
        }

        stream.SendNext(states);
    }

    private void DeserializeSynchronizationTypeState(PhotonStream stream)
    {
        byte[] state = (byte[])stream.ReceiveNext();

        var layer = asUser.SynchronizeLayers;
        for (int i = 0; i < layer.Count; ++i)
        {
            layer[i].SynchronizeType = (SynchronizeType)state[i];
        }

        var para = asUser.SynchronizeParameters;
        for (int i = 0; i < para.Count; ++i)
        {
            para[i].SynchronizeType = (SynchronizeType)state[para.Count + i];
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (asUser.m_Animator == null)
        {
            return;
        }

        if (stream.IsWriting == true)
        {
            if (this.m_WasSynchronizeTypeChanged == true)
            {
                this.m_StreamQueue.Reset();
                this.SerializeSynchronizationTypeState(stream);

                this.m_WasSynchronizeTypeChanged = false;
            }

            this.m_StreamQueue.Serialize(stream);
            this.SerializeDataDiscretly(stream);
        }
        else
        {
#if PHOTON_DEVELOP
                if( ReceivingSender != null )
                {
                    ReceivingSender.OnPhotonSerializeView( stream, info );
                }
                else
#endif
            {
                if (stream.PeekNext() is byte[])
                {
                    this.DeserializeSynchronizationTypeState(stream);
                }

                this.m_StreamQueue.Deserialize(stream);
                this.DeserializeDataDiscretly(stream);
            }
        }
    }

    #endregion

}
