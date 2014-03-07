namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    //using KosmoGraph.Desktop.ViewModel.Layout;
    using KosmoGraph.Model;
    using KosmoGraph.Persistence;
    using KosmoGraph.Persistence.Xml;
    
    public class EntityRelationshipViewModel : ViewModel
    {
        #region Creation from model store

        public static EntityRelationshipViewModel CreateNew()
        {
            return CreateNewFromStore(new XmlModelDatabase());
        }

        public static EntityRelationshipViewModel CreateNewFromStore(IModelStore modelStore)
        {
            var model = new EntityRelationshipViewModel(new XmlModelDatabase());

            modelStore.Tag
                .Query()
                .ToList()
                .ForEach(t => model.Add(model.CreateTag(t)));

            modelStore.Entity
                .Query()
                .ToList()
                .ForEach(e => model.Add(model.CreateEntity(e)));

            modelStore.Relationship
                .Query()
                .ToList()
                .ForEach(e => model.Add(model.CreateRelationship(e)));

            return model;
        }

        #endregion Creation from model store

        #region Construction and initialization of this instance

        private EntityRelationshipViewModel(IModelStore modelStore)
        {
            this.ModelStore = modelStore;
            this.tags = new ObservableCollection<TagViewModel>();
            this.tags.CollectionChanged += tags_CollectionChanged;
            this.entities = new ObservableCollection<EntityViewModel>();
            this.entities.CollectionChanged += entities_CollectionChanged;
            this.relationships = new ObservableCollection<RelationshipViewModel>();
            this.relationships.CollectionChanged += relationships_CollectionChanged;
            this.Events = new EventAggregator();
        }

        #endregion Construction and initialization of this instance

        public void AddDummyData()
        {
            var a = this.Add(this.CreateNewEntity("test1"));
            a.Left = 10;
            a.Top = 10;

            var b = this.Add(this.CreateNewEntity("test2"));
            b.Left = 100;
            b.Top = 100;

            var relationship1 = this.Add(this.CreateNewRelationship(a, b));

            var c = this.Add(this.CreateNewEntity("test3"));
            c.Left = 150;
            c.Top = 150;

            var tag1 = this.Add(this.CreateNewTag("tag1"));
            tag1.Add(tag1.CreateNewPropertyDefinition("property1"));
            relationship1.Add(relationship1.CreateNewAssignedTag(tag1));

            this.Add(this.CreateNewTag("tag2"));
        }

        #region The model behind the View Model

        
        #endregion The model behind the View Model

        #region Create model item view models

        public TagViewModel CreateNewTag(string name)
        {
            return this.CreateTag(this.ModelStore.Tag.CreateNew(t => t.Name = name));
        }

        private TagViewModel CreateTag(Tag modelItem)
        {
            return new TagViewModel(this, modelItem);
        }

        public EntityViewModel CreateNewEntity(string name)
        {
            return this.CreateEntity(this.ModelStore.Entity.CreateNew(e => e.Name = name));
        }

        private EntityViewModel CreateEntity(Entity modelItem)
        {
            var newEntity = new EntityViewModel(this, modelItem)
            {
                Top = 10,
                Left = 10
            };

            // initialize Entity with currently visible tags
            //TODO:PERF:Should be async, could take longer
            this.Tags
                .Where(t => t.IsVisible)
                .ToList()
                .ForEach(t => newEntity.Add(newEntity.CreateNewAssignedTag(t)));

            return newEntity;
        }

        public RelationshipViewModel CreateNewRelationship(EntityViewModel from, EntityViewModel to)
        {
            var newRelationship = this.CreateRelationship(from, to, this.ModelStore.Relationship.CreateNew(r =>
            {
                r.FromId = from.ModelItem.Id;
                r.ToId = to.ModelItem.Id;
            }));

            // initialize thid reletionship woth currently visible tags from model
            this.Tags
                .Where(t => t.IsVisible)
                .ToList()
                .ForEach(t => newRelationship.Add(newRelationship.CreateNewAssignedTag(t)));

            return newRelationship;
        }

        private RelationshipViewModel CreateRelationship(Relationship modelItem)
        {
            return this.CreateRelationship(
                this.Entities.Single(e => e.ModelItem.Id == modelItem.FromId),
                this.Entities.Single(e => e.ModelItem.Id == modelItem.ToId),
                modelItem);
        }

        private RelationshipViewModel CreateRelationship(EntityViewModel from, EntityViewModel to, Relationship modelItem)
        {
            return new RelationshipViewModel(from.CentralConnector, to.CentralConnector, modelItem);
        }

        #endregion Create model item view models

        #region Aggregate view model events

        public IEventAggregator Events { get; private set; }

        #endregion Aggregate view model events

        #region Access the elements of the entity relationship model

        public ObservableCollection<TaggedModelItemViewModelBase> Items
        {
            get
            {
                return this.items;
            }
        }

        private readonly ObservableCollection<TaggedModelItemViewModelBase> items = new ObservableCollection<TaggedModelItemViewModelBase>();

        #endregion Access the elemets of the entity relationship model

        #region Relationships

        public IEnumerable<RelationshipViewModel> Relationships
        {
            get
            {
                return this.relationships;
            }
        }

        private ObservableCollection<RelationshipViewModel> relationships = null;

        private void relationships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(r => this.Items.Add((TaggedModelItemViewModelBase)r));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(r => this.Items.Remove((TaggedModelItemViewModelBase)r));
                    break;
            }
        }

        #endregion Relationships

        #region Entities

        public IEnumerable<EntityViewModel> Entities
        {
            get
            {
                return this.entities;
            }
        }

        private readonly ObservableCollection<EntityViewModel> entities = null;

        private void entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(i => this.Items.Add((TaggedModelItemViewModelBase)i));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(i => this.Items.Remove((TaggedModelItemViewModelBase)i));
                    break;
            }
        }

        #endregion Entities

        #region Tags

        public ObservableCollection<TagViewModel> Tags
        {
            get
            {
                return this.tags;
            }
        }

        private readonly ObservableCollection<TagViewModel> tags;

        #endregion Tags

        public void ClearSelectedItems()
        {
            foreach (var item in this.Items)
                item.IsSelected = false;
            foreach (var tag in this.Tags)
                tag.IsItemSelected = false;
        }

        #region Add and remove relationships

        public RelationshipViewModel Add(RelationshipViewModel relationship)
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
            this.ModelStore.Relationship.Add(relationship.ModelItem);
            this.relationships.Add(relationship);

            return relationship;
        }

        public bool Remove(RelationshipViewModel relationship)
        {
            if (relationship == null)
                throw new ArgumentNullException("relationship");

            this.ModelStore.Relationship.Remove(relationship.ModelItem);
            return this.relationships.Remove(relationship);
        }

        #endregion Add and remove relationships

        #region Add and remove entities

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

            this.ModelStore.Entity.Add(entity.ModelItem);
            this.entities.Add(entity);
            return entity;
        }

        public bool Remove(EntityViewModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.ModelStore.Entity.Remove(entity.ModelItem);

            if (this.entities.Remove(entity))
            {
                // remove connected items
                this.Items
                    .OfType<RelationshipViewModel>()
                    .Where(r => r.From.Entity.Equals(entity) || r.To.Entity.Equals(entity))
                    .ToList()
                    .ForEach(r => this.Remove(r));

                return true;
            }

            return false;
        }

        #endregion Add and remove entities

        #region Add and remove tags

        public TagViewModel Add(TagViewModel tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");

            // handle adding same tag twice: ignor new tag, return existing one
            if (this.Tags.Contains(tag))
                return tag;

            // handle duplicate names by throwing
            if (this.Tags.Any(t => t.Name.Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new InvalidOperationException("Tag name not unique");

            this.ModelStore.Tag.Add(tag.ModelItem);
            this.Tags.Add(tag);

            return tag;
        }

        public bool Remove(TagViewModel tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");

            this.ModelStore.Tag.Remove(tag.ModelItem);
            return this.Tags.Remove(tag);
        }

        private void tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                e.OldItems
                    .Cast<TagViewModel>()
                    .ToList()
                    .ForEach(t => this.Items
                        .OfType<TaggedModelItemViewModelBase>()
                        .ToList()
                        .ForEach(ti => ti.RemovedFromModel(t)));
            }
        }

        #endregion Add and remove tags

        public void NotifyAfterDrag()
        {
            (this.AfterDrag ?? delegate { })();
        }

        public Action AfterDrag { get; set; }

        #region Layout the models nodes

        public void StartLayout()
        {
            var tmp = new SpringEmbedderLayout(100);
            tmp.Start(this.Entities.ToArray(), this.Relationships.ToArray());
        }

        private void StartLayout(IEnumerable<EntityViewModel> enumerable1, IEnumerable<RelationshipViewModel> enumerable2)
        {
            throw new NotImplementedException();
        }

        #endregion Layout the models nodes
    }
}