using UnityEngine;
using TMPro;

public class EmployeeLevelDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text employeeLevelText;
    [SerializeField] private TMP_Text employeeCountText;

    [Header("Display Settings")]
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";
    [SerializeField] private bool showEmployeeCount = true;
    [SerializeField] private string countPrefix = "Employees: ";
    [SerializeField] private string countSuffix = "";

    [Header("Update Settings")]
    [SerializeField] private bool updateEveryFrame = true;

    private int lastLevel = -1;
    private int lastCount = -1;

    private void Start()
    {
        UpdateDisplay();
    }

    private void Update()
    {
        if (updateEveryFrame)
        {
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        if (employeeLevelText == null) return;

        int avgLevel = GetAverageEmployeeLevel();
        int count = GetEmployeeCount();

        if (employeeLevelText != null)
        {
            employeeLevelText.text = prefix + avgLevel.ToString() + suffix;
        }

        if (employeeCountText != null && showEmployeeCount)
        {
            employeeCountText.text = countPrefix + count.ToString() + countSuffix;
        }

        lastLevel = avgLevel;
        lastCount = count;
    }

    public int GetAverageEmployeeLevel()
    {
        if (PlayerManager.instance == null) return 1;

        int totalLevel = 0;
        int count = PlayerManager.instance.hiredEmployees.Count;

        if (count == 0) return 1;

        foreach (HiredEmployeeScript emp in PlayerManager.instance.hiredEmployees)
        {
            totalLevel += emp.GetFinalLevel();
        }

        return totalLevel / count;
    }

    public int GetEmployeeCount()
    {
        if (PlayerManager.instance == null) return 0;
        return PlayerManager.instance.hiredEmployees.Count;
    }

    public void ForceUpdate()
    {
        UpdateDisplay();
    }

    public void OnEmployeesChanged()
    {
        UpdateDisplay();
    }
}