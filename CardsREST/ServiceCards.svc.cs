using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

using CardsREST.Model;

namespace CardsREST
{

    public class ServiceCards : IServiceCards
    {

        private string SERVICE_NAME = "CardsREST.";

        List<Transaccion> _transactionList = new List<Transaccion>();

        private CardsEntities db = new CardsEntities();

        #region DummyMethods

        public Record GetTrue()
        {
            return new Record() { id = "0", value = "Verdadero" };
        }

        public Record GetFalse()
        {
            return new Record() { id = "1", value = "Falso" };
        }

        public List<Record> GetVersion()
        {

            List<Record> version = new List<Record>();

            version.Add(new Record() { id = "1.1.4", value = "2016-04-06 - Release 1.1.4 - Sprint 4 : Desincorporar GetReport de la batería de servicios." });
            version.Add(new Record() { id = "1.1.3", value = "2016-02-25 - Release 1.1.3 - GetReport : Fix de linq con el rango de fechas." });
            version.Add(new Record() { id = "1.1.2", value = "2016-01-29 - Release 1.1.2 - GetBatch : Fix de linq con la condición -contains- por -equals- " });
            version.Add(new Record() { id = "1.1.1", value = "2016-01-14 - Release 1.1.1 - GetBalance : Fix de linq con la condición -contains- por -equals- " });
            version.Add(new Record() { id = "1.1.0", value = "2016-01-12 - Release 1.1.0" });
            version.Add(new Record() { id = "1.0.6", value = "2015-11-17 - AddBatch : Retornar el valor de BatchID en los campos excode & exdetail / Implementación de detalles de mensajería para Operación Rechazada." });
            version.Add(new Record() { id = "1.0.5", value = "2015-10-28 - FIX Actualización del Modelo y Stored Procedures." });
            version.Add(new Record() { id = "1.0.4", value = "2015-10-27 - New card/active, card/inactive, card/changestatus : Validar y retornar mensaje de excepción cuando el cliente no existe." });
            version.Add(new Record() { id = "1.0.3", value = "2015-10-27 - FIX GetBatch : El valor del campo PUNTOS para transacciones de tipo de cuenta Lealtad." });
            version.Add(new Record() { id = "1.0.2", value = "2015-09-23 - New GetReport" });

            return version;
            
        }

        public Record GetEcho(string numero)
        {

            if (int.Parse(numero) > 9)
                numero = "0";

            string[] numbers = { "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve" };

            return new Record() { id = numero, value = numbers[int.Parse(numero)] };

        }

        #endregion

        #region CardsMethods

        /* 
         * Nombre      :    AddClient
         * Descripción :    Agregar un registro de cliente, tarjetas y cuentas. 
         * Parámetros  :    string numdoc, string name, string phone, string address
         */
        public Response AddClient(string numdoc, string name, string phone, string address)
        {

            using (CardsEntities context = new CardsEntities())
            {

                string resCode = "1";
                string resDetail = "Excepción: ";
                string resSource = SERVICE_NAME + String.Format("AddClient(string numdoc={0}, string name={1}, string phone={2}, string address={3})", numdoc , name , phone , address);

                ObjectParameter respuesta = new ObjectParameter("response", typeof(int));

                if (numdoc != null && name != null && phone != null && address != null)
                {
                    context.PLZ_ADD_CLIENTE(numdoc, name, phone, address, respuesta);
                    resCode = respuesta.Value.ToString();
                }

                resDetail = getExcepcionDetail(resCode);

                return new Response() { excode = resCode, exdetail = resDetail, exsource = resSource };

            }

        }

