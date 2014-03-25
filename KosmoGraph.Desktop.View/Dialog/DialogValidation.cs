namespace KosmoGraph.Desktop.Dialog
{
    using KosmoGraph.Desktop.Dialog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class DialogValidationBuilder
    {
        public static DialogValidation Validate(ICommand contentValidationCommand)
        {
            return new DialogValidation
            {
                Command = DialogCommands.Validate,
                ContentCommand = contentValidationCommand
            };
        }
    }

    public class DialogValidation
    { 
        public ICommand Command { get; set; }

        public ICommand ContentCommand { get; set; }
    }
}
