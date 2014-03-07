namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class ModelItemViewModelBase : NotificationObject
    {
        #region Construction and initialization of this instance

        public ModelItemViewModelBase(EntityRelationshipViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("entityRelationshipModel");

            this.entityRelationshipModel = model;
        }

        public EntityRelationshipViewModel Model
        {
            get
            {
                return this.entityRelationshipModel;
            }
        }

        private readonly EntityRelationshipViewModel entityRelationshipModel;

        #endregion 

        #region A model item has an selection state

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected == value)
                    return;

                this.isSelected = value;
                this.RaisePropertyChanged(() => this.IsSelected);
                this.OnIsSelectedChanged(this.isSelected);
            }
        }

        private bool isSelected;

        virtual protected void OnIsSelectedChanged(bool newValue)
        {
        }

        #endregion 

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                if (this.isVisible == value)
                    return;

                this.isVisible = value;
                this.OnIsVisibleChanged(this.IsVisible);
                this.RaisePropertyChanged(() => this.IsVisible);
            }
        }

        private bool isVisible = false;

        virtual protected void OnIsVisibleChanged(bool newValue)
        {

        }
    }
}
