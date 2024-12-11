using UnityEngine;

public class BreakablePot : MonoBehaviour
{
    public GameObject shardPrefab; // 파편 프리팹
    public AudioClip breakSoundEffect; // 깨지는 효과음
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 바닥 태그 확인
        {
            BreakPot();
        }
    }

    private void BreakPot()
    {
        if (breakSoundEffect != null)
        {
            audioSource.PlayOneShot(breakSoundEffect);
        }

        // 파편 생성
        for (int i = 0; i < 5; i++)
        {
            GameObject shard = Instantiate(shardPrefab, transform.position, Random.rotation);
            Rigidbody rb = shard.AddComponent<Rigidbody>();
            rb.AddExplosionForce(200f, transform.position, 1f);
            Destroy(shard, 5f); // 5초 뒤 파편 제거
        }

        Destroy(gameObject); // 도자기 제거
    }
}