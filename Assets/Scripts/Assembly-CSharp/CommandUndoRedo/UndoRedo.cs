namespace CommandUndoRedo
{
	public class UndoRedo
	{
		private DropoutStack<ICommand> undoCommands = new DropoutStack<ICommand>();

		private DropoutStack<ICommand> redoCommands = new DropoutStack<ICommand>();

		public int maxUndoStored
		{
			get
			{
				return undoCommands.maxLength;
			}
			set
			{
				SetMaxLength(value);
			}
		}

		public UndoRedo()
		{
		}

		public UndoRedo(int maxUndoStored)
		{
			this.maxUndoStored = maxUndoStored;
		}

		public void Clear()
		{
			undoCommands.Clear();
			redoCommands.Clear();
		}

		public void Undo()
		{
			if (undoCommands.Count > 0)
			{
				ICommand command = undoCommands.Pop();
				command.UnExecute();
				redoCommands.Push(command);
			}
		}

		public void Redo()
		{
			if (redoCommands.Count > 0)
			{
				ICommand command = redoCommands.Pop();
				command.Execute();
				undoCommands.Push(command);
			}
		}

		public void Insert(ICommand command)
		{
			if (maxUndoStored > 0)
			{
				undoCommands.Push(command);
				redoCommands.Clear();
			}
		}

		public void Execute(ICommand command)
		{
			command.Execute();
			Insert(command);
		}

		private void SetMaxLength(int max)
		{
			undoCommands.maxLength = max;
			redoCommands.maxLength = max;
		}
	}
}
