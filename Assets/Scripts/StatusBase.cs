
public class StatusBase
{
    public float maxHP;
    public float speed;
    public float currentHP;

    public void Initialize(float maxHealth, float spd)
    {
        maxHP = maxHealth;
        speed = spd;
        currentHP = maxHP;
    }
    
}
