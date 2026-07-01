using UnityEngine;

public class EmployeeSlotManager : MonoBehaviour
{
    public static EmployeeSlotManager instance;

    [Header("Employee Positions (assigned at runtime per office)")]
    public Transform[] employeeSlots;
    private EmployeeCharacter[] occupiedSlots;

    void Awake()
    {
        // Keep a single persistent instance across office swaps.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (employeeSlots == null)
        {
            employeeSlots = new Transform[0];
        }

        occupiedSlots = new EmployeeCharacter[employeeSlots.Length];
    }

    /// <summary>
    /// Call this whenever the player upgrades/changes office.
    /// Moves all currently placed employees into the new slot layout,
    /// instead of losing them.
    /// </summary>
    public void ChangeOffice(Transform[] newSlots)
    {
        // Collect everyone currently placed, in order.
        EmployeeCharacter[] currentEmployees = new EmployeeCharacter[occupiedSlots != null ? occupiedSlots.Length : 0];
        int count = 0;

        if (occupiedSlots != null)
        {
            for (int i = 0; i < occupiedSlots.Length; i++)
            {
                if (occupiedSlots[i] != null)
                {
                    currentEmployees[count] = occupiedSlots[i];
                    count++;
                }
            }
        }

        // Swap in the new office's slot layout.
        employeeSlots = newSlots != null ? newSlots : new Transform[0];
        occupiedSlots = new EmployeeCharacter[employeeSlots.Length];

        // Re-seat everyone into the new office's positions.
        for (int i = 0; i < count; i++)
        {
            if (i < employeeSlots.Length)
            {
                occupiedSlots[i] = currentEmployees[i];
                currentEmployees[i].transform.position = employeeSlots[i].position;
            }
            else
            {
                // New office somehow has fewer slots than employees placed.
                // Employee stays in the list logically (PlayerManager.hiredEmployees)
                // but has no physical seat. Optionally hide or handle as needed.
                Debug.LogWarning(currentEmployees[i].name + " has no slot in the new office layout.");
            }
        }
    }

    public bool AddEmployee(EmployeeCharacter employee)
    {
        for (int i = 0; i < employeeSlots.Length; i++)
        {
            if (occupiedSlots[i] == null)
            {
                occupiedSlots[i] = employee;
                employee.transform.position = employeeSlots[i].position;
                Debug.Log(employee.name + " placed in Slot " + (i + 1));
                return true;
            }
        }
        Debug.Log("Office is full!");
        return false;
    }

    public void RemoveEmployee(EmployeeCharacter employee)
    {
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i] == employee)
            {
                occupiedSlots[i] = null;
                return;
            }
        }
    }

    public int GetAvailableSlots()
    {
        int freeSlots = 0;
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i] == null)
            {
                freeSlots++;
            }
        }
        return freeSlots;
    }
}