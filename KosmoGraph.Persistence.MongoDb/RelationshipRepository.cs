namespace KosmoGraph.Persistence.MongoDb
{
    using KosmoGraph.Model;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NLog;
using System;
using System.Collections.Generic;

    public sealed partial class RelationshipRepository : IRelationshipRepository
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Construction and initialization of this instance

        static RelationshipRepository()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(PropertyValue)))
            {
                // this type is shared by entity and relationship
                BsonClassMap.RegisterClassMap<PropertyValue>(map =>
                {
                    map.MapProperty(epv => epv.DefinitionId).SetElementName("definitionId");
                    map.MapProperty(epv => epv.Value).SetElementName("value");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(AssignedFacet)))
            {
                // this type is shared by entity and relationship
                BsonClassMap.RegisterClassMap<AssignedFacet>(map =>
                {
                    map.MapProperty(af => af.FacetId).SetElementName("facetId");
                    map.MapProperty(af => af.Properties).SetElementName("properties");
                });
            }
           
            BsonClassMap.RegisterClassMap<Relationship>(map =>
            {
                map.AutoMap();
                map.SetIdMember(map.GetMemberMap(r => r.Identity));
                map.UnmapProperty(r => r.From);
                map.UnmapProperty(r => r.To);
                map.GetMemberMap(r => r.FromId).SetElementName("fromId");
                map.GetMemberMap(r => r.ToId).SetElementName("toId");
                map.MapProperty(r => r.AssignedFacets).SetElementName("facets");
            });
        }

        public RelationshipRepository(string inDatabaseName)
        {
            if(string.IsNullOrEmpty(inDatabaseName))
                throw new ArgumentNullException("database name may not be empty");

            this.databaseName = inDatabaseName;
            this.relationshipCollection = new Lazy<MongoCollection<Relationship>>(() => new MongoClient().GetServer().GetDatabase(this.databaseName).GetCollection<Relationship>("relationship"));
        }

        private readonly string databaseName;

        private readonly Lazy<MongoCollection<Relationship>> relationshipCollection;

        #endregion 

        #region IRelationshipRepository Members

        public void Insert(Relationship e1)
        {
            this.relationshipCollection.Value.Insert(e1);
        }

        public Relationship FindByIdentity(Guid guid)
        {
            return this.relationshipCollection.Value.FindOneById(guid) as Relationship;
        }

        public Relationship Update(Relationship r1)
        {
            this.relationshipCollection.Value.Save(r1);
            return r1;
        }

        public void Remove(Relationship e1)
        {
            this.relationshipCollection.Value.Remove(Query<Relationship>.EQ(e => e.Identity, e1.Identity));
        }

        public IEnumerable<Relationship> GetAll()
        {
            return this.relationshipCollection.Value.FindAll();
        }
        #endregion
    }
}
