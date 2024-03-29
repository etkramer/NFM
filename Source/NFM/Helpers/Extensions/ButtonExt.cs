﻿using System.Windows.Input;
using Avalonia.Controls;

namespace NFM;

public static partial class ButtonExt
{
	public class CommandImpl : ICommand
	{
		private readonly Action commandAction;

		public CommandImpl(Action command)
		{
			commandAction = command;
		}

		#pragma warning disable CS0067
		public event EventHandler CanExecuteChanged;

		bool ICommand.CanExecute(object parameter)
		{
			return commandAction is not null;
		}

		void ICommand.Execute(object parameter)
		{
			commandAction?.Invoke();
		}
		#pragma warning restore CS0067
	}

	public static T OnClick<T>(this T subject, Action command) where T : Button
	{
		subject.Command = new CommandImpl(command);
		return subject;
	}
}
