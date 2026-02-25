using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Database
{
    public class DatabazeOpatreni
    {
        /// <summary>
        /// Odebere opatření z databáze pomocí procedury <c>PKG_OPATRENI.SP_DELETE_OPATRENI</c>
        /// </summary>
        public static void OdeberOpatreni(OracleConnection conn, DisciplinarniOpatreni opatreni)
        {

            try
            {
                using (var cmd = new OracleCommand("PKG_OPATRENI.SP_DELETE_OPATRENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("v_id_opatreni", OracleDbType.Int32).Value = opatreni.IdOpatreni;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_DELETE_OPATRENI: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Přidá opatření do databáze pomocí procedury <c>PKG_OPATRENI.SP_ADD_OPATRENI</c>
        /// </summary>
        public static void PridejOpatreni(OracleConnection conn, DisciplinarniOpatreni opatreni)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_OPATRENI.SP_ADD_OPATRENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("v_id_clenklubu", OracleDbType.Int32).Value = opatreni.IdHrac;
                    cmd.Parameters.Add("v_datum_ukonceni", OracleDbType.Date).Value = opatreni.DatumUkonceniOpatreni;
                    cmd.Parameters.Add("v_duvod", OracleDbType.Varchar2, 50).Value = opatreni.Duvod;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ADD_OPATRENI: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existujícího opatření pomocí procedury <c>PKG_OPATRENI.SP_UPDATE_OPATRENI</c> 
        /// </summary>
        public static void EditaceOpatreni(OracleConnection conn, DisciplinarniOpatreni opatreni)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_OPATRENI.SP_UPDATE_OPATRENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("v_id_opatreni", OracleDbType.Int32).Value = opatreni.IdOpatreni;
                    cmd.Parameters.Add("v_datum_ukonceni", OracleDbType.Date).Value = opatreni.DatumUkonceniOpatreni;
                    cmd.Parameters.Add("v_duvod", OracleDbType.Varchar2, 50).Value = opatreni.Duvod;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_UPDATE_OPATRENI: " + ex.Message, ex);
            }
        }
    }
}
