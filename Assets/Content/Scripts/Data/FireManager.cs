
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;

    [Header("Fire Settings")]
    public GameObject firePrefab;
    public LayerMask flammableLayers;
    public LayerMask inFlammableLayers;
    public float spreadRadius = 1.5f;
    public int maxAttemptsPerFire = 15;

    [Header("Global Spawn Settings")]
    public float globalSpawnInterval = 8f;
    public int globalSpawnAttempts = 25;
    public Collider2D mapArea;
    private Vector2 mapMinBounds;
    private Vector2 mapMaxBounds;
    private bool isStarted;

    private PriorityQueue<FireBehavior> fireQueue = new PriorityQueue<FireBehavior>();
    private HashSet<FireBehavior> pendingRemovals = new HashSet<FireBehavior>();
    private float nextGlobalSpawnTime;

    void Awake()
    {
        isStarted = false;
        Instance = this;
    }

    public void StartFireCycle()
    {
        nextGlobalSpawnTime = Time.time + globalSpawnInterval;
        mapMinBounds = mapArea.bounds.min;
        mapMaxBounds = mapArea.bounds.max;
        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        int processedCount = 0;
        int maxPerFrame = fireQueue.Count;

        while(fireQueue.Count > 0 && processedCount < maxPerFrame)
        {
            FireBehavior fire = fireQueue.Peek();

            if (fire == null || pendingRemovals.Contains(fire))
            {
                fireQueue.Pop();
                continue;
            }

            if (Time.time >= fire.NextSpreadTime)
            {
                fireQueue.Pop();
                if (!pendingRemovals.Contains(fire))
                {
                    TrySpreadFrom(fire);
                    fire.ResetSpreadTimer();
                    fireQueue.Push(fire);
                }
                processedCount++;
            }
            else
            {
                break;
            }
        }

        
        if (pendingRemovals.Count > 0)
        {
            foreach (var fire in pendingRemovals)
            {
                fireQueue.Remove(fire);
            }
            pendingRemovals.Clear();
        }


        if (Time.time >= nextGlobalSpawnTime)
        {
            SpawnGlobalFire();
            nextGlobalSpawnTime = Time.time + globalSpawnInterval;
        }
    }

    public void RegisterFire(FireBehavior fire)
    {
        if (fire != null)
            fireQueue.Push(fire);
    }

    public void UnregisterFire(FireBehavior fire)
    {
        if (fire != null)
            pendingRemovals.Add(fire);
    }

    void TrySpreadFrom(FireBehavior fire)
    {
        Vector2 center = fire.transform.position;

        for (int attempts = 0; attempts < maxAttemptsPerFire; attempts++)
        {
            Vector2 targetSpawn = center + UnityEngine.Random.insideUnitCircle * spreadRadius;

            if (!Physics2D.OverlapCircle(targetSpawn, 0.2f, inFlammableLayers)
                && Physics2D.OverlapCircle(targetSpawn, 0.2f, flammableLayers))
            {
                GameObject newFire = Instantiate(firePrefab, targetSpawn, Quaternion.identity);
                FireBehavior fb = newFire.GetComponent<FireBehavior>();
                fb.Initialize();
                RegisterFire(fb);
                break;
            }
        }
    }

    void SpawnGlobalFire()
    {
        for (int attempts = 0; attempts < globalSpawnAttempts; attempts++)
        {
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(mapMinBounds.x, mapMaxBounds.x),
                UnityEngine.Random.Range(mapMinBounds.y, mapMaxBounds.y)
            );

            if (!Physics2D.OverlapCircle(randomPos, 0.2f, inFlammableLayers)
                && Physics2D.OverlapCircle(randomPos, 0.2f, flammableLayers))
            {
                GameObject newFire = Instantiate(firePrefab, randomPos, Quaternion.identity);
                FireBehavior fb = newFire.GetComponent<FireBehavior>();
                fb.Initialize();
                RegisterFire(fb);
                break;
            }
        }
    }
}

public class PriorityQueue<T> where T : FireBehavior
{
    private List<T> heap = new List<T>();

    public int Count => heap.Count;

    public void Push(T item)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1);
    }

    public T Pop()
    {
        if (heap.Count == 0) return null;
        T root = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return root;
    }

    public T Peek()
    {
        return heap.Count > 0 ? heap[0] : null;
    }

    public void Remove(T item)
    {
        int index = heap.IndexOf(item);
        if (index < 0) return;

        heap[index] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(index);
        HeapifyUp(index);
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].NextSpreadTime >= heap[parent].NextSpreadTime) break;
            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;
        while (true)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].NextSpreadTime < heap[smallest].NextSpreadTime)
                smallest = left;
            if (right <= lastIndex && heap[right].NextSpreadTime < heap[smallest].NextSpreadTime)
                smallest = right;

            if (smallest == index) break;
            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
}
