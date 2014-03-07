namespace KosmoGraph.Services
{
    using KosmoGraph.Model;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EntityRelationshipService : IManageEntitiesAndRelationships
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Construction and initialization of this instance

        public EntityRelationshipService(IEntityRepository entityRepository, IRelationshipRepository relationshipRepository)
        {
            this.entityRepository = entityRepository;
            this.relationshipRepository = relationshipRepository;
        }

        private readonly IEntityRepository entityRepository;
        private readonly IRelationshipRepository relationshipRepository;

        #endregion

        #region Manage Entities

        public Task<Entity> CreateNewEntity(Action<Entity> initializeWith)
        {
            var tmp = EntityFactory.CreateNew(initializeWith ?? delegate { });

            if (string.IsNullOrEmpty(tmp.Name))
                throw new ArgumentNullException("name");

            return Task.Factory.StartNew(() => this.entityRepository.Insert(tmp));
        }

        public Task<Entity> UpdateEntity(Entity updatedEntity)
        {
            if (string.IsNullOrEmpty(updatedEntity.Name))
                throw new ArgumentNullException("name");

            return Task.Factory.StartNew(() => this.entityRepository.Update(updatedEntity));
        }

        public bool RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            log.Debug("Removing entity '{0}' and its relationships", entity.Id);

            // send request to Db directly ?
            this.relationshipRepository.RemoveByEntityIdentity(entity.Id);

            log.Info("Removed entities '{0}' relationships", entity.Id);


            this.entityRepository.Remove(entity);

            log.Info("Removed entity '{0}' and its relationships", entity.Id);

            return true;
        }

        public void AddFacetToEntity(Entity toEntity, Facet addFacet, Action<AssignedFacet> setProperties, Action<Entity> continueWith)
        {
            // add facet to entity synchronuously...

            var assignedFacet = toEntity.Add(toEntity.CreateNewAssignedFacet(addFacet));

            (setProperties ?? delegate { })(assignedFacet);

            // ... but acccess Database async

            Task.Factory
                .StartNew(() =>
                {
                    return this.entityRepository.Update(toEntity);
                })
                .EndWith(
                    succeeded: continueWith,
                    cancelled: () => { OnCancel("Add Facet '{0}' to entity '{1}'", addFacet.Id, toEntity.Id); },
                    failed: (f) => OnFailed(f, "Add Facet '{0}' to entity '{1}'", addFacet.Id, toEntity.Id));
        }

        public Task<GetEntitiesByRelationshipResult> GetEntitiesByRelationship(Relationship relationship)
        {
            return Task.Factory
                .StartNew<GetEntitiesByRelationshipResult>(() => new GetEntitiesByRelationshipResult(relationship,
                    this.entityRepository.FindByIdentity(relationship.FromId),
                    this.entityRepository.FindByIdentity(relationship.ToId)));
        }

        public Task<IEnumerable<Entity>> GetAllEntities()
        {
            return Task.Factory.StartNew(() => this.entityRepository.GetAll());
        }

        #endregion

        #region IManageRelationships Members

        public Relationship CreatePartialRelationship(Entity fromEntity, Action<Relationship> initializeWith)
        {
            var tmp = RelationshipFactory.CreateNewPartial(fromEntity);
            (initializeWith ?? delegate { })(tmp);
            return tmp;
        }

        public Task<CompletePartialRelationshipResult> CompletePartialRelationship(Relationship relationship, Entity destinationEntity)
        {
            if (relationship == null)
                throw new ArgumentNullException("relationship");

            if (destinationEntity == null)
                throw new ArgumentNullException("destinationEntity");

            log.Debug("Creating complete relationship '{0}' from '{1}' to '{2}'", relationship.Identity, relationship.From.Id, destinationEntity.Id);

            relationship.SetDestination(destinationEntity);

            return Task.Factory.StartNew<CompletePartialRelationshipResult>(() =>
            {
                // retrieve entities in parallel
                var getEntitiesInBackground = this.GetEntitiesByRelationship(relationship);

                // insert new relatinship
                this.relationshipRepository.Insert(relationship);

                log.Info("Created complete relationship '{0}' from '{1}' to '{2}'", relationship.Identity, relationship.From.Id, destinationEntity.Id);

                return new CompletePartialRelationshipResult(
                    getEntitiesInBackground.Result.Relationship,
                    getEntitiesInBackground.Result.From,
                    getEntitiesInBackground.Result.To
                );
            });
        }

        public Task<Relationship> UpdateRelationship(Relationship updatedRelationship)
        {
            return Task.Factory.StartNew(() => this.relationshipRepository.Update(updatedRelationship));
        }

        public Task<IEnumerable<Relationship>> GetAllRelationships()
        {
            return Task.Factory.StartNew(() => this.relationshipRepository.GetAll());
        }

        #endregion

        #region Default fail/cancel handling

        private static void OnCancel(string message, params object[] parameters)
        {
            log.Warn("Operation canceled:{0}", string.Format(message, parameters));
        }

        private static bool OnFailed(Exception failure, string message, params object[] parameters)
        {
            log.Error("Operation Failed:{0}:Failure:{1}", string.Format(message, parameters), failure);
            return true; // mark exception as 'handled'
        }

        #endregion
    }
}
