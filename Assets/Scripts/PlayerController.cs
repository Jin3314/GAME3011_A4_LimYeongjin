using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public UnityEvent Interact;
    private Vector2 mousePosition;

    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    [SerializeField] private GameObject cursorObject;

    public void OnUse()
    {
        Interact.Invoke();
    }

    public void OnClick(InputValue button)
    {
        PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(m_PointerEventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.CompareTag("Tile") && !results[i].gameObject.GetComponent<Tile>().isSet)
            {
                Tile tile = results[i].gameObject.GetComponent<Tile>();

                tile.Set(cursorObject.GetComponent<Tile>().GetSprite(), cursorObject.GetComponent<Tile>().type, cursorObject.GetComponent<Tile>().connectionDirections, cursorObject.transform.rotation);

                tile.CheckConnection();

                Game.Instance.SelectRandomTile();

                Game.Instance.numMoves--;
            }
        }
    }

    public void OnCursorMove(InputValue value)
    {
        mousePosition = value.Get<Vector2>();

        PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = value.Get<Vector2>();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(m_PointerEventData, results);

        if (!cursorObject) return;

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.CompareTag("Tile") && !results[i].gameObject.GetComponent<Tile>().isSet)
            {
                cursorObject.transform.localScale = new Vector3(1, 1, 1);

                RectTransform cursorRect = cursorObject.GetComponent<RectTransform>();
                RectTransform resultsTransform = results[i].gameObject.GetComponent<RectTransform>();

                Vector3 cursorPosition = new Vector3(resultsTransform.localPosition.x,
                                                     resultsTransform.localPosition.y);

                cursorRect.localPosition = cursorPosition;

                break;
            }
            else
            {
                cursorObject.transform.localScale = Vector3.zero;
            }
        }
    }
}
