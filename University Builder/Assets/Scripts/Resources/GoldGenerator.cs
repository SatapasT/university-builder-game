using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGenerator : MonoBehaviour
{
    [SerializeField] private float intervalSeconds = 10f;

    private Coroutine incomeRoutine;

    private void Start()
    {
        incomeRoutine = StartCoroutine(PassiveIncomeLoop());
    }

    private IEnumerator PassiveIncomeLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return new WaitForSeconds(intervalSeconds);
            GeneratePassiveIncome();
        }
    }

    private void GeneratePassiveIncome()
    {
        if (ResourcesManager.Instance == null ||
            BuildProgressTracker.Instance == null)
            return;

        Dictionary<ResourceType, int> totalIncome = new();

        foreach (BuildType buildType in System.Enum.GetValues(typeof(BuildType)))
        {
            if (buildType == BuildType.None)
                continue;

            if (BuildProgressTracker.Instance.GetState(buildType)
                != BuildProgressTracker.BuildState.Built)
                continue;

            BuildInfo info = BuildDatabase.Get(buildType);
            if (info == null || info.PassiveIncome.Length == 0)
                continue;

            foreach (ResourceAmount income in info.PassiveIncome)
            {
                if (!totalIncome.ContainsKey(income.type))
                    totalIncome[income.type] = 0;

                totalIncome[income.type] += income.amount;
            }
        }

        foreach (var pair in totalIncome)
        {
            ResourcesManager.Instance.AddResource(pair.Key, pair.Value);
        }
    }

    private void OnDestroy()
    {
        if (incomeRoutine != null)
            StopCoroutine(incomeRoutine);
    }
}
