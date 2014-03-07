
//namespace KosmoGraph.Test
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using KosmoGraph.DataAccess;
//    using KosmoGraph.Contracts.DDD;

//    public static class DataAccessContextExtensions
//    {
//        public static void DeleteAll(this DataAccessContext dax)
//        {
//            dax.Entities.Query().ToList().ForEach(e => dax.Entities.Delete(e));
//            dax.Tags.Query().ToList().ForEach(e => dax.Tags.Delete(e));
//            dax.Relationships.Query().ToList().ForEach(e => dax.Relationships.Delete(e));
//            dax.SaveChanges();
//        }
//    }
//}
