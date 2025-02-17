using UnityEngine;
using UnityEngine.UI;

public class TurretUIButton : MonoBehaviour {
    // Assign the turret data via the Inspector
    public TurretData turretData;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked() {
        TurretPlacementManager.Instance.SetSelectedTurret(turretData);
    }
}