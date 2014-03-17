namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditRelationshipViewModelBase : EditFacetedViewModelBase
    {
        public EditRelationshipViewModelBase(string withTitleFormat)
        {
            this.titleFormat = withTitleFormat;
        }

        public EditRelationshipViewModelBase(string withTitleFormat, EntityViewModel from)
        {
            this.titleFormat = withTitleFormat;
            this.From = from;
        }

        private readonly string titleFormat;

        public EntityViewModel From { get; private set; }

        public abstract EntityViewModel To { get; }

        public bool EnableCommit { get; set; }

        #region Title for edit dialog view

        public string Title
        {
            get
            {
                if (this.To == null)
                    return string.Format(this.titleFormat, this.From.Name, "?");
                
                return string.Format(this.titleFormat, this.From.Name, this.To.Name);
            }
        }

        #endregion 
    }
}
