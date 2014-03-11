
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

    public sealed class FacetRepository : IFacetRepository
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Construction and Initialization of this instance

        static FacetRepository()
        {
            //BsonClassMap.RegisterClassMap<PropertyDefinition>(map =>
            //{
            //    map.AutoMap();
            //});
            BsonClassMap.RegisterClassMap<Facet>(map =>
            {
                map.AutoMap();
                map.MapProperty(f => f.Properties);
            });
        }

        public FacetRepository(string inDatabaseName)
        {
            if (string.IsNullOrEmpty(inDatabaseName))
                throw new ArgumentNullException("database name may not be empty");

            this.databaseName = inDatabaseName;
            this.facetCollection = new Lazy<MongoCollection<Facet>>(() =>
            {
                var facetCollection = new MongoClient().GetServer().GetDatabase(this.databaseName).GetCollection<Facet>("facet");
                facetCollection.EnsureIndex(new IndexKeysBuilder<Facet>().Ascending(f => f.Name), IndexOptions.SetBackground(true).SetUnique(true));
                return facetCollection;
            });
        }
        
        private readonly string databaseName;

        private readonly Lazy<MongoCollection<Facet>> facetCollection;

        #endregion 

        #region IFacetRepository Members

        public Facet Insert(Facet facet)
        {
            try
            {
                this.facetCollection.Value.Insert(facet);
                return facet;
            }
            catch (WriteConcernException wex)
            {
                log.Error("Inserting facet '{0}' raised exception {1}", facet.Id, wex);
                throw new InvalidOperationException(string.Format("Inserting facet '{0}'", facet.Id), wex);
            }
        }

        public Facet FindByIdentity(Guid guid)
        {
            return this.facetCollection.Value.FindOneById(guid) as Facet;
        }

        public Facet Update(Facet facet)
        {
            this.facetCollection.Value.Save(facet);
            return facet;
        }

        public bool Remove(Facet facet)
        {
            return this.facetCollection
                .Value
                .Remove(Query<Facet>.EQ(e => e.Id, facet.Id))
                .Ok;
        }

        #endregion 
    
        #region IFacetRepository Members

        public IEnumerable<Facet> GetAll()
        {
            foreach (var facet in this.facetCollection.Value.FindAll())
                yield return facet;
        }

        #endregion
    }
}
