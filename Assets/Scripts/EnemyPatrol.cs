using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Vector3 targetOffset = new Vector3(5f, 0f, 0f);
    [SerializeField] private float duration = 2f;

    void Start()
    {
        Vector3 startPos = this.transform.position;
        Vector3 endPos = startPos + this.targetOffset;

        DG.Tweening.Sequence patrolSequence = DOTween.Sequence();

        patrolSequence.Append(this.transform.DOMove(endPos, this.duration).SetEase(Ease.Linear));

        patrolSequence.AppendCallback(() => this.transform.Rotate(0f, 180f, 0f));

        patrolSequence.Append(this.transform.DOMove(startPos, this.duration).SetEase(Ease.Linear));

        patrolSequence.AppendCallback(() => this.transform.Rotate(0f, 180f, 0f));

        patrolSequence.SetLoops(-1);
    }
}