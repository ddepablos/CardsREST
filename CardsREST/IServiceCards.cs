using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CardsREST.Model;

namespace CardsREST
{

    [ServiceContract]
    public interface IServiceCards
    {

        #region DummyContracts

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/gettrue")]
        Record GetTrue();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getfalse")]
        Record GetFalse();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getversion")]
        Record GetVersion();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getecho/{numero}")]
        Record GetEcho(string numero);

        #endregion

        #region CardsContracts

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getclient/{keyword}")]
        CClient GetClient(string keyword);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/addclient/{numdoc}/{name}/{phone}/{address}")]
        Response AddClient(string numdoc, string name, string phone, string address);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/addbatch/{numdoc}/{transcode}/{monto}/{factoracred}/{factorcanje}/{sumausuario}")]
        Response AddBatch(string numdoc, string transcode, string monto, string factoracred, string factorcanje, string sumausuario);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/addcard/{numdoc}")]
        Response AddCard(string numdoc);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/card/print/{numdoc}")]
        Response CardToPrint(string numdoc);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/card/active/{numdoc}")]
        Response CardToActive(string numdoc);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/card/inactive/{numdoc}")]
        Response CardToInactive(string numdoc);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/card/status/{numdoc}/{status}")]
        Response CardChangeStatus(string numdoc, string status);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getbatch/{accounttype}/{numdoc}")]
        List<CBatch> GetBatch(string accounttype, string numdoc);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getbalance/{numdoc}")]
        List<CBalance> GetBalance(string numdoc);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/getreport/{fromdate}/{untildate}/{numdoc}/{trx}")]
        List<CBatch> GetReport(string fromdate, string untildate, string numdoc = null, string trx = null);

        #endregion

    }
}
