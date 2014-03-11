namespace KosmoGraph.Desktop.Dialog
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class DialogAction
    {
        public static DialogAction Ok(string title, ICommand contentCommand)
        {
            return new DialogAction
            {
                IsDefault=true,
                IsCancel=false,
                Name=title,
                Command = DialogCommands.Ok,
                ContentCommand = contentCommand
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

        public static DialogAction Cancel(string title, ICommand contentCommand)
        {
            return new DialogAction
            {
                IsCancel=true,
                IsDefault=false,
                Name=title,
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

        public bool IsDefault { get; set; }
        
        public bool IsCancel { get;set;}

        public string Name {get; set;}

        public ICommand Command { get; set; }

        public ICommand ContentCommand { get; set; }
    }

    public sealed class DialogViewModel : NotificationObject
    {
        #region Dialog Size

        #endregion 

        #region Dialog is populated with this data element

        public ObservableCollection<object> DialogContent
        {
            get
            {
                return this.dialogContent;
            }
            set
            {
                if (object.ReferenceEquals(this.dialogContent, value))
                    return;
                this.dialogContent = value;
                this.RaisePropertyChanged(() => this.DialogContent);
            }
        }

        private ObservableCollection<object> dialogContent;

        #endregion 

        #region Dialog provides dialog actions

        public IEnumerable<DialogAction> DialogActions
        {
            get
            {
                return this.dialogActions;
            }
            set
            {
                if (object.ReferenceEquals(value, this.dialogActions))
                    return;
                this.dialogActions = value;
                this.RaisePropertyChanged(() => this.DialogActions);
            }
        }

        private IEnumerable<DialogAction> dialogActions;

        #endregion
    }
}
