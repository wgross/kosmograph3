
namespace Kosmograph.Cli
{
    using KosmoGraph.Model;
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Services;
    using System;
    using System.Linq;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get,"Entity")]
    [OutputType(typeof(bool))]
    public sealed class GetEntityCommand : PSCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [PSDefaultValue(Value = "kosmograph")]
        public string DatabaseName { get; set; }
    
        #region Parameterset 'byId'

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "byId", ValueFromPipelineByPropertyName = true)]
        public Guid Id { get; set; }
        
        #endregion 

        //[Parameter(Mandatory = true, Position = 1, ParameterSetName = "byPredicate", ValueFromPipeline = false, ValueFromPipelineByPropertyName = true)]
        //public string Where { get; set; }

        #region Parmeterset 'byName'

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "byName", ValueFromPipeline = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        #endregion 

        #region Parameterset 'byPage'

        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "byPage", ValueFromPipeline = false, ValueFromPipelineByPropertyName = false)]
        public int Take { get; set; }

        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "byPage", ValueFromPipeline = false, ValueFromPipelineByPropertyName = false)]
        public int Skip { get; set; }

        #endregion 
        
        protected override void BeginProcessing()
        {
            this.entityRepository = new EntityRepository(this.DatabaseName);
        }

        private EntityRepository entityRepository;

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "byId")
            {
                this.WriteObject(this.entityRepository.FindByIdentity(this.Id));
            }
            else if (this.ParameterSetName == "byName")
            {
                this.WriteObject(this.entityRepository.FindByName(this.Name));
            }
            else if (this.ParameterSetName == "byPage")
            {
                foreach (var entity in this.entityRepository.GetAll().Skip(this.Skip).Take(this.Take))
                {
                    this.WriteObject(entity);
                }
            }
        }
    }
}
