namespace KosmoGraph.Persistence.MongoDb
{
    using MongoDB.Driver;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public static class MongoLoggingExtensions
    {
        public static WriteConcernResult Log(this WriteConcernResult thisWriteConcernResult, Logger log, string message)
        {
            if (thisWriteConcernResult.Ok)
            {
                log.Info((message + ":documents affected:{0},updated existing:{1}"),
                    thisWriteConcernResult.DocumentsAffected,
                    thisWriteConcernResult.UpdatedExisting);
            }
            else
            {
                log.Info((message + ":{0},updated existing:{1}:{2}"),
                    thisWriteConcernResult.ErrorMessage,
                    thisWriteConcernResult.LastErrorMessage
                );
            }
            return thisWriteConcernResult;
        }
    }
}
