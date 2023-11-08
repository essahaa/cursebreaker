using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTile : MonoBehaviour
{
    public static MovableTile Instance;

    public int Level { get; set; }

    public int GridSizeX { get; set; }
    public int GridSizeY { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public string TileType { get; set; }
    public bool IsLocked { get; set; } // Use a property for the locked flag.
}
