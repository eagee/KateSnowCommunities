using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
///  This is the master script for the GridPlayerObject that will control each individual painted element
///  on the grid board, including position, visibility, and color.
/// </summary>
public class GridPlayerScript : NetworkBehaviour
{
    /// <summary>
    ///  Single instance of the server object (for some reason not using this idiom causes issues with more than one item trying to be the server - I don't know what I'm doing wrong, but this works)
    /// </summary>
    public static GridPlayerScript myMasterInstance;

    /// <summary>
    /// Indicates whether the player object is the master instance that stores the ServerElements property used by other players.
    /// </summary>
    [SyncVar]
    public bool isMasterInstance = false;

    /// <summary>
    /// SyncList object of ElementProperty values
    /// </summary>
    public class SyncListProperties : SyncListStruct<ElementProperty> { }

    /// <summary>
    /// These are the elements that remain synced between the server and the client.
    /// </summary>
    public SyncListProperties ServerElements = new SyncListProperties();

    // Contains a dictionary of colors that can be referenced by ElementProperties
    private Dictionary<int, Color> myClientColors = new Dictionary<int, Color>();
    private byte myPaletteIndex;

    /// <summary>
    /// Paint elements based on game objects in the scene that we can set properties on based on the server sync elements.
    /// </summary>
    private Dictionary<byte, GameObject> mySceneElements = new Dictionary<byte, GameObject>();

    /// <summary>
    /// Indicates whether the client frame is dirty or not, and needs to be updated based on changes in the server elements
    /// </summary>
    private bool myFrameIsDirty = false;

    /// <summary>
    /// Iterate through each of the palette elements in the scene, and populate a list of their colors in a map.
    /// Also sets the the current palette index to 1.
    /// </summary>
    private void SetupPlayerPalette()
    {
        myPaletteIndex = 1;
        PlayerPaintPaletteHandler[] palettes = FindObjectsOfType<PlayerPaintPaletteHandler>();
        foreach (var palette in palettes)
        {
            myClientColors[palette.PaletteIndex] = palette.myPaletteColor;
        }
    }

    public Color GetActivePaletteColor()
    {
        if (myClientColors.ContainsKey(myPaletteIndex))
            return myClientColors[myPaletteIndex];

        return myClientColors[0];
    }


    /// <summary>
    /// If we're setting up the local player, this method will enable mouse and touch input.
    /// Otherwise it will disable the touch and mouse input component.
    /// </summary>
    private void SetupTouchAndMouseInput()
    {
        if (isLocalPlayer)
        {
            this.tag = "Player";
            GetComponent<TouchAndMouseInput>().enabled = true;
            Camera camera = GameObject.FindObjectOfType<Camera>();
            GetComponent<TouchAndMouseInput>().inputCamera = camera;
            Vector3 pos = this.transform.position;
            pos.z = -2;
            this.transform.position = pos;
        }
        else
        {
            GetComponent<TouchAndMouseInput>().enabled = false;
        }
    }

    /// <summary>
    /// GameObject start method sets reference to the single instance on the server.
    /// </summary>
    void Start()
    {
        // Sets up the palette and colors that will be used to paint the scene.
        SetupPlayerPalette();

        // Sets up touch and mouse input if this is the local player
        SetupTouchAndMouseInput();
    }
    
    /// <summary>
    /// When the server starts populate ServerElements with all the default properties of the GameObjects in the scene with an "ElementScript" attribute.
    /// (This way our server elements are always updated dynamically based on what's actually in the scene)
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Get the master player script object on the server, if one doesn't
        // exist already, then assign this player object as the master instance
        // so that other objects will use it as such.
        GridPlayerScript[] players = FindObjectsOfType<GridPlayerScript>();
        if (players.Length <= 1)
        {
            isMasterInstance = true;
            myMasterInstance = this;
            myMasterInstance.ServerElements.Clear();

            ElementScript[] elements = FindObjectsOfType<ElementScript>();

            foreach (var element in elements)
            {
                ElementProperty newProperty = new ElementProperty(byte.Parse(element.gameObject.name), (byte)Random.Range(0, 12));
                myMasterInstance.ServerElements.Add(newProperty);
            }
        }
        else
        {
            foreach (GridPlayerScript player in players)
            {
                if (player.isMasterInstance == true)
                {
                    myMasterInstance = player;
                }
            }
        }

