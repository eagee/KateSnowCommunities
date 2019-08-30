using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GlobalSettings
{
    private static readonly GlobalSettings instance = new GlobalSettings();

    private bool m_showPalette = true;
    private bool m_allowToBeServer = false;
    
    private GlobalSettings() { }

    public static GlobalSettings Instance
    {
        get
        {
            return instance;
        }
    }

    public bool ShowPalette
    {
        get { return m_showPalette; }
    }
    public bool AllowToBeServer
    {
        get { return m_allowToBeServer; }
    }


}