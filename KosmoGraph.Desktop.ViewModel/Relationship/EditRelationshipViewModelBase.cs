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

        public EditRelationshipViewModelBase(string withTitleFormat, EntityViewModel from, EntityViewModel to)
        {
            this.titleFormat = withTitleFormat;
            this.From = from;
            this.To = to;
        }

        private string titleFormat;

        public EntityViewModel From { get; private set; }

        public EntityViewModel To { get; private set; }

        public bool EnableCommit { get; set; }

        #region Title for edit dialog view

        public string Title
        {
            get
            {
                return string.Format(this.titleFormat, this.From.Name, this.To.Name);
            }
        }

        #endregion 
    }
}
