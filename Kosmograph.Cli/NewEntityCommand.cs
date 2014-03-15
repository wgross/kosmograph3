namespace Kosmograph.Cli
{
    using KosmoGraph.Model;
    using KosmoGraph.Persistence.MongoDb;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.New,"Entity")]
    [OutputType(typeof(Entity))]
    public sealed  class NewEntityCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory=true,ValueFromPipelineByPropertyName=true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void BeginProcessing()
        {
            this.entityRepository = new EntityRepository(this.DatabaseName);
        }

        private EntityRepository entityRepository;

        protected override void ProcessRecord()
        {
            var entity = Entity.Factory.CreateNew(e => e.Name=this.Name);
            this.entityRepository.Insert(entity);
            this.WriteObject(entity);
        }
    }
}
