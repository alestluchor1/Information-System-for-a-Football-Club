using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public static class DatabazeHraci
    {

        /// <summary>
        /// Odebere hráče z databáze pomocí procedury <c>PKG_HRACI.SP_ODEBER_HRACE</c>
        /// </summary>
        public static void OdeberHrace(OracleConnection conn, Hrac hrac)
        {

            using (var cmd = new OracleCommand("PKG_HRACI.SP_ODEBER_HRACE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.Id;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Přidá hráče z databáze pomocí procedury <c>PKG_HRACI.SP_ADD_HRAC</c>
        /// </summary>
        public static void PridejHrace(OracleConnection conn, Hrac hrac)
        {
            try
            {

                using (var cmd = new OracleCommand("PKG_HRACI.SP_ADD_HRAC", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.Id;
                    cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = hrac.RodneCislo;
                    cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = hrac.Jmeno;
                    cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = hrac.Prijmeni;
                    cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = hrac.TelefonniCislo;
                    cmd.Parameters.Add("v_pozice", OracleDbType.Varchar2).Value = hrac.Pozice;
                    cmd.Parameters.Add("v_pocet_golu", OracleDbType.Int32).Value = hrac.PocetVstrelenychGolu;
                    cmd.Parameters.Add("v_pocet_zlute", OracleDbType.Int32).Value = hrac.PocetZlutychKaret;
                    cmd.Parameters.Add("v_pocet_cervene", OracleDbType.Int32).Value = hrac.PocetCervenychKaret;
                    cmd.Parameters.Add("v_vek", OracleDbType.Int32).Value = hrac.Vek;
                    cmd.Parameters.Add("v_zdravotni_stav", OracleDbType.Char).Value = hrac.ZdravotniStav;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ADD_HRAC: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existujícího hráče pomocí procedury <c>PKG_HRACI.SP_UPDATE_HRAC</c> 
        /// </summary>
        public static void EditaceHrace(OracleConnection conn, Hrac hrac)
        {
            using (var cmd = new OracleCommand("PKG_HRACI.SP_UPDATE_HRAC", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.Id;
                cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = hrac.RodneCislo;
                cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = hrac.Jmeno;
                cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = hrac.Prijmeni;
                cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = hrac.TelefonniCislo;
                cmd.Parameters.Add("v_pozice", OracleDbType.Varchar2).Value = hrac.Pozice;
                cmd.Parameters.Add("v_pocet_golu", OracleDbType.Int32).Value = hrac.PocetVstrelenychGolu;
                cmd.Parameters.Add("v_pocet_zlute", OracleDbType.Int32).Value = hrac.PocetZlutychKaret;
                cmd.Parameters.Add("v_pocet_cervene", OracleDbType.Int32).Value = hrac.PocetCervenychKaret;
                cmd.Parameters.Add("v_vek", OracleDbType.Int32).Value = hrac.Vek;
                cmd.Parameters.Add("v_zdravotni_stav", OracleDbType.Char).Value = hrac.ZdravotniStav;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
