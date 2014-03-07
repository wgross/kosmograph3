namespace KosmoGraph.Desktop.ViewModel
{
//    using KosmoGraph.Model;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//    public abstract class AssignedTagViewModelBase<TAssignedTag, TParent, TPropertyValue>
//        where TAssignedTag : IAssignedTag<TParent, TPropertyValue>
//    {
//        #region Construction and initialization of this instance

//        public AssignedTagViewModelBase(TagViewModel tag, TAssignedTag modelItem)
//        {
//            this.ModelItem = modelItem;
//            this.Tag = tag;
//            this.Tag.Properties.CollectionChanged += PropertyDefinitions_CollectionChanged;
//            this.Properties = new ObservableCollection<PropertyValueViewModel>();
//            this.ModelItem
//                .Properties
//                .ToList()
//                .ForEach(pv => 
//                {
//                    this.Properties.Add(this.Tag
//                        .Properties
//                        .First(pd => pd.ModelItem.Id == pv.DefinitionId)
//                        .CreatePropertyValue(pv));
//                });
//        }

//        void PropertyDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            if (e.Action == NotifyCollectionChangedAction.Add)
//            {
//                e.NewItems
//                    .Cast<PropertyDefinitionViewModel>()
//                    .Select(pd => pd.CreateNewPropertyValue(this.ModelItem))
//                    .ToList()
//                    .ForEach(pv => 
//                    {
//                        this.ModelItem.Properties.Add(pv.ModelItem);
//                        this.Properties.Add(pv);
//                    });
//            }
//            else if (e.Action == NotifyCollectionChangedAction.Remove)
//            {
//                e.OldItems
//                    .Cast<PropertyDefinitionViewModel>()
//                    .ToList()
//                    .ForEach(pd => 
//                    {
//                        var propertyToRemove = this.Properties.First(p => p.Definition.Equals(pd));
//                        this.ModelItem.Properties.Remove(propertyToRemove.ModelItem);
//                        this.Properties.Remove(propertyToRemove);
//                    });
//            }
//        }

//        public TagViewModel Tag { get; private set; }

//        public TAssignedTag ModelItem
//        {
//            get;
//            private set;
//        }
//        #endregion

//        public TagViewModel Tag { get; private set; }

//        public ObservableCollection<PropertyValueViewModel> Properties { get; private set; }
//    }
}
