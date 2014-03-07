namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IRelationshipRepository
    {
        void Insert(Relationship r);
        Relationship Update(Relationship r);
        void Remove(Relationship r);

        Relationship FindByIdentity(Guid id);

        IEnumerable<Relationship> FindByEntityIdentity(Guid guid);

        void RemoveByEntityIdentity(Guid guid);

        IEnumerable<Relationship> GetAll();
    }
}
