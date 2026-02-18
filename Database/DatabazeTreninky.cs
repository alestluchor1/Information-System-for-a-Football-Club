using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public class DatabazeTreninky
    {
        /// <summary>
        /// Přidá trénink z databáze pomocí procedury <c>PKG_TRENINKY.SP_ADD_TRENINK</c>
        /// </summary>
        public static void PridejTrenink(OracleConnection conn, Trenink trenink)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_TRENINKY.SP_ADD_TRENINK", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = trenink.IdTrener;
                    cmd.Parameters.Add("v_datum", OracleDbType.Date).Value = trenink.Datum;
                    cmd.Parameters.Add("v_misto", OracleDbType.Varchar2).Value = trenink.Misto;

                    if (string.IsNullOrWhiteSpace(trenink.Popis))
                    {
                        cmd.Parameters.Add("v_popis", OracleDbType.Varchar2).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("v_popis", OracleDbType.Varchar2).Value = trenink.Popis;
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_TRENINKY.SP_ADD_TRENINK: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existujícího tréninku pomocí procedury <c>PKG_TRENINKY.SP_UPDATE_TRENINK</c> 
        /// </summary>
        public static void EditujTrenink(OracleConnection conn, Trenink trenink)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_TRENINKY.SP_UPDATE_TRENINK", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idtrenink", OracleDbType.Int32).Value = trenink.Id;
                    cmd.Parameters.Add("v_datum", OracleDbType.Date).Value = trenink.Datum;
                    cmd.Parameters.Add("v_misto", OracleDbType.Varchar2).Value = trenink.Misto;

                    if (string.IsNullOrWhiteSpace(trenink.Popis))
                    {
                        cmd.Parameters.Add("v_popis", OracleDbType.Varchar2).Value = DBNull.Value;
                    }

                    else
                    {
                        cmd.Parameters.Add("v_popis", OracleDbType.Varchar2).Value = trenink.Popis;
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_TRENINKY.SP_UPDATE_TRENINK: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Odebere trénink z databáze pomocí procedury <c>PKG_TRENINKY.SP_DELETE_TRENINK</c>
        /// </summary>
        public static void OdeberTrenink(OracleConnection conn, Trenink trenink)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_TRENINKY.SP_DELETE_TRENINK", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idtrenink", OracleDbType.Int32).Value = trenink.Id;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_TRENINKY.SP_DELETE_TRENINK: " + ex.Message, ex);
            }
        }

    }
}
