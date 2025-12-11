using UnityEngine;

public class BuildSelectButton : MonoBehaviour
{
    [SerializeField] private BuildType buildType = BuildType.Castle;

    public void OnClickSelect()
    {
        SelectedBuildTracker.Instance.SelectBuild(buildType);
    }
}
