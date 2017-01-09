using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ServerProperties : NetworkBehaviour {

    public class SyncListProperties : SyncListStruct<ElementProperty> { }
    public SyncListProperties Elements = new SyncListProperties();

    [Server]
    public override void OnStartServer()
    {
        Elements.Clear();
        base.OnStartServer();
    }

    [Command]
    public void CmdAddServerElement(string guid)
    {
        ElementProperty newProperty = new ElementProperty(guid, 0);
        Elements.Add(newProperty);
    }

    [Command]
    public void CmdSwapVisibility(string guid)
    {
        for(int index = 0; index < Elements.Count; index++)
        {
            ElementProperty element = Elements[index];
            if (element.ID.Equals(guid))
            { 
                Elements[index] = element;
            }
        }
    }
    public ElementProperty GetServerElement(string guid)
    {
        foreach (ElementProperty element in Elements)
        {
            if (element.ID.Equals(guid))
            {
                return element;
            }
        }

        return new ElementProperty();
    }

}
