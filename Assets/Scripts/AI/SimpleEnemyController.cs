using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyController : MonoBehaviour
{
    [SerializeField]
    private Human target;

    [SerializeField]
    private float seekRadius = 5;

    [SerializeField]
    private float distanceFromTarget = 2;

    [SerializeField]
    private float patrolRadius = 3f; // Радиус патрулирования вокруг начальной позиции

    private Human player;
    private NavMeshAgent agent;
    private Vector3 startPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position; // Сохраняем начальную позицию для ограничения радиуса патрулирования
    }

    private IEnumerator Start()
    {
        StartCoroutine(SeekTarget());

        while (true)
        {
            if (player != null)
            {
                float distToPlayer = Vector3.Distance(player.transform.position, target.transform.position);

                if (distToPlayer < distanceFromTarget)
                {
                    target.Attack(player);
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                Vector3 randomDestination = RandomNavmeshLocation(patrolRadius); // Патрулирование в пределах заданного радиуса
                agent.SetDestination(randomDestination);
                yield return new WaitUntil(() => agent.remainingDistance < 1 || player != null);
            }
        }
    }

    private IEnumerator SeekTarget()
    {
        while (true)
        {
            if (player == null)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, seekRadius, LayerMask.GetMask("Player"));

                foreach (var collider in hitColliders)
                {
                    Human foundPlayer = collider.GetComponent<Human>();
                    if (foundPlayer != null)
                    {
                        player = foundPlayer;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // Метод для поиска случайной точки на NavMesh в заданном радиусе от начальной позиции
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += startPosition; // Используем начальную позицию как центр радиуса
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
        return hit.position;
    }
}