        /* 
         * Nombre      :    AddBatch
         * Descripción :    Agregar un registro en la tabla Batch. 
         * Parámetros  :    string numdoc, string transcode, string monto, string factoracred, string factorcanje
         */
        public Response AddBatch(string numdoc, string transcode, string monto, string factoracred, string factorcanje, string sumausuario)
        {

            string resCode = "1";
            string resDetail = "Excepción: ";
            string resSource = SERVICE_NAME + String.Format("AddBatch(string numdoc={0}, string transcode={1}, string monto={2}, string factoracred={3}, string factorcanje={4}, string sumausuario={5})", numdoc, transcode, monto, factoracred, factorcanje, sumausuario);

            using (CardsEntities context = new CardsEntities())
            {

                ObjectParameter respuesta = new ObjectParameter("response", typeof(int));

                if (numdoc != null && monto != null && transcode != null && (factoracred != null || factorcanje != null))
                {
                    if ( transcode.Equals("161"))
                    {
                        context.PLZ_ROLLBACK_BATCH(numdoc, int.Parse(monto), sumausuario, respuesta);
                    }
                    else
                    {
                        context.PLZ_ADD_BATCH1(numdoc, monto, int.Parse(transcode), decimal.Parse(factoracred), decimal.Parse(factorcanje), sumausuario, respuesta);
                    }

                    resCode = respuesta.Value.ToString();
                }

                resDetail = getExcepcionDetail(resCode);

                return new Response() { excode = resCode, exdetail = resDetail, exsource = resSource };

            }

        }

        /* 
         * Nombre      :    GetClient
         * Descripción :    Consultar uno o varios registros de clientes con atributos de Cards. 
         * Parámetros  :    string keyword
         */
        public CClient GetClient(string keyword)
        {

            using (CardsEntities context = new CardsEntities())
            {
                /*
                0	Inactiva
                1	Activa
                2	Robada
                3	Perdida
                4	Reemplazada
                5	Renovada
                6	Suspendida
                7	Eliminada                 
                 */
                if (Regex.IsMatch(keyword, @"^\d+$"))
                {
                    /* Buscar por el campo Cédula del Cliente */
                    var q = from cl in context.Clients
                            join c in context.Cards on cl.clientID equals c.clientID
                            where cl.CIDClient.Equals(keyword) 
                            orderby c.cardID descending
                            select new CClient()
                            {
                                numdoc = cl.CIDClient,
                                nameclient = cl.nameClient,
                                pan = c.PAN,
                                estatus = c.status.ToString(),
                                tarjeta = (c.status == 0 ? "Nueva"    :
                                           c.status == 1 ? "Activa"      :
                                           c.status == 2 ? "Robada"      :
                                           c.status == 3 ? "Perdida"     :
                                           c.status == 4 ? "Reemplazada" :
                                           c.status == 5 ? "Renovada"    :
                                           c.status == 6 ? "Suspendida"  : 
                                           c.status == 7 ? "Eliminada"   : ""),
                                printed = c.printed,
                                excode = "0",
                                exdetail = "Transacción ejecutada satisfactoriamente."
                            };

                    if (q.Count() == 0)
                    {
                        return new CClient() { excode = "-1", exdetail = "El registro de cliente no existe en el modelo de Cards.", numdoc = "", nameclient = "", pan = "", estatus = "", tarjeta = "", printed = "" };
                    }
                    else
                    {
                        return q.FirstOrDefault();
                    }
                }
                else
                {
                    /* Buscar por el campo Nombre del Cliente */
                    var q = from cl in context.Clients
                            join c in context.Cards on cl.clientID equals c.clientID
                            where (c.status == 0 || c.status == 1) && cl.nameClient.Contains(keyword)
                            select new CClient()
                            {
                                numdoc = cl.CIDClient,
                                nameclient = cl.nameClient,
                                pan = c.PAN,
                                estatus = c.status.ToString(),
                                tarjeta = (c.status == 0 ? "Nueva"   :
                                           c.status == 1 ? "Activa"     :
                                           c.status == 2 ? "Robada"     :
                                           c.status == 3 ? "Perdida"    :
                                           c.status == 4 ? "Reemplazada":
                                           c.status == 5 ? "Renovada"   :
                                           c.status == 6 ? "Suspendida" :
                                           c.status == 7 ? "Eliminada"  : ""),
                                printed = c.printed
                            };

                    return q.FirstOrDefault();

                }

            }

        }

        /* 
         * Nombre      :    CardToPrint
         * Parámetros  :    string numdoc
         */
        public Response CardToPrint(string numdoc)
        {
            return CardStatus(numdoc, 2);
        }

        /* 
         * Nombre      :    CardToActive
         * Parámetros  :    string numdoc
         */
        public Response CardToActive(string numdoc)
        {
            return CardStatus(numdoc, 1);
        }

