namespace BecomeJPEG
{
	// this does not resize.
	internal class ArrayQueue<T>
	{
		private T[] array;
		private int firstIndex;
		private int lastIndex;
		private int size;

		public int Count
		{
			get;
			private set;
		}

		public T this[int index]
		{
			get
			{
				return array[(firstIndex + index) % size];
			}
		} 

		public ArrayQueue(int size) 
		{
			this.size = size;
			array = new T[size];
			firstIndex = 0;
			lastIndex = 0;
		}

		public void Enqueue(T item)
		{
			array[lastIndex %= size] = item;
			lastIndex++;
			Count++;
		}

		public T Dequeue()
		{
			if (Count == 0)
			{
				return default;
			}
			T element = array[firstIndex %= size];
			firstIndex++;
			Count--;
			return element;
		}
	}
}
