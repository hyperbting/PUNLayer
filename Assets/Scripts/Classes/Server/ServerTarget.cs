using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerTarget
{
    public ServerMasterTargetType smTargetType;
    #region BestRegion ONLY Filed
    #endregion

    #region SpecificRegion ONLY Filed
    [ConditionalHide("smTargetType", ServerMasterTargetType.SpecificRegion)]
    public string photonRegion;
    #endregion

    #region SpecificMaster ONLY Filed
    [ConditionalHide("smTargetType", ServerMasterTargetType.SpecificMaster)]
    public string ipAddress;
    [ConditionalHide("smTargetType", ServerMasterTargetType.SpecificMaster)]
    public int serverPort;
    #endregion

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    #region
    public enum ServerMasterTargetType
    {
        BestRegion,
        SpecificRegion,
        SpecificMaster,
    }
    #endregion
}
