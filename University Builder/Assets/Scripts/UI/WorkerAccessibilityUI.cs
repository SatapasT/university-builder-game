using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerAccessibilityUI : MonoBehaviour
{
    [SerializeField] private Slider workerSlider;
    [SerializeField] private TextMeshProUGUI workerLabel;

    private void OnEnable()
    {
        if (workerSlider == null || workerLabel == null)
        {
            Debug.LogError("WorkerAccessibilityUI: workerSlider/workerLabel not assigned.");
            return;
        }

        int current = 1;
        if (WorkerManager.Instance != null)
            current = WorkerManager.Instance.GetMaxWorkers();

        // Make slider behave like an int slider
        workerSlider.wholeNumbers = true;

        workerSlider.onValueChanged.RemoveListener(OnWorkerChanged);
        workerSlider.SetValueWithoutNotify(current);
        workerSlider.onValueChanged.AddListener(OnWorkerChanged);

        RefreshLabel(current);
    }

    private void OnDisable()
    {
        if (workerSlider != null)
            workerSlider.onValueChanged.RemoveListener(OnWorkerChanged);
    }

    private void OnWorkerChanged(float value)
    {
        if (WorkerManager.Instance == null)
        {
            Debug.LogWarning("WorkerAccessibilityUI: WorkerManager.Instance is null.");
            RefreshLabel(Mathf.RoundToInt(value));
            return;
        }

        int newCount = Mathf.RoundToInt(value);
        WorkerManager.Instance.SetWorkerCount(newCount);

        // Read back actual (clamped) value
        int actual = WorkerManager.Instance.GetMaxWorkers();
        workerSlider.SetValueWithoutNotify(actual);

        RefreshLabel(actual);

        // If worker assignment panel is open, refresh it
        PlayerMenu.Instance?.RefreshWorkerAssignmentText();
    }

    private void RefreshLabel(int count)
    {
        if (workerLabel != null)
            workerLabel.text = $"Worker number: {count}";
    }
}
