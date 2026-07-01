using UnityEngine;

public class EmployeeCharacter : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;


    public void SetupCharacter(Sprite employeeSprite)
    {
        spriteRenderer.sprite = employeeSprite;
    }
}