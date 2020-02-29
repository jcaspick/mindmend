using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public Node a;
    public Node b;
    private int health;
    private int maxHealth;
    public GameColor color;

    public ConnectionVisualBase visual;

    void OnDestroy()
    {
        if (visual != null)
            Destroy(visual.gameObject);
    }

    public void CreateVisuals(float angle)
    {
        visual = Instantiate(GameSettings.instance.graphics.connection);
        visual.Create(a.transform.position, b.transform.position, angle);
        visual.SetHealth(health, maxHealth);
    }

    public void Break() {
        visual.Break();
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int health)
    {
        this.health = health;
        if (visual != null)
            visual.SetHealth(health, maxHealth);
    }

    public void ModifyHealth(int delta)
    {
        health += delta;
        if (visual != null)
            visual.SetHealth(health, maxHealth);
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        if (visual != null)
            visual.SetHealth(health, maxHealth);
    }

    public void SetColor(GameColor color)
    {
        this.color = color;
        visual.SetColor(color);
    }
}