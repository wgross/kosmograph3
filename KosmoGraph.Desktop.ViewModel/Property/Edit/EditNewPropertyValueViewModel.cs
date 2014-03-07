namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Edits the value of a newly assigned Facets PropertyDefinietion at an entity
    /// A property value is always insitialized with a PropertyDefinitionViewModel. 
    /// Theproper defineitin has to be saved already befire ist is assigned to a value.
    /// </summary>
    public sealed class EditNewPropertyValueViewModel : EditPropertyValueViewModelBase
    {
        public EditNewPropertyValueViewModel(PropertyDefinitionViewModel fromDefinition)
            : base(fromDefinition)
        {}

        public override bool IsDirty
        {
            // new proerty values are always dirty
            get { return true; }
        }
    }
}
