
namespace KosmoGraph.Persistence.MongoDb
{
    using KosmoGraph.Model;
    using MongoDB.Driver.Builders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed partial class RelationshipRepository
    {
        public IEnumerable<Relationship> FindByEntityIdentity(Guid guid)
        {
            var query = Query.Or(
                    Query.EQ("fromId", guid),
                    Query.EQ("toId", guid));

            foreach (var relationship in this.relationshipCollection.Value.Find(query))
                yield return relationship;
        }

        public void RemoveByEntityIdentity(Guid guid)
        {
            log.Debug("Removing all relationships by entityId '{0}'", guid);

            var query = Query.Or(
                    Query.EQ("fromId", guid),
                    Query.EQ("toId", guid));

            var result = this.relationshipCollection.Value.Remove(query);

            log.Info("Removed '{1}' relationships by entityId '{0}'", guid, result.DocumentsAffected);
        }
    }
}
