using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspectInjector.Broker;

namespace NFM
{
	[AttributeUsage(AttributeTargets.Method)]
	[Injection(typeof(CommandAspect))]
	public class CommandAttribute : Attribute
	{
		public string UndoMethod { get; set; }
		public CommandAttribute(string undoMethod)
		{
			UndoMethod = undoMethod;
		}
	}

	public class Command
	{
		#region Instance
		public string Name;
		public readonly Action UndoAction;
		public readonly Action RedoAction;

		public Command(string name, Action undoAction, Action redoAction)
		{
			Name = name;
			UndoAction = undoAction;
			RedoAction = redoAction;
		}
		#endregion

		public const int UndoHistoryLength = 32;

		private static List<Command> undoHistory = new();
		private static int undoIndex = -1;

		public static void Undo()
		{
			if (undoIndex == -1)
				return;

			// Invoke the undo action.
			undoHistory[undoIndex].UndoAction.Invoke();

			// Don't decrement the index if this was the first command.
			if (undoIndex > 0)
			{
				undoIndex--;
			}
		}

		public static void Redo()
		{
			if (undoIndex == -1)
			{
				return;
			}

			// Invoke the redo action.
			undoHistory[undoIndex].RedoAction?.Invoke();

			// Don't decrement the index if this was the most recent command.
			if (undoIndex < undoHistory.Count - 1)
			{
				undoIndex++;
			}
		}

		public static void DoCommand(Action command, Action undo, string name)
		{
			command.Invoke();
			AddCommand(undo, command, name);
		}

		public static void AddCommand(Action undo, Action redo, string name)
		{
			if (undoHistory.Count > undoIndex + 1)
			{
				undoHistory.RemoveRange(undoIndex + 1, undoHistory.Count); // Clear potential redos.
			}

			undoHistory.Add(new Command(name, undo, redo));
			undoIndex++;

			if (undoIndex > UndoHistoryLength)
			{
				undoIndex--;
				undoHistory.RemoveAt(0);
			}
		}
	}

	[Aspect(Scope.PerInstance)]
	public sealed class CommandAspect
	{
		[Advice(Kind.Around, Targets = Target.Method | Target.AnyAccess)]
		public object AroundMethod([Argument(Source.Instance)] object source, [Argument(Source.Triggers)] Attribute[] triggers, [Argument(Source.Type)] Type type, [Argument(Source.Arguments)] object[] args, [Argument(Source.Target)] Func<object[], object> func, [Argument(Source.Name)] string name)
		{
			CommandAttribute commandAttribute = triggers.OfType<CommandAttribute>().FirstOrDefault();
			if (commandAttribute != null)
			{
				MethodInfo undoMethod = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(o => o.Name == commandAttribute.UndoMethod).FirstOrDefault();
				if (undoMethod == null)
				{
					throw new InvalidOperationException("Method with [Command] attriute must have matching undo method with same arguments");
				}

				Action undoAction = () => undoMethod.Invoke(source, args);
				Action redoAction = () => func.Invoke(args);
				Command.AddCommand(undoAction, redoAction, name.PascalToDisplay());
			}

			return func.Invoke(args);
		}
	}
}
