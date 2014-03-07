namespace KosmoGraph.Model
{
    using System;

    public sealed class FacetFactory
    {
        public static Facet CreateNew()
        {
            return new Facet() { Id = Guid.NewGuid() };
        }

        public static Facet CreateNew(Action<Facet> setup)
        {
            var tmp = new Facet() { Id = Guid.NewGuid() };
            (setup ?? delegate { })(tmp);
            return tmp;
        }
    }
}