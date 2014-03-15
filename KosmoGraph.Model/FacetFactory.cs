namespace KosmoGraph.Model
{
    using System;

    internal sealed class FacetFactory : IModelItemFactory<Facet>
    {
        #region IModelItemFactory<Facet> Members

        public Facet CreateNew(Action<Facet> initializeWith)
        {
            var tmp = new Facet() { Id = Guid.NewGuid() };
            (initializeWith ?? delegate { })(tmp);
            return tmp;
        }

        #endregion
    }
}