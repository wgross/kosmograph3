namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IModelItemFactory<TModelItem>
    {
        TModelItem CreateNew(Action<TModelItem> initializeWith);
    }
}
