using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    protected ControlMeta commander;

    protected int difficulty = 50;

    protected Player _player;
    
    protected virtual void SetControl(ControlMeta meta)
    {
        commander = meta;
        commander.AddControls(this);
        _player = commander.GetPlayer();
    }

    public virtual void Initialized(int diff)
    {
        difficulty = diff;
    }

    public virtual void ControlDifficulty(int diff)
    {
        difficulty = diff;
    }
}
