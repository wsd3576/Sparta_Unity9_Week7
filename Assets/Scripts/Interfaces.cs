public interface IDamageable
{
    void Damage(ConditionType type, float damage);
}

public interface IInteractable
{
    public (string, string) GetObjectInfo();
    public void OnInteractInput();
}