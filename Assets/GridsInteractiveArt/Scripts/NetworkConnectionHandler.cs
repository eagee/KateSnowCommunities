using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// When the isServer property is true: this class ensures that a match maker room exists and that an internet match has been created.
/// When the isServer property is false: this class obtains the current match maker list from the server, and connects; handling any disconnect
/// errors gracefully.
/// </summary>
[RequireComponent(typeof(NetworkManager))]
public class NetworkConnectionHandler : MonoBehaviour
{
    public bool edtUseMatchMaker = true;
	public bool supportClientOnly = false;

    private bool m_isServer = false;
    private bool m_useMatchMaker = true;
    private float m_ConnectionTimer = 5.0f;
    private const float STD_CONNECTION_WAIT_TIME = 1.0f;
    private const float MAX_CONNECTION_WAIT_TIME = 2.0f;
    private float m_ConnectionWaitTime = STD_CONNECTION_WAIT_TIME;
    
    private NetworkManager m_networkManager;

    /// <summary>
    /// Provides a single place where changes to the m_handleServerBehavior value is changed.
    /// </summary>
    public bool IsServer
    {
        get { return m_isServer; }
        set { m_isServer = value; }
    }

    /// <summary>
    /// Proivdes a single place where the match maker value is set
    /// </summary>
    public bool UseMatchMaker
    {
        get
        {
            return m_useMatchMaker;
        }
        set
        {
            //TODO: Add code here that will disconnect the active match maker client
            // if a user decides to switch into private mode.
            m_useMatchMaker = value;
        }
    }

    /// <summary>
    /// Used when object is initialized
    /// </summary>
    void Start()
    {
        // The PC version of this software will always be in charge of creating the match (to avoid confusion with phone users)
        // Phone users will always connect expecting the PC to be there.
		if((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && !supportClientOnly)
        {
            IsServer = true;
        }
        else
        {
            IsServer = false;
        }
        UseMatchMaker = edtUseMatchMaker;
    }

    /// <summary>
    /// Awake is called after all other objects in the scene have been initialized, so we can grab other components safely.
    /// </summary>
    void Awake()
    {
        m_networkManager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        m_ConnectionTimer += Time.deltaTime;
        if (m_ConnectionTimer > m_ConnectionWaitTime)
        {
            m_ConnectionTimer = 0.0f;
            m_ConnectionWaitTime = STD_CONNECTION_WAIT_TIME;
            // If this is a server, ensure that there is an active match being hosted via match maker;
            // otherwise attempt to connect to the server via match maker if the client is disconnected.
            if (!NetworkClient.active && !NetworkServer.active)
            {
                if (UseMatchMaker == true)
                {
                    HandleMatchmakerSetup();
                }
                else
                {
                    m_networkManager.StartHost();
                }
            }
        }// end check for connection timer.
    }

    /// <summary>
    /// Ensures that for a client or server matchmaker creates or connects to the necessary matches.
    /// </summary>
    private void HandleMatchmakerSetup()
    {
        if (m_networkManager.matchMaker == null)
        {
            Debug.Log("Starting Matchmaker!");
            m_ConnectionWaitTime = MAX_CONNECTION_WAIT_TIME;
            m_networkManager.StartMatchMaker();
        }
        else if (m_networkManager.matchInfo == null)
        {
            if (m_networkManager.matches == null)
            {
                if (IsServer)
                {
                    Debug.Log("Creating a Server Match!");
                    m_ConnectionWaitTime = MAX_CONNECTION_WAIT_TIME;
                    m_networkManager.matchMaker.CreateMatch(m_networkManager.matchName, m_networkManager.matchSize, true, "", "", "", 0, 1, m_networkManager.OnMatchCreate);
                }
                else
                {
                    Debug.Log("Getting List of Matches!");
                    m_ConnectionWaitTime = MAX_CONNECTION_WAIT_TIME;
                    m_networkManager.matchMaker.ListMatches(0, 20, m_networkManager.matchName, false, 0, 1, m_networkManager.OnMatchList);
                }
            }
            else if(!IsServer)
            {
                // If this is a client connection and we've got a populated list of matches, then go ahead and join the first one
                // (it's the only one we should have available for the show)
                if(m_networkManager.matches.Count > 0)
                {
                    Debug.Log("Joining KateSnowInteractive Match!");
                    m_ConnectionWaitTime = MAX_CONNECTION_WAIT_TIME;
                    m_networkManager.matchName = m_networkManager.matches[0].name;
                    m_networkManager.matchSize = (uint)m_networkManager.matches[0].currentSize;
                    m_networkManager.matchMaker.JoinMatch(m_networkManager.matches[0].networkId, "", "", "", 0, 1, m_networkManager.OnMatchJoined);
                }
                else
                {
                    Debug.Log("No matches on Server. Killing MatchMaker.");
                    m_networkManager.StopMatchMaker();
                }
            }
        }// end else if (m_networkManager.matchInfo == null)
    }
}