        // Use this to print debug messages when server elements change
        //ServerElements.Callback = OnServerElementsChanged;
    }

    /// <summary>
    /// When the client starts we populate mySceneElemetns with all the GameObjects in the scene with an "ElementScript" attribute.
    /// (This way we can always assign properties in ServerElements visually to the scene when there's an update)
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();

        // Get the master player script object on the server, if one doesn't
        // exist already, then assign this player object as the master instance
        // so that other objects will use it as such.
        GridPlayerScript[] players = FindObjectsOfType<GridPlayerScript>();
        if (players.Length <= 1)
        {
            myMasterInstance = this;
        }
        else
        {
            foreach (GridPlayerScript player in players)
            {
                if (player.isMasterInstance == true)
                {
                    myMasterInstance = player;
                }
            }
        }

        // Set up references to the paint elements that we'll be modifying from the scene
        ElementScript[] paintElements = FindObjectsOfType<ElementScript>();
        foreach (var element in paintElements)
        {
            mySceneElements.Add(byte.Parse(element.gameObject.name), element.gameObject);
        }

        myFrameIsDirty = true;
    }

    /// <summary>
    /// The update method for the player object iterates through all child objects, locates the object associated with the element ID
    /// (e.g. object name) and modifies the properties of that object by what's on the server.
    /// </summary>
    void Update()
    {
        if (myFrameIsDirty)
        {
            myFrameIsDirty = false;
            foreach (ElementProperty element in myMasterInstance.ServerElements)
            {
                if (mySceneElements.ContainsKey(element.ID))
                {
                    mySceneElements[element.ID].GetComponent<ElementScript>().SetShapeFrame(element.shapeFrame);
                    mySceneElements[element.ID].GetComponent<Renderer>().material.color = myClientColors[element.shapePaletteIndex];
                }
            }
        }
    }

    /// <summary>
    ///  Method called by child objects to handle input behavior, will triggering property changes
    ///  based on current player state
    /// </summary>
    /// <param name="elementID"></param>
    public void OnHandleOnChildTouchUp(ElementScript element)
    {

        CmdServerSetVisibility(byte.Parse(element.name), myPaletteIndex);

        // Code to prevnt overwriting shapes of the same color
        //byte elementID = byte.Parse(element.name);
        //for (int index = 0; index < myMasterInstance.ServerElements.Count; index++)
        //{
        //    if (myMasterInstance.ServerElements[index].ID == elementID
        //     && myMasterInstance.ServerElements[index].shapePaletteIndex != myPaletteIndex)
        //    {
        //        
        //    }
        //}
    }

    public void OnPaletteColorChanged(byte paletteIndex)
    {
        if(myClientColors.ContainsKey(paletteIndex))
        {
            myPaletteIndex = paletteIndex;
        }        
    }


    public void OnConnectedToServer()
    {

    }

    /// <summary>
    ///  Server method called by OnHandleChildTouchUp that will call the set visibility command on the single sever object.
    /// </summary>
    /// <param name="ID">The name of the component to change.</param>
    [Command]
    public void CmdServerSetVisibility(byte ID, byte paletteIndex)
    {
        myMasterInstance.CmdSetVisibility(ID, paletteIndex);
    }

    /// <summary>
    ///  This Command method sets the network sync properties for all clients based on the parameters specified.
    ///  Elements have to be removed and inserted in order to get the struct to sync, it seems to be a bug in unity
    ///  that hasn't been addressed yet.
    /// </summary>
    /// <param name="ID">The ID of the shape being changed.</param>
    //[Command]
    public void CmdSetVisibility(byte ID, byte paletteIndex)
    {
        int targetIndex = -1;
        ElementProperty newElement = new ElementProperty(0, 0);
        for (int index = 0; index < ServerElements.Count; index++)
        {
            ElementProperty element = ServerElements[index];
            if (element.ID.Equals(ID))
            {
                targetIndex = index;
                newElement.ID = element.ID;
                newElement.shapePaletteIndex = paletteIndex;
                newElement.shapeFrame = (byte)Random.Range(0f, 12.9f);
                ServerElements[targetIndex] = newElement;
                RpcClientFrameIsDirty();
                break;
            }
        }
    }

    /// <summary>
    /// Sets the client frame to dirty when master instance frame is dirty (since that isn't synced to all clients)
    /// </summary>
    /// <param name="ID"></param>
    [ClientRpc]
    void RpcClientFrameIsDirty()
    {           
        myFrameIsDirty = true;
    }

    /// <summary>
    /// Debug routine triggered every time a server element is changed.
    /// </summary>
    //void OnServerElementsChanged(SyncListStruct<ElementProperty>.Operation op, int itemIndex)
    //{
    //    myFrameIsDirty = true;
    //}



}
