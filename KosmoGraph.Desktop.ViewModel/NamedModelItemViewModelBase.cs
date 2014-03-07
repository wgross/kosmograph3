namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NamedModelItemViewModelBase<TModelItem> : ModelItemViewModelBase
        where TModelItem : class, IHasName
    {
        #region Construction and initialization of this instance

        public NamedModelItemViewModelBase(EntityRelationshipViewModel parentEntityRelationshipModel, TModelItem modelItem)
            : base(parentEntityRelationshipModel)
        {
            if (modelItem == null)
                throw new ArgumentNullException("modelItem");

            this.ModelItem = modelItem;
        }

        public TModelItem ModelItem
        {
            get;
            private set;
        }

        #endregion 

        #region Has a name

        public string Name
        {
            get
            {
                return this.ModelItem.Name;
            }
            set
            {
                if (this.ModelItem.Name == value)
                    return;
                this.ModelItem.Name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        #endregion 
    }
}
