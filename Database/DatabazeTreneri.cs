using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public class DatabazeTreneri
    {
        /// <summary>
        /// Odebere trenéra z databáze pomocí procedury <c>PKG_TRENERI.SP_ODEBER_TRENERI</c>
        /// </summary>
        public static void OdeberTrenera(OracleConnection conn, Trener trener)
        {
            using (var cmd = new OracleCommand("PKG_TRENERI.SP_ODEBER_TRENERI", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = trener.Id;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Přidá trenéra do databáze pomocí procedury <c>PKG_TRENERI.SP_ADD_TRENERI</c>
        /// </summary>
        public static void PridejTrenera(OracleConnection conn, Trener trener)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_TRENERI.SP_ADD_TRENERI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = trener.Id;
                    cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = trener.RodneCislo;
                    cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = trener.Jmeno;
                    cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = trener.Prijmeni;
                    cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = trener.TelefonniCislo;
                    cmd.Parameters.Add("v_specializace", OracleDbType.Varchar2).Value = trener.Specializace;
                    cmd.Parameters.Add("v_idtreninkovaskupina", OracleDbType.Int32).Value = trener.IdTreninkovaSkupina;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ADD_TRENERI: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace trenéra pomocí procedury <c>PKG_TRENERI.SP_UPDATE_TRENERI</c>
        /// </summary>
        public static void EditaceTrenera(OracleConnection conn, Trener trener)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_TRENERI.SP_UPDATE_TRENERI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = trener.Id;
                    cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = trener.RodneCislo;
                    cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = trener.Jmeno;
                    cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = trener.Prijmeni;
                    cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = trener.TelefonniCislo;
                    cmd.Parameters.Add("v_specializace", OracleDbType.Varchar2).Value = trener.Specializace;
                    cmd.Parameters.Add("v_idtreninkovaskupina", OracleDbType.Int32).Value = trener.IdTreninkovaSkupina;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_UPDATE_TRENERI: " + ex.Message, ex);
            }
        }
    }
}