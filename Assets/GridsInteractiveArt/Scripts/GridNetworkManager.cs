using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GridNetworkManager : NetworkManager {

    NetworkConnectionHandler m_NetworkConnectionHandler;
    
    void Awake()
    {
        m_NetworkConnectionHandler = GetComponent<NetworkConnectionHandler>();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        if (conn.lastError != NetworkError.Ok)
        {
            m_NetworkConnectionHandler.ResetMatchmaker();
        }
        base.OnClientDisconnect(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        base.OnClientError(conn, errorCode);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        base.OnServerDisconnect(conn);
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        base.OnServerError(conn, errorCode);
    }
}
