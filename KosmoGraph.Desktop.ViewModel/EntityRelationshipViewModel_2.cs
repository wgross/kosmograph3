﻿namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using KosmoGraph.Services;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Text;

    public class EntityRelationshipViewModel
    {
        #region Construction and initialization of this instance 

        public EntityRelationshipViewModel(IManageEntitiesAndRelationships forService, IManageFacets withFacets)
        {
            this.EntityRelationshipService = forService;
            this.FacetService = withFacets;

            // initialize facets with all facets for facet service
            this.Facets = new ObservableCollection<FacetViewModel>();
            this.FacetService.GetAllFacets().EndWith(facets =>
            {
                // get all facets in different thread and observe changes lateron
                facets
                    .ToList()
                    .ForEach(f => this.Facets.Add(this.CreateFacetFromModelItem(f)));

                this.Facets.CollectionChanged += Facets_CollectionChanged;
            });

            // initialize all Entities, facets are needed!
            this.Entities = new ObservableCollection<EntityViewModel>();
            this.EntityRelationshipService.GetAllEntities().EndWith(entities =>
            {
                entities
                    .ToList()
                    .ForEach(e => this.Entities.Add(this.CreateNewEntityFromModelItem(e)));

                this.Entities.CollectionChanged += Entities_CollectionChanged;
            });

            this.Relationships = new ObservableCollection<RelationshipViewModel>();
            this.EntityRelationshipService.GetAllRelationships().EndWith(relationships =>
            {
                relationships.ToList().ForEach(r =>
                {
                    this.Relationships.Add(this.CreateRelationshipFromModelItem(r));
                });

                this.Relationships.CollectionChanged += Relationships_CollectionChanged;
            });

            this.Items = new ObservableCollection<ModelItemViewModelBase>(this.Facets
                .Cast<ModelItemViewModelBase>()
                .Union(this.Entities)
                .Union(this.Relationships));
        }

      
        private IManageEntitiesAndRelationships EntityRelationshipService { get; set; }

        private IManageFacets FacetService { get; set; }

        #endregion

        #region Facets of this model

        public ObservableCollection<FacetViewModel> Facets { get; private set; }

        void Facets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(i => this.Items.Add((ModelItemViewModelBase)i));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(i => this.Items.Remove((ModelItemViewModelBase)i));
                    break;
            }
        }

        #endregion 

        #region Entities of this model

        public ObservableCollection<EntityViewModel> Entities
        {
            get; private set;
        }

        private void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(i => this.Items.Add((FacetedModelItemViewModelBase)i));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(i => this.Items.Remove((FacetedModelItemViewModelBase)i));
                    break;
            }
        }

        #endregion Entities

        #region Relationships of this model

        public ObservableCollection<RelationshipViewModel> Relationships { get; private set; }

        private void Relationships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(i => this.Items.Add((FacetedModelItemViewModelBase)i));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(i => this.Items.Remove((FacetedModelItemViewModelBase)i));
                    break;
            }
        }

        #endregion 

        public ObservableCollection<ModelItemViewModelBase> Items { get; set; }

        public static EntityRelationshipViewModel CreateNew()
        {
            throw new NotImplementedException();
        }

        #region Create new model items

        public EditNewFacetViewModel CreateNewFacet()
        {
            return new EditNewFacetViewModel(this, this.FacetService);

            //if(this.Tags.Any(f => StringComparer.CurrentCultureIgnoreCase.Equals(f.Name, name)))
            //    throw new InvalidOperationException(string.Format("Duplicate facet name '{0}'", name));

            //return this.CreateFacet(this.FacetService.CreateNewFacet(f=>f.Name = name));
        }

        private FacetViewModel CreateFacetFromModelItem(Facet modelItem)
        {
            return new FacetViewModel(this, modelItem);
        }

        public EditNewEntityViewModel CreateNewEntity()
        {
            return new EditNewEntityViewModel(this, this.EntityRelationshipService);

            //if(this.Entities.Any(e => StringComparer.CurrentCultureIgnoreCase.Equals(e.Name, name)))
            //    throw new InvalidOperationException(string.Format("Duplicate entity name '{0}", name));

            //return this.CreateEntityFromModelItem(this.modelService.CreateNewEntity(e => e.Name = name));
        }

        private EntityViewModel GetOrCreateEntityFromModelItem(Entity modelItem)
        {
            // TODO: Where is the newly created entity added to the model. 
            return this.GetOrCreateEntityFromModelItemId(modelItem.Id) ?? this.CreateNewEntityFromModelItem(modelItem);
        }

        private EntityViewModel GetOrCreateEntityFromModelItemId(Guid entityId)
        {
            return this
                .Entities
                .FirstOrDefault(evm => evm.ModelItem.Id == entityId);
        }

        private EntityViewModel CreateNewEntityFromModelItem(Entity modelItem)
        {
            var newEntity = new EntityViewModel(this, modelItem)
            {
                // set new entity in left upper corner
                Top = 10,
                Left = 10
            };

            // initialize Entity with currently visible tags
            //TODO:PERF:Should be async, could take longer
            this.Facets
                .Where(t => t.IsVisible)
                .ToList()
                .ForEach(t => newEntity.Add(newEntity.CreateNewAssignedFacet(t)));

            return newEntity;
        }

        public EditNewRelationshipViewModel CreateNewRelationship(EntityViewModel from, EntityViewModel to)
        {
            return new EditNewRelationshipViewModel(from, to, this, this.EntityRelationshipService);
        }

        private RelationshipViewModel CreateRelationshipFromModelItem(Relationship modelItem)
        {
            return new RelationshipViewModel(
                new EntityConnectorViewModel(this.GetOrCreateEntityFromModelItemId(modelItem.FromId)),
                new EntityConnectorViewModel(this.GetOrCreateEntityFromModelItemId(modelItem.ToId)),
                modelItem);
        }

        private RelationshipViewModel CreateRelationshipFromModelItem(Relationship modelItem, Entity from, Entity to)
        {
            return new RelationshipViewModel(
                new EntityConnectorViewModel(this.GetOrCreateEntityFromModelItem(from)), 
                new EntityConnectorViewModel(this.GetOrCreateEntityFromModelItem(to)),
                modelItem);
        }

        #endregion 

        #region Edit existing model items

        public EditExistingEntityViewModel EditEntity(EntityViewModel entityToEdit)
        {
            return new EditExistingEntityViewModel(entityToEdit, this.EntityRelationshipService);
        }

        public EditExistingRelationshipViewModel EditRelationship(RelationshipViewModel relationshipViewModel)
        {
            return new EditExistingRelationshipViewModel(relationshipViewModel, this.EntityRelationshipService);
        }

        #endregion 

        public void ClearSelectedItems()
        {
            throw new NotImplementedException();
        }

        #region Add model items

        public EntityViewModel Add(Entity entity)
        {
            return this.Add(this.GetOrCreateEntityFromModelItem(entity));
        }

        public EntityViewModel Add(EntityViewModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!entity.Model.Equals(this))
                throw new InvalidOperationException("entity is attached to a foreign model");

            // handle adding the same entity twice: ingnore and return the existing one
            if (this.Entities.Contains(entity))
                return entity;

            // handle duplicate name: throw
            if (this.Entities.Any(e => StringComparer.InvariantCultureIgnoreCase.Equals(e.Name, entity.Name)))
                throw new InvalidOperationException("entity name must be unique");

            this.Entities.Add(entity);
            return entity;
        }

        public FacetViewModel Add(Facet facet)
        {
            return this.Add(this.CreateFacetFromModelItem(facet));
        }

        public FacetViewModel Add(FacetViewModel facet)
        {
            if (facet == null)
                throw new ArgumentNullException("facet");

            if (!facet.Model.Equals(this))
                throw new InvalidOperationException("facet is attacted to a foreign model");

            // handle adding same facet twice: ignor new facet, return existing one
            if (this.Facets.Contains(facet))
                return facet;

            // handle duplicate names by throwing
            if (this.Facets.Any(t => t.Name.Equals(facet.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new InvalidOperationException("Facet name is not unique");

            this.Facets.Add(facet);

            return facet;
        }

        public RelationshipViewModel Add(Relationship relationship, Entity from, Entity to)
        {
            return this.Add(this.CreateRelationshipFromModelItem(relationship, from, to));
        }

        private RelationshipViewModel Add(RelationshipViewModel relationship)
        {
            if (relationship == null)
                throw new ArgumentNullException("relationship");

            if (relationship.From == null)
                throw new ArgumentNullException("relationship.From");

            if (relationship.To == null)
                throw new ArgumentNullException("relationship.To");

            var alreadyAdded = this.Items
                .OfType<RelationshipViewModel>()
                .FirstOrDefault(r => (r.From.Equals(relationship.From) && r.To.Equals(relationship.To)) || (r.From.Equals(relationship.To) && r.To.Equals(relationship.From)));

            if (alreadyAdded != null)
                return alreadyAdded;

            this.Add(relationship.From.Entity);
            this.Add(relationship.To.Entity);
            this.Relationships.Add(relationship);

            return relationship;
        }

        #endregion 

        public void Remove(FacetViewModel facetToRemove)
        {
            this.FacetService
                .RemoveFacet(facetToRemove.ModelItem)
                .EndWith(ok => this.RemoveFacetFromViewModelItems(facetToRemove));
        }

        private void RemoveFacetFromViewModelItems(FacetViewModel facetToRemove)
        {
            this.Facets.Remove(facetToRemove);
            this.Entities.ForEach(e => e.RemoveAssignedFacet(facetToRemove));
            this.Relationships.ForEach(e => e.RemoveAssignedFacet(facetToRemove));
            // remove from entities
            // remove from retationships
        }

        public EditExistingFacetViewModel EditFacet(FacetViewModel facetViewModel)
        {
            return new EditExistingFacetViewModel(facetViewModel, this.FacetService);
        }
    }
}