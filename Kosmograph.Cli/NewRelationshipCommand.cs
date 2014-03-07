namespace Kosmograph.Cli
{
    using KosmoGraph.Model;
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Services;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.New,"Relationship")]
    [OutputType(typeof(Relationship))]
    public sealed  class NewRelationshipCommand : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory=true,ValueFromPipelineByPropertyName=true)]
        [ValidateNotNull]
        public Entity From{ get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNull]
        public Entity To { get; set; }

        protected override void BeginProcessing()
        {
            this.entityRelationshipService = new EntityRelationshipService(new EntityRepository(this.DatabaseName),new RelationshipRepository(this.DatabaseName));
        }

        private EntityRelationshipService entityRelationshipService;

        protected override void ProcessRecord()
        {
            this.WriteObject(this.entityRelationshipService
                .CompletePartialRelationship(this.entityRelationshipService
                    .CreatePartialRelationship(this.From, delegate { }),this.To)
                .Result.Relationship);
        }
    }
}
