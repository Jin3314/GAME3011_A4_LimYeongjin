using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileType
{
    None,
    Straight,
    Angle,
    T,
    Quad,
    Node,
}

public class Tile : MonoBehaviour
{
    public TileType type { get; private set; }
    private float tileAngle;

    public bool isSet;
    public bool isActivated;

    public TileData data;

    public List<Vector3> connectionDirections;

    public bool isPowerNode;

    public void Awake()
    {
        type = TileType.None;
        tileAngle = 0;
    }

    public void Set(Sprite sprite, TileType tileType, List<Vector3> connections, Quaternion rotation)
    {
        GetComponent<Image>().sprite = sprite;
        type = tileType;
        connectionDirections = new List<Vector3>(connections);
        transform.rotation = rotation;

        isSet = true;
    }

    public void Copy(Tile tile)
    {
        GetComponent<Image>().sprite = tile.GetSprite();
        type = tile.type;
        connectionDirections = new List<Vector3>(tile.connectionDirections);
        transform.rotation = tile.transform.rotation;

        isSet = true;
    }

    public Sprite GetSprite()
    {
        return GetComponent<Image>().sprite;
    }

    public void ActivateTile()
    {
        GetComponent<Image>().color = Color.green;
        isActivated = true;
    }

    public void CheckConnection()
    {
        bool connectionTest = false;

        foreach (Vector3 direction in connectionDirections)
        {
            RectTransform rect = GetComponent<RectTransform>();
            Vector3 parentScale = transform.parent.localScale;
            Vector3 worldScale = transform.parent.parent.localScale;

            Vector2 origin = new Vector2(rect.position.x, rect.position.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rect.sizeDelta.x * worldScale.x * parentScale.x);

            foreach (RaycastHit2D hit in hits)
            {
                GameObject hitObject = hit.collider.gameObject;

                if (!ReferenceEquals(hitObject, gameObject) && hitObject.CompareTag("Tile") && hitObject.GetComponent<Tile>().isSet)
                {
                    Tile tile = hitObject.GetComponent<Tile>();

                    // List.Contains does not work with Vector3
                    foreach (Vector3 connection in tile.connectionDirections)
                    {
                        connectionTest = Vector3.Distance(connection, -direction) <= 0.001f;
                        if (connectionTest) break;
                    }

                    // Adjacent tile is activated
                    if (tile.isActivated && !isActivated 
                        && (!isPowerNode && !tile.isPowerNode) // Adjacent power nodes cannot turn each other on
                        && connectionTest)
                    {
                        ActivateTile();
                        CheckConnection();
                        Game.Instance.CheckWinCondition();
                    }
                    // Adjacent tile is not activated but this one is
                    else if (!tile.isActivated && isActivated
                        && (!isPowerNode && !tile.isPowerNode)
                        && connectionTest)
                    {
                        tile.ActivateTile();
                        tile.CheckConnection();
                        Game.Instance.CheckWinCondition();
                    }
                }
            }
        }
    }

    private void Update()
    {
        foreach (Vector3 direction in connectionDirections)
        {
            RectTransform rect = GetComponent<RectTransform>();
            Vector3 parentScale = transform.parent.localScale;
            Vector3 worldScale = transform.parent.parent.localScale;

            Vector2 origin = new Vector2(rect.position.x, rect.position.y);

            Debug.DrawRay(origin, direction * rect.sizeDelta.x * worldScale.x * parentScale.x, Color.green);
        }
    }
}
