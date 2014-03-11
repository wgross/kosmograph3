
namespace KosmoGraph.Persistence.MongoDb
{
    using KosmoGraph.Model;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using KosmoGraph.Persistence.MongoDb.Tasks;

    public sealed class EntityRepository : IEntityRepository
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Construction and Initialization of this instance

        static EntityRepository()
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
           
            BsonClassMap.RegisterClassMap<Entity>(map => {
                map.AutoMap();
                map.GetMemberMap(e => e.Name).SetElementName("name");
                map.GetMemberMap(e => e.AssignedFacets).SetElementName("facets");
            });
        }

        public EntityRepository(string inDatabaseName)
        {
            if (string.IsNullOrEmpty(inDatabaseName))
                throw new ArgumentNullException("database name may not be empty");

            this.databaseName = inDatabaseName;
            this.entityCollection = new Lazy<MongoCollection<Entity>>(() =>
            {
                var entityCollection = new MongoClient().GetServer().GetDatabase(this.databaseName).GetCollection<Entity>("entity");
                entityCollection.EnsureIndex(new IndexKeysBuilder<Entity>().Ascending(e => e.Name), IndexOptions.SetBackground(true).SetUnique(true));
                return entityCollection;
            });
        }
        
        private readonly string databaseName;

        private readonly Lazy<MongoCollection<Entity>> entityCollection;

        #endregion 

        #region IEntityRepository Members

        public Entity Insert(Entity e1)
        {
            try
            {
                //Task.Factory.StartNew(delegate
                //{
                this.entityCollection.Value.Insert(e1).Log(log, string.Format("Inserting entity '{0}'", e1.Id));
                //})
                //.Then(failed: exs => { log.ErrorException("Caught", exs.First()); });
                return e1;
            }
            catch (WriteConcernException wex)
            {
                log.Error("Inserting entity '{0}' raised exception {1}", e1.Id, wex);
                throw new InvalidOperationException(string.Format("Inserting entity '{0}'", e1.Id), wex);
            }
        }

        public Entity FindByIdentity(Guid guid)
        {
            return this.entityCollection.Value.FindOneById(guid) as Entity;
        }

        public Entity Update(Entity e1)
        {
            this.entityCollection.Value.Save(e1);
            return e1;
        }

        public bool Remove(Entity e1)
        {
            return this.entityCollection
                .Value
                .Remove(Query<Entity>.EQ(e => e.Id, e1.Id))
                .Ok;
        }

        public IEnumerable<Entity> GetAll()
        {
            foreach(var e in this.entityCollection.Value.FindAll())
                yield return e;
        }

        #endregion 

        public Entity FindByName(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            
            return this.entityCollection.Value.Find(Query.EQ("name", name)).FirstOrDefault();
        }

    }
}
