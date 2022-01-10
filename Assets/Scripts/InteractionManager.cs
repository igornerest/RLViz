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

    private void Start()
    {
        BuildInitialGridWorld();
    }

    private void Update()
    {
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
            GridBlock gridBlock = Instantiate(gridBlockPrefab, state.Position, Quaternion.identity).GetComponent<GridBlock>();
            gridBlock.displayModeManager = displayModeManager;
            gridBlock.UpdateBlock(state, MDPManager.Instance.Mdp);
        }
    }

    private void HandleInteractionMode()
    {
        switch (interactionModeManager.GetInteractionMode())
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

    private void HandleSimulateInteractionMode()
    {
        Debug.Log("Handling Simulation");
    }

    private void HandleDeleteInteractionMode()
    {
        Debug.Log("Handling Deletion");
    }

    private void HandleCreateInteractionMode()
    {
        Debug.Log("Handling Creation");
    }

    private void HandleEditInteractionMode()
    {
        Debug.Log("Handling Deletion");
    }
}