        /* 
         * Nombre      :    CardToInactive
         * Parámetros  :    string numdoc
         */
        public Response CardToInactive(string numdoc)
        {
            return CardStatus(numdoc, 0);
        }

        /* 
         * Nombre      :    CardStatus
         * Descripción :    Cambiar el estatus del registro de Cards. 
         * Parámetros  :    string numdoc, int status
         */
        private Response CardStatus(string numdoc, int status)
        {

            string resCode = "1";
            string resDetail = "Excepción: ";
            string resSource = SERVICE_NAME + String.Format("CardStatus(string numdoc={0}, int status={1})", numdoc, status);

            using (CardsEntities context = new CardsEntities())
            {

                ObjectParameter respuesta = new ObjectParameter("response", typeof(int));

                if (numdoc != null)
                {
                    context.PLZ_UPD_CARD_STATUS(numdoc, status, respuesta);
                    resCode = respuesta.Value.ToString();
                }

                resDetail = getExcepcionDetail(resCode);

                return new Response() { excode = resCode, exdetail = resDetail, exsource = resSource };

            }

        }

        /* 
         * Nombre      :    getExcepcionDetail
         * Descripción :    Retornar el detalle de la excepción a partir del código de excepción. 
         * Parámetros  :    string code
         */
        private string getExcepcionDetail(string code) {

            string detail = null;

            switch (code)
            {

                case "0":
                    return "OK";
                case "1":
                    return "Operación ejecutada sin éxito.";
                case "2":
                    detail += "Los valores recibidos poseen algún valor nulo o el total de valores recibidos no es correcto.";
                    break;
                case "-2":
                    detail += "El Número de Documento no existe o no está registrado.";
                    break;
                case "-3":
                    detail += "El Número de Tarjeta no existe o no está registrado.";
                    break;
                case "4":
                    detail += "El Número de Tarjeta no existe o no está registrado.";
                    break;
                case "5":
                    detail += "El Número de Cuenta no existe o no está registrado.";
                    break;
                case "6":
                    detail += "El Código de Transacción no existe o no está registrado.";
                    break;
                /* excepciones nativas */
                case "7":
                    detail += "El Número del Documento ya se encuentra registrado, no puede ser creado el registro en Cards.";
                    break;
                case "8":
                    detail += "Operación no válida, no se puede cambiar al mismo estatus a la tarjeta.";
                    break;
                case "-4":
                    detail += "Operación no válida, no se puede ejecutar el cambio de estatus a la tarjeta.";
                    break;
                case "9":
                    detail += "El Número de la Tarjeta ya ha sido impreso.";
                    break;
                case "202":
                    detail += "El Número de Documento no posee cuentas asociadas.";
                    break;
                case "203":
                    detail += "El Estatus de la Tarjeta no es válido para la operación.";
                    break;
                case "204":
                    detail += "El Número de Documento no se encuentra registrado.";
                    break;
                case "300":
                    detail += "Operación Rechazada: La Tarjeta posee estatus de eliminada.";
                    break;
                case "308":
                    detail += "Operación Rechazada: El cambio de estatus no aplica al estatus actual de la tarjeta.";
                    break;
                case "-100":
                    detail += "Operación Rechazada: Saldo Insuficiente.";
                    break;
                case "-200":
                    detail += "Operación Rechazada: Número de Documento o Monto no son válidos.";
                    break;
                case "-300":
                    detail += "Operación Rechazada: Número de Documento no existe.";
                    break;
                case "-500":
                    detail += "Operación Rechazada: Número de Tarjeta no está activa o Número de Cuenta no válido.";
                    break;
                /*
                    DECLARE @PARAMETROSNOVALIDOS INT = -900	
                    DECLARE @CLIENTENOFOUND		 INT = -901	
                    DECLARE @TRXNOFOUND			 INT = -902
                    DECLARE @SALDOINSUFICIENTE   INT = -903
                 */
                case "-900":
                    detail += "Operación Rechazada: Los valores de los parámetros no son correctos.";
                    break;
                case "-901":
                    detail += "Operación Rechazada: El Número del Documento (Origen o Destino) no está registrado en el sistema.";
                    break;
                case "-902":
                    detail += "Operación Rechazada: El Saldo es insuficiente para completar la transacción.";
                    break;
                /*
                DECLARE @PARAMETROSNOVALIDOS INT = -910	
                DECLARE @CLIENTENOFOUND		 INT = -911	
                DECLARE @TRXNOFOUND			 INT = -912
                DECLARE @SALDOINSUFICIENTE   INT = -913
                DECLARE @TRXNOVALID			 INT = -914                     
                */
                case "-910":
                    detail += "Operación Rechazada: Los valores de los parámetros no son correctos.";
                    break;
                case "-911":
                    detail += "Operación Rechazada: El Número del Documento no está registrado en el sistema.";
                    break;
                case "-912":
                    detail += "Operación Rechazada: La transacción para ser anulada no es válida.";
                    break;
                case "-913":
                    detail += "Operación Rechazada: El Saldo es insuficiente para completar la transacción.";
                    break;
                case "-914":
                    detail += "Operación Rechazada: La Transacción no cumple con los criterios para reversar.";
                    break;
                default:
                    detail += code;
                    break;
            }

            return detail;

        }

