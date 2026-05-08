using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class EnemySquash : MonoBehaviour
{
    [SerializeField] private AudioClip squashSound;
    private AudioSource audioSource;
    private bool isSquashed = false;

    void Start()
    {
        this.audioSource = this.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !this.isSquashed)
        {
            this.Squash();
        }
    }

    private void Squash()
    {
        this.isSquashed = true;

        if (this.squashSound != null)
        {
            AudioSource.PlayClipAtPoint(this.squashSound, this.transform.position);
        }

        DG.Tweening.Sequence squashSequence = DOTween.Sequence();

        squashSequence.Append(this.transform.DOScaleY(0.1f, 0.2f));

        squashSequence.OnComplete(() => Destroy(this.gameObject));
    }
}