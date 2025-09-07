using _Project.Scripts.Storage;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUIScript : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private StyleSheet styleSheet;

    public string panelName = "Panel";
    
    private VisualElement root;

    private void OnEnable()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UI Document is null");
            return;
        }
        InitializeUI();

        root.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        Debug.Log(
            $"OnPointerDown: PanelName: {panelName}"
                .NewLine().Add($"position: {evt.position.x}, {evt.position.y}")
                .NewLine().Add($"target: {evt.target}")
                .NewLine().Add($"current target: {evt.currentTarget}")
            );
        
        DragDropManager.SetGhostIconPosition(evt.position);
        DragDropManager.GhostIcon.style.visibility = Visibility.Visible;
        DragDropManager.GhostIcon.CapturePointer(evt.pointerId);
        
        evt.StopPropagation();
    }

    void InitializeUI()
    {
        root = uiDocument.rootVisualElement.AddClass("root");
        root.styleSheets.Add(styleSheet);
        
        var board = root.CreateChild("board");

        // board.userData = this;

        board.CreateChild("container1");
        board.CreateChild("container2");
    }
}
