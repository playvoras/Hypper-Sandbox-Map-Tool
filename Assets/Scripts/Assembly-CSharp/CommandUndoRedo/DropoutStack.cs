using System.Collections.Generic;

namespace CommandUndoRedo
{
	public class DropoutStack<T> : LinkedList<T>
	{
		private int _maxLength = int.MaxValue;

		public int maxLength
		{
			get
			{
				return _maxLength;
			}
			set
			{
				SetMaxLength(value);
			}
		}

		public DropoutStack()
		{
		}

		public DropoutStack(int maxLength)
		{
			this.maxLength = maxLength;
		}

		public void Push(T item)
		{
			if (base.Count > 0 && base.Count + 1 > maxLength)
			{
				RemoveLast();
			}
			if (base.Count + 1 <= maxLength)
			{
				AddFirst(item);
			}
		}

		public T Pop()
		{
			T value = base.First.Value;
			RemoveFirst();
			return value;
		}

		private void SetMaxLength(int max)
		{
			_maxLength = max;
			if (base.Count > _maxLength)
			{
				int num = base.Count - _maxLength;
				for (int i = 0; i < num; i++)
				{
					RemoveLast();
				}
			}
		}
	}
}
