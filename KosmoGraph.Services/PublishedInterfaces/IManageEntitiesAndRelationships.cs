
namespace KosmoGraph.Services
{
    using KosmoGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public sealed class GetEntitiesByRelationshipResult
    {
        public GetEntitiesByRelationshipResult (Relationship relationship, Entity from, Entity to)
	    {
            this.Relationship = relationship;
            this.From = from;
            this.To = to;
	    }

        public Relationship Relationship { get; private set; }

        public Entity From { get; private set; }

        public Entity To { get; private set; }
    }

    public sealed class CompletePartialRelationshipResult
    {
        public CompletePartialRelationshipResult(Relationship relationship, Entity from, Entity to)
        {
            this.Relationship = relationship;
            this.From = from;
            this.To = to;
        }

        public Relationship Relationship { get; private set; }

        public Entity From { get; private set; }

        public Entity To { get; private set; }
    }

    public sealed class ValidateEntityResult
    {
        public bool NameIsNullOrEmpty { get; set; }

        public bool NameIsNotUnique { get; set; }
    }

    public interface IManageEntities
    {
        Task<Entity> CreateNewEntity(Action<Entity> initializeWith);

        Task<Entity> UpdateEntity(Entity updatedEntity);

        Task<IEnumerable<Entity>> GetAllEntities();

        Task<bool> RemoveEntity(Entity entity);

        Task<ValidateEntityResult> ValidateEntity(string entityName);
    }

    public interface IManageRelationships
    {
        Relationship CreatePartialRelationship(Entity fromEntity, Action<Relationship> initializeWith);

        Task<CompletePartialRelationshipResult> CompletePartialRelationship(Relationship relationship, Entity destinationEntity);

        Task<Relationship> UpdateRelationship(Relationship toUpdate);

        Task<IEnumerable<Relationship>> GetAllRelationships();

        Task<bool> RemoveRelationship(Relationship toRemove);
    }

    public interface IManageEntitiesAndRelationships : IManageEntities, IManageRelationships
    {   
        Task<GetEntitiesByRelationshipResult> GetEntitiesByRelationship(Relationship relationship);
    }
}
