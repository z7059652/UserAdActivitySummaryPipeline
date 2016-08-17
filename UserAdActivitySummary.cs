using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparkSchema;
using Microsoft.Bond;
using Microsoft.AdCenter.BI.UET.Schema;
using UserAdActivitySummaryPipeline;
using Newtonsoft.Json;

namespace Microsoft.Spark.CSharp.Common
{
    class UserAdActivitySummaryRaw
    {
        [JsonConverter(typeof(BondConvert))]
        public GUID UETUserId;
        
        [JsonConverter(typeof(BondConvert))]
        public UserSearchClickSummary UserSearchClickData;
        public UserAdActivitySummaryRaw()
        {

        }
        private UETUserSearchClickSpark Convert2UETUserSearchClickSpark()
        {
            UETUserSearchClickSpark uss = new UETUserSearchClickSpark();
            uss.UETUserId = this.UETUserId;
            object val = this.UserSearchClickData;
            uss.UserSearchClickData = val as UserSearchClickSummarySpark;
            return uss;
        }
    }
}
