public interface IDamageable
{
    void Damage(ConditionType type, float damage);
}

public interface IInteractable
{
    public (string, string, int) GetObjectInfo();
    public void OnInteractInput();
}