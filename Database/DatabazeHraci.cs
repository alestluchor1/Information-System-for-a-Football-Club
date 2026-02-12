using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public class DatabazeHraci
    {

        /// <summary>
        /// Odebere hráče z databáze pomocí procedury <c>PKG_HRACI.SP_ODEBER_HRACE</c>
        /// </summary>
        public static void OdeberHrace(OracleConnection conn, Hrac hrac)
        {

            using (var cmd = new OracleCommand("PKG_HRACI.SP_ODEBER_HRACE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.id;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Přidá hráče z databáze pomocí procedury <c>PKG_HRACI.SP_PRIDEJ_HRACE</c>
        /// </summary>
        public static void PridejHrace(OracleConnection conn, Hrac hrac)
        {
            try
            {

                using (var cmd = new OracleCommand("PKG_HRACI.SP_ADD_HRAC", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.id;
                    cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = hrac.rodneCislo;
                    cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = hrac.jmeno;
                    cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = hrac.prijmeni;
                    cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = hrac.telefonniCislo;
                    cmd.Parameters.Add("v_pozice", OracleDbType.Varchar2).Value = hrac.pozice;
                    cmd.Parameters.Add("v_pocet_golu", OracleDbType.Int32).Value = hrac.pocetVstrelenychGolu;
                    cmd.Parameters.Add("v_pocet_zlute", OracleDbType.Int32).Value = hrac.pocetZlutychKaret;
                    cmd.Parameters.Add("v_pocet_cervene", OracleDbType.Int32).Value = hrac.pocetCervenychKaret;
                    cmd.Parameters.Add("v_vek", OracleDbType.Int32).Value = hrac.vek;
                    cmd.Parameters.Add("v_zdravotni_stav", OracleDbType.Char).Value = hrac.zdravotniStav;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ADD_HRAC: " + ex.Message, ex);
            }
        }

    }
}
