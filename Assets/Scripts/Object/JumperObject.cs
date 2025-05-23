using UnityEngine;

public class JumperObject : MonoBehaviour
{
    [SerializeField] private float jumpPower;
    private Rigidbody _rigidbody;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) //플레이어 일 때만
        {
            _rigidbody = other.gameObject.GetComponent<Rigidbody>();

            if (other.relativeVelocity.y < 0f && other.transform.position.y > transform.position.y) //플레이어가 위에서 내려온 경우 일 때만
            {
                _rigidbody.AddForce(Vector2.up * (jumpPower), ForceMode.Impulse);
            }
        }
    }
}
