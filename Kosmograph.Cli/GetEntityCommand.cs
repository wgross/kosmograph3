
namespace Kosmograph.Cli
{
    using KosmoGraph.Model;
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Services;
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get,"Entity")]
    [OutputType(typeof(bool))]
    public sealed class GetEntityCommand : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public Guid Id { get; set; }

        protected override void BeginProcessing()
        {
            this.entityRepository = new EntityRepository(this.DatabaseName);
        }

        private EntityRepository entityRepository;

        protected override void ProcessRecord()
        {
            this.WriteObject(this.entityRepository.FindByIdentity(this.Id));
        }
    }
}
