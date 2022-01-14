using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private InteractionModeManager interactionModeManager;
    [SerializeField] private DisplayModeManager displayModeManager;
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Transform gridBlockPrefab;
    [SerializeField] private Transform ghostBlockPrefab;

    private InteractionMode currInteractionMode;

    private Transform hoveredGridBlock;
    private Transform selectedGridBlock;

    private Transform ghostBlock;

    private void Start()
    {
        BuildInitialGridWorld();
        SetGhostBlock();
    }

    private void Update()
    {
        if (IsMousePointerOverUIPanel())
        {
            ClearHoveredGridBlocks();
        }

        HandleInteractionMode();
    }

    private bool IsMousePointerOverUIPanel()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        canvas.GetComponent<GraphicRaycaster>().Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    private void BuildInitialGridWorld()
    {
        foreach (State state in MDPManager.Instance.Mdp.GetAllStates())
        {
            BuildBlock(state);
        }
    }

    private void BuildBlock(State state)
    {
        GridBlock gridBlock = Instantiate(gridBlockPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
        gridBlock.displayModeManager = displayModeManager;
        gridBlock.UpdateBlock(state, MDPManager.Instance.Mdp);
    }

    private void SetGhostBlock()
    {
        var position = new Vector3Int(0, 0, 0);
        ghostBlock = Instantiate(ghostBlockPrefab, position, Quaternion.identity);
    }

    private void HandleInteractionMode()
    {
        var updatedInteractionMode = interactionModeManager.GetInteractionMode();
        if (currInteractionMode != updatedInteractionMode)
        {
            ClearSelectedGridBlocks();
            currInteractionMode = updatedInteractionMode;
        }

        switch (currInteractionMode)
        {
            case InteractionMode.Simulate:
                HandleSimulateInteractionMode();
                break;

            case InteractionMode.Delete:
                HandleDeleteInteractionMode();
                break;

            case InteractionMode.Create:
                HandleCreateInteractionMode();
                break;

            case InteractionMode.Edit:
                HandleEditInteractionMode();
                break;
        }
    }

    private bool UpdateHoveredGridBlock(Transform transform)
    {
        if (hoveredGridBlock != transform)
        {
            if (hoveredGridBlock != null)
            {
                hoveredGridBlock.GetComponent<GridBlock>().IsHoveredByUser = false;
            }

            hoveredGridBlock = transform;
            hoveredGridBlock.GetComponent<GridBlock>().IsHoveredByUser = true;
            return true;
        }

        return false;
    }

    private void ClearHoveredGridBlocks()
    {
        if (hoveredGridBlock != null)
        {
            hoveredGridBlock.GetComponent<GridBlock>().IsHoveredByUser = false;
        }

        hoveredGridBlock = null;
    }

    private bool UpdateSelectedGridBlock(Transform transform)
    {
        if (selectedGridBlock != transform)
        {
            if (selectedGridBlock != null)
            {
                selectedGridBlock.GetComponent<GridBlock>().IsSelectedByUser = false;
            }

            selectedGridBlock = transform;
            selectedGridBlock.GetComponent<GridBlock>().IsSelectedByUser = true;
            return true;
        }

        return false;
    }

    private void ClearSelectedGridBlocks()
    {
        if (selectedGridBlock != null)
        {
            selectedGridBlock.GetComponent<GridBlock>().IsSelectedByUser = false;
        }

        selectedGridBlock = null;
    }

    private void UpdateGhostBlock(Vector3 position)
    {
        int xPos = (int)position.x;
        int yPos = (int)position.z;
        ghostBlock.transform.position = new Vector3Int(xPos, 0, yPos);
        ghostBlock.gameObject.SetActive(true);
    }

    private void ClearGhostBlock()
    {
        ghostBlock.gameObject.SetActive(false);
    }

    private void HandleSimulateInteractionMode()
    {
        // TODO
    }

    private void HandleDeleteInteractionMode()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "GridBlock" && !IsMousePointerOverUIPanel())
        {
            UpdateHoveredGridBlock(hit.transform);

            if (Input.GetMouseButtonDown(0))
            {
                Destroy(hit.transform.gameObject);
            }
        }
        else
        {
            ClearHoveredGridBlocks();
        }
    }

    private void HandleCreateInteractionMode()
    {
        ClearSelectedGridBlocks();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "BaseFloor" && !IsMousePointerOverUIPanel())
        {
            UpdateGhostBlock(hit.point);

            if (Input.GetMouseButtonDown(0))
            {
                int xPos = (int)hit.point.x;
                int yPos = (int)hit.point.z;
                Debug.Log(string.Format("Creating state at position {0}, {1}", xPos, yPos));

                State newState = new State(xPos, yPos);
                interactionModeManager.UpdateState(newState, MDPManager.Instance.Mdp);

                BuildBlock(newState);
            }
        }
        else
        {
            ClearGhostBlock();
        }
    }

    private void HandleEditInteractionMode()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "GridBlock" && !IsMousePointerOverUIPanel())
        {
            UpdateHoveredGridBlock(hit.transform);

            if (Input.GetMouseButtonDown(0))
            {
                State state = hit.transform.GetComponent<GridBlock>().State;

                if (UpdateSelectedGridBlock(hit.transform))
                {
                    interactionModeManager.UpdatePanelWithStateInfo(state, MDPManager.Instance.Mdp);
                }
            }
        }
        else
        {
            ClearHoveredGridBlocks();
        }

        if (selectedGridBlock != null)
        {
            State state = selectedGridBlock.GetComponent<GridBlock>().State;
            interactionModeManager.UpdateState(state, MDPManager.Instance.Mdp);
        }
    }
}
