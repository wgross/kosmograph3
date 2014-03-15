//namespace KosmoGraph.Desktop.ViewModel.Test
//{
//    using KosmoGraph.Model;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;

//    public class TestData
//    {
//        public static IEnumerable<Facet> f1_empty()
//        {
//            return new[]
//            {
//                Facet.Factory.CreateNew(f => f.Name = "f1")
//            };
//        }

//        public static IEnumerable<Facet> f1_pd1()
//        {
//            return new[]
//            {
//                Facet.Factory.CreateNew(f => 
//                {
//                    f.Name = "f1";
//                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
//                })
//            };
//        }

//        public static IEnumerable<Entity> e1_f1_empty() 
//        {
//            return new[]
//            {
//                Entity.Factory.CreateNew(e => 
//                {
//                    e.Name = "e1";
//                    e.Add(e.CreateNewAssignedFacet(f1_empty.First()));
//                })
//            };
//        }

//        public static IEnumerable<Entity> e1_f1_pd1()
//        {
//            return new[]
//            {
//                Entity.Factory.CreateNew(e => 
//                {
//                    e.Name = "e1";
//                    e.Add(e.CreateNewAssignedFacet(f1_pd1.First()));
//                })
//            };
//        }
//    }
//}
