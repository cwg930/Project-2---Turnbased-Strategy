using UnityEngine;
using System.Collections;

/*	
 * Generic Priority Queue
 * constructors: 
 * 		PriorityQueue(int)
 * public functions 
 * 		Remove() : Tuple<T,int> 
 * 		insert(T, int) : void
 *  
*/
public class PriorityQueue<T>  
{

	private Tuple<T,int>[] heap;
	private int numItems;
	public int Count
	{
		get { return numItems; }
	}
		
	public bool Empty
	{
		get { return numItems == 0;}
	}

	public PriorityQueue(int size)
	{
		heap = new Tuple<T, int>[size];
		numItems = 0;
	}

	/* Insert an element of type T then 
	 * 	rearrange the heap
	 */
	public void Enqueue(T element, int priority)
	{
		heap [numItems] = new Tuple<T,int>(element, priority);
		PercolateUp (numItems);
		numItems++;
	}
	/* Remove the head element then
	 * 	rearrange the heap
	 * Returns only the element
	 */
	public T Dequeue()
	{
		Tuple<T,int> top = heap [0];
		heap [0] = heap [numItems];
		numItems--;
		PercolateDown ();
		return top.first;
	}
	/* Remove the head element then
	 * 	rearrange the heap
	 * Returns the element and its
	 * 	priority
	 */
	public Tuple<T,int> DequeueWithPriority()
	{
		Tuple<T,int> top = heap [0];
		heap [0] = heap [numItems];
		numItems--;
		PercolateDown ();
		return top;
	}


	/* Percolates the top node down 
	 * 	to its proper position
	 */
	private void PercolateDown()
	{
		int u,v;
		v = 1;
		
		while (true) {
			u = v;
			if(2*u + 1 <= numItems)
			{
				if(heap[u].second >= heap[2*u].second)
					v = 2*u;
				if(heap[v].second >= heap[2*u+1].second)
					v = 2*u+1;
			}
			else if (2*u <= numItems)
			{
				if(heap[u].second >= heap[2*u].second)
					v = 2*u;
			}
			if (u != v)
			{
				Tuple<T, int> temp = heap[u];
				heap[u] = heap[v];
				heap[v] = temp;
			}
			else
			{
				break;
			}
		}
	}
	/*	Percolates the node at <index> up to it's 
	 *	proper position recursively  
	 */
	private void PercolateUp(int index)
	{
		int parent = index / 2;
		if (heap [index].second < heap [parent].second) {
			Tuple<T, int> temp = heap[parent];
			heap[parent] = heap[index];
			heap[index] = temp;
			PercolateUp(parent);
		}
	}
}
