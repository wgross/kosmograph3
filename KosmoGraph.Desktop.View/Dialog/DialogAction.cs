﻿namespace KosmoGraph.Desktop.Dialog
{
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class DialogActionBuilder
    {
        public static DialogAction Ok(string title, Action<object> onOk)
        {
            return Ok(title, new DelegateCommand<object>(onOk));
        }

        public static DialogAction Ok(string title, ICommand contentCommitCommand)
        {
            return new DialogAction
            {
                IsDefault = true,
                IsCancel = false,
                Name = title,
                Command = DialogCommands.Ok,
                ContentCommand = contentCommitCommand
            };
        }

        public static DialogAction Ok(string title, params ICommand[] contentCommands)
        {
            if (contentCommands == null)
                throw new ArgumentNullException("contentCommands");

            var compositeCommand = new CompositeCommand();
            foreach (var cmd in contentCommands)
                compositeCommand.RegisterCommand(cmd);

            return Ok(title, compositeCommand);
        }

        public static DialogAction Cancel(string title, bool isDefaultCancel, Action<object> onCancel)
        {
            return Cancel(title, isDefaultCancel, new DelegateCommand<object>(onCancel));
        }

        public static DialogAction Cancel(string title, bool isDefaultCancel, ICommand contentCommand)
        {
            return new DialogAction
            {
                IsCancel = isDefaultCancel,
                IsDefault = false,
                Name = title,
                Command = DialogCommands.Cancel,
                ContentCommand = contentCommand
            };
        }

        public static DialogAction Cancel(string title, Action<object> onCancel)
        {
            return Cancel(title, new DelegateCommand<object>(onCancel));
        }

        public static DialogAction Cancel(string title, ICommand contentCommand)
        {
            return new DialogAction
            {
                IsCancel = true,
                IsDefault = false,
                Name = title,
                Command = DialogCommands.Cancel,
                ContentCommand = contentCommand
            };
        }

        public static DialogAction Cancel(string title, params ICommand[] contentCommands)
        {
            if (contentCommands == null)
                throw new ArgumentNullException("contentCommands");

            var compositeCommand = new CompositeCommand();
            foreach (var cmd in contentCommands)
                compositeCommand.RegisterCommand(cmd);

            return Cancel(title, compositeCommand);
        }
    }

    public class DialogAction
    { 
        public bool IsDefault { get; set; }

        public bool IsCancel { get; set; }

        public string Name { get; set; }

        public ICommand Command { get; set; }

        public ICommand ContentCommand { get; set; }
    }
}
