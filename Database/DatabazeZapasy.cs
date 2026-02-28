using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public static class DatabazeZapasy
    {
        /// <summary>
        /// Přidá zápas do databáze pomocí procedury <c>PKG_ZAPASY.SP_ADD_ZAPAS</c>
        /// </summary>
        public static void PridejZapas(OracleConnection conn, Zapas zapas)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_ZAPASY.SP_ADD_ZAPAS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("v_datum", OracleDbType.Date).Value = zapas.Datum;
                    cmd.Parameters.Add("v_id_stav", OracleDbType.Int32).Value = (int)zapas.IdStav;
                    cmd.Parameters.Add("v_id_typ_souteze", OracleDbType.Int32).Value = (int)zapas.IdTypSouteze;
                    cmd.Parameters.Add("v_domaci_tym", OracleDbType.Varchar2).Value = zapas.DomaciTym;
                    cmd.Parameters.Add("v_hoste_tym", OracleDbType.Varchar2).Value = zapas.HosteTym;
                    cmd.Parameters.Add("v_vysledek ", OracleDbType.Varchar2).Value = zapas.Vysledek;
                    cmd.Parameters.Add("v_pocet_zlutych_karet", OracleDbType.Int32).Value = zapas.PocetZlutychKaret;
                    cmd.Parameters.Add("v_pocet_cervenych_karet", OracleDbType.Int32).Value = zapas.PocetCervenychKaret;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ADD_ZAPAS: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Odebere zápasy z databáze pomocí procedury <c>PKG_ZAPASY.SP_ODEBER_ZAPAS</c>
        /// </summary>
        public static void OdeberZapas(OracleConnection conn, Zapas zapas)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_ZAPASY.SP_ODEBER_ZAPAS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("v_id_zapas", OracleDbType.Int32).Value = zapas.IdZapas;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ODEBER_ZAPAS: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existujícího zápasu pomocí procedury <c>PKG_ZAPASY.SP_UPDATE_ZAPAS</c> 
        /// </summary>
        public static void EditaceZapasu(OracleConnection conn, Zapas zapas)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_ZAPASY.SP_UPDATE_ZAPAS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("v_id_zapas", OracleDbType.Int32).Value = zapas.IdZapas;
                    cmd.Parameters.Add("v_datum", OracleDbType.Date).Value = zapas.Datum;
                    cmd.Parameters.Add("v_id_stav", OracleDbType.Int32).Value = (int)zapas.IdStav;
                    cmd.Parameters.Add("v_id_typ_souteze", OracleDbType.Int32).Value = (int)zapas.IdTypSouteze;
                    cmd.Parameters.Add("v_domaci_tym", OracleDbType.Varchar2).Value = zapas.DomaciTym;
                    cmd.Parameters.Add("v_hoste_tym", OracleDbType.Varchar2).Value = zapas.HosteTym;
                    cmd.Parameters.Add("v_vysledek ", OracleDbType.Varchar2).Value = zapas.Vysledek;
                    cmd.Parameters.Add("v_pocet_zlutych_karet", OracleDbType.Int32).Value = zapas.PocetZlutychKaret;
                    cmd.Parameters.Add("v_pocet_cervenych_karet", OracleDbType.Int32).Value = zapas.PocetCervenychKaret;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_UPDATE_ZAPAS: " + ex.Message, ex);
            }
        }
    }
}