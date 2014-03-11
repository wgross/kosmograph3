namespace KosmoGraph.Desktop.View
{
    using KosmoGraph.Desktop.ViewModel;
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Services;

    public static class EntityRelationshipViewModelFactoryExtensions
    {
        public static EntityRelationshipViewModel CreateNewDefault(this EntityRelationshipViewModelFactory thisEntityRelationshipViewModelFactory)
        {
            return thisEntityRelationshipViewModelFactory.CreateNew(
                new EntityRelationshipService(
                        new EntityRepository("kosmograph"),
                        new RelationshipRepository("kosmograph")),
                new FacetService(
                        new FacetRepository("kosmograph")));

            //var e = this.Model.CreateNewEntity("new");
            //e.Top = this.ActualHeight / 2;
            //e.Left = this.ActualWidth / 2;

            //this.Model.Add(e);
        }
    }
}