        /* 
         * Nombre      :    GetBatch
         * Descripción :    Retornar transacciones de. 
         * Parámetros  :    string accounttype, string numdoc
         */
        public List<CBatch> GetBatch(string accounttype, string numdoc)
        {


            if (accounttype.Equals("lealtad") || accounttype.Equals("7"))
            {
                return GetBatchLealtad(numdoc);
            }
            else
            {
                return GetBatchPrepago(numdoc);
            }

        }

        /* 
         * Nombre      :    GetBatchLealtad
         * Descripción :    Retornar objeto lista de transacciones de Cuentas de Suma. 
         * Parámetros  :    string numdoc
         */
        private List<CBatch> GetBatchLealtad(string numdoc) {

            int?[] lealtad = { 203, 204, 318, 319 };

            using (CardsEntities context = new CardsEntities())
            {

                var query = (from tc in context.Transactions
                             join b in context.Batches on tc.TransCode equals b.TransCode
                             join ca in context.Cards on b.b002 equals ca.PAN
                             join cl in context.Clients on ca.clientID equals cl.clientID
                             where context.Cards.Any(c => c.PAN == b.b002) && cl.CIDClient.Equals(numdoc) && lealtad.Contains(b.TransCode)
                             select new CBatch
                             {
                                 batchid = b.BatchID,
                                 fecha = b.transDate,
                                 pan = b.b002,
                                 transcode = b.TransCode.ToString(),
                                 transname = tc.NName,
                                 saldo = b.b004.ToString(),
                                 puntos = b.puntos.ToString(),
                                 isodescription = b.IsoDescription,
                                 b037 = b.b037,
                                 batchtime = b.b012,
                                 numdoc = cl.CIDClient
                             });

                return query.ToList();

            }
        
        }

        /* 
         * Nombre      :    GetBatchPrepago
         * Descripción :    Retornar objeto lista de transacciones de Cuentas de Prepago. 
         * Parámetros  :    string numdoc
         */
        private List<CBatch> GetBatchPrepago(string numdoc)
        {

            int?[] prepago = { 145, 200, 161, 201, 202 };

            using (CardsEntities context = new CardsEntities())
            {

                var query = (from tc in context.Transactions
                             join b in context.Batches on tc.TransCode equals b.TransCode
                             join ca in context.Cards on b.b002 equals ca.PAN
                             join cl in context.Clients on ca.clientID equals cl.clientID
                             where context.Cards.Any(c => c.PAN == b.b002) && cl.CIDClient.Equals(numdoc) && prepago.Contains(b.TransCode)
                             select new CBatch
                             {
                                 batchid = b.BatchID,
                                 fecha = b.transDate,
                                 pan = b.b002,
                                 transcode = b.TransCode.ToString(),
                                 transname = tc.NName,
                                 saldo = (b.b004).ToString().Replace(".00", ""),    
                                 puntos = b.puntos.ToString(),
                                 isodescription = b.IsoDescription,
                                 b037 = b.b037,
                                 batchtime = b.b012,
                                 numdoc = cl.CIDClient
                             });

                return query.ToList();

            }

        }

        /* 
         * Nombre      :    GetBalance
         * Descripción :    Retornar objeto lista del balance de cuentas. 
         * Parámetros  :    string numdoc
         */
        public List<CBalance> GetBalance(string numdoc)
        {

            try
            {

                using (CardsEntities context = new CardsEntities())
                {

                    var q = (from at in context.AccountsTypes
                            join a in context.Accounts on at.accountType equals a.accountType
                            join c in context.Cards on a.cardID equals c.cardID
                            join cl in context.Clients on c.clientID equals cl.clientID
                            where (cl.CIDClient.Equals(numdoc))
                            select new CBalance()
                            {
                                numdoc = numdoc,
                                accounttype = a.accountType.ToString(),
                                accountname = at.nname,
                                accountnumber = a.accountNumber,
                                saldo = ( 
                                        a.accountType == 7 ? a.puntos.ToString() :
                                        a.accountType == 5 ? a.saldo.ToString() : "0"
                                        ),
                                pan = c.PAN,
                                cardstatus = (c.status == 0 && c.printed == null || (c.status == 1 && c.printed == null) ? "Nueva" :
                                              c.status == 4 ? "Reemplazada" :
                                              c.status == 6 && c.offset != null || (c.status == 0 && c.offset != null) ? "Suspendida" :
                                              c.status == 1 && c.printed != null && c.offset != null ? "Activa" :
                                             (c.status == 2 || c.status == 3) || c.status == 0 && c.offset == null ? "Bloqueada" :
                                              c.status == 6 && c.printed != null ? "Eliminada" : "")
                            });

                    return q.ToList();
                
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /* 
         * Nombre      :    AddCard
         * Descripción :    Retornar objeto lista del balance de cuentas. 
         * Parámetros  :    string numdoc
         */
        public Response AddCard(string numdoc)
        {

            using (CardsEntities context = new CardsEntities())
            {

                string resCode = "1";
                string resDetail = "Excepción: ";
                string resSource = SERVICE_NAME + String.Format( "AddCard( string numdoc={0} )", numdoc );

                ObjectParameter respuesta = new ObjectParameter("response", typeof(int));

                if (numdoc != null)
                {
                    context.PLZ_ADD_CARD(numdoc, respuesta);
                    resCode = respuesta.Value.ToString();
                }

                resDetail = getExcepcionDetail(resCode);

                return new Response() { excode = resCode, exdetail = resDetail, exsource = resSource };

            }

            throw new NotImplementedException();
        }

        /* 
         * Nombre      :    CardChangeStatus
         * Descripción :    Cambiar el estatus (cualquiera) de la tarjeta, a partir del número de documento. 
         * Parámetros  :    string numdoc, string status
         */
        public Response CardChangeStatus(string numdoc, string status)
        {
            return CardStatus(numdoc, int.Parse(status));
        }

        /* 
         * Nombre      :    AddTransfer
         * Descripción :    Implementar transacciones de transferencias. Aplica para cualquier tipo de cuenta. 
         * Parámetros  :    string origennumdoc, string destinonumdoc, string monto, string accounttype, string sumausuario
         */
        public Response AddTransfer(string origennumdoc, string destinonumdoc, string monto, string accounttype, string sumausuario)
        {

            string resCode = "1";
            string resDetail = "Excepción: ";
            string resSource = SERVICE_NAME + String.Format("AddTransfer(string origennumdoc={0}, string destinonumdoc={1}, string monto={2}, string accounttype={3}, string sumausuario={4})", origennumdoc, destinonumdoc, monto, accounttype, sumausuario);

            using (CardsEntities context = new CardsEntities())
            {

                ObjectParameter respuesta = new ObjectParameter("response", typeof(int));

                if (origennumdoc != null && destinonumdoc != null && monto != null && accounttype != null && sumausuario != null)
                {
                    context.PLZ_TRANSFERENCIA_BATCH(origennumdoc , destinonumdoc , monto , int.Parse(accounttype) , sumausuario, respuesta);
                    resCode = respuesta.Value.ToString();
                }

                resDetail = getExcepcionDetail(resCode);

                return new Response() { excode = resCode, exdetail = resDetail, exsource = resSource };

            }
        
        }
    }

    #endregion

    }