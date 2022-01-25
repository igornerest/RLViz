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

    [SerializeField] private GameObject baseFloor;

    private InteractionMode currInteractionMode;

    private Transform hoveredGridBlock;
    private Transform selectedGridBlock;

    private Transform ghostBlock;

    private float lastMouseClickTime = 0f;

    private void Start()
    {
        BuildInitialGridWorld();
        SetGhostBlock();
        baseFloor.SetActive(false);
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
        ghostBlock = Instantiate(gridBlockPrefab, position, Quaternion.identity);
        ghostBlock.tag = "GhostBlock";
        ghostBlock.GetComponent<GridBlock>().IsGhostBlock = true;
    }

    private void HandleInteractionMode()
    {
        var updatedInteractionMode = interactionModeManager.GetInteractionMode();
        if (currInteractionMode != updatedInteractionMode)
        {
            ClearSelectedGridBlocks();
            baseFloor.SetActive(false);
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

    private void HandleMouseClick(System.Action actionHandler)
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMouseClickTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            float totalMouseClickTime = Time.time - lastMouseClickTime;
            float timeThreshold = 0.1f;

            if (totalMouseClickTime < timeThreshold)
            {
                actionHandler();
            }
        }
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

            HandleMouseClick(() =>
                Destroy(hit.transform.gameObject)
            );
        }
        else
        {
            ClearHoveredGridBlocks();
        }
    }

    private void HandleCreateInteractionMode()
    {
        baseFloor.SetActive(true);
        ClearSelectedGridBlocks();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasValidHit = Physics.Raycast(ray, out hit) && !IsMousePointerOverUIPanel();

        if (hasValidHit && hit.transform?.tag == "BaseFloor")
        {
            UpdateGhostBlock(hit.point);
        }
        
        if (hasValidHit && hit.transform?.tag == "BaseFloor" || hit.transform?.tag == "GhostBlock")
        {
            HandleMouseClick(() =>
            {
                ClearGhostBlock();

                int xPos = (int)hit.point.x;
                int yPos = (int)hit.point.z;
                Debug.Log(string.Format("Creating state at position {0}, {1}", xPos, yPos));

                State newState = new State(xPos, yPos);
                interactionModeManager.UpdateState(newState, MDPManager.Instance.Mdp);
                BuildBlock(newState);
            });
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

            HandleMouseClick(() =>
            {
                State state = hit.transform.GetComponent<GridBlock>().State;

                if (UpdateSelectedGridBlock(hit.transform))
                {
                    interactionModeManager.UpdatePanelWithStateInfo(state, MDPManager.Instance.Mdp);
                }
            });
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
