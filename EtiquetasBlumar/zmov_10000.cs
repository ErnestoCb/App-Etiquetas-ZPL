using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SAP.Middleware.Connector;

namespace EtiquetasBlumar
{
    class zmov_10000
    {
        public responce_ZMOV_10000 sapRun(request_ZMOV_10000 import)
        {
            RfcDestination configSap = RfcDestinationManager.GetDestination("SCLEM");
            RfcRepository SapRfcRepository = configSap.Repository;
            IRfcFunction rfcFunction = SapRfcRepository.CreateFunction("ZMOV_10000");

            rfcFunction.SetValue("CHARG", import.CHARG);
            rfcFunction.SetValue("MATNR", import.MATNR);
            rfcFunction.Invoke(configSap);
            string aa = rfcFunction.ToString();
            responce_ZMOV_10000 res = new responce_ZMOV_10000();
            IRfcTable rfcTable_CHAR_OF_BATCH = rfcFunction.GetTable("CHAR_OF_BATCH");
            res.CHAR_OF_BATCH = new ZMOV_10000_CHAR_OF_BATCH[rfcTable_CHAR_OF_BATCH.RowCount];
            int i_CHAR_OF_BATCH = 0;
            foreach (IRfcStructure row in rfcTable_CHAR_OF_BATCH)
            {
                ZMOV_10000_CHAR_OF_BATCH datoTabla = new ZMOV_10000_CHAR_OF_BATCH();
                datoTabla.ATNAM = row.GetString("ATNAM");
                datoTabla.ATWTB = row.GetString("ATWTB");
                datoTabla.XDELETE = row.GetString("XDELETE");
                datoTabla.CHAR_NOT_VALID = row.GetString("CHAR_NOT_VALID");
                datoTabla.ATINN = row.GetInt("ATINN");
                datoTabla.ATWTB_LONG = row.GetString("ATWTB_LONG");
                res.CHAR_OF_BATCH[i_CHAR_OF_BATCH] = datoTabla; ++i_CHAR_OF_BATCH;
            }

            return res;
        }
    }
    public class request_ZMOV_10000
    {
        public String CHARG;
        public String MATNR;
    }
    public class responce_ZMOV_10000
    {
        public ZMOV_10000_CHAR_OF_BATCH[] CHAR_OF_BATCH;
    }
    public class ZMOV_10000_CHAR_OF_BATCH
    {
        public String ATNAM;
        public String ATWTB;
        public String XDELETE;
        public String CHAR_NOT_VALID;
        public Int32 ATINN;
        public String ATWTB_LONG;
    }
    
}


    


