using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    /// <summary>
    /// Třída pro práci s kontrakty v databázi
    /// Obsahuje metody pro přidání, úpravu a odstranění kontraktu
    /// </summary>
    internal static class DatabazeKontrakty
    {
        /// <summary>
        /// Metoda slouží k přidání kontraktu do databáze (PKG_KONTRAKTY.SP_ADD_KONTRAKT)
        /// </summary>
        public static void PridejKontrakt(OracleConnection conn, Kontrakt kontrakt)
        {
            using (var cmd = new OracleCommand("PKG_KONTRAKTY.SP_ADD_KONTRAKT", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                // ID hráče
                cmd.Parameters.Add("v_id_clen", OracleDbType.Int32).Value = kontrakt.IdHrac;

                // Datum od / do
                cmd.Parameters.Add("v_datum_zacatku", OracleDbType.Date).Value = kontrakt.DatumZacatku.ToDateTime(TimeOnly.MinValue);

                cmd.Parameters.Add("v_datum_konce", OracleDbType.Date).Value = kontrakt.DatumKonce.ToDateTime(TimeOnly.MinValue);

                // Plat
                cmd.Parameters.Add("v_plat", OracleDbType.Int64).Value = kontrakt.Plat;

                // Telefoní číslo na agenta
                OracleParameter parametrCisloAgenta = cmd.Parameters.Add("v_cislo_na_agenta", OracleDbType.Varchar2);

                if (kontrakt.TelCisloNaAgenta == null)
                {
                    parametrCisloAgenta.Value = "";
                }
                else
                {
                    parametrCisloAgenta.Value = kontrakt.TelCisloNaAgenta;
                }

                // Výstupní klauzule
                OracleParameter parametrVystupniKlauzule = cmd.Parameters.Add("v_vystupni_klauzule", OracleDbType.Int64);

                if (kontrakt.VystupniKlauzule == 0)
                {
                    parametrVystupniKlauzule.Value = DBNull.Value;
                }
                else
                {
                    parametrVystupniKlauzule.Value = kontrakt.VystupniKlauzule;
                }

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Chyba při volání procedury SP_ADD_KONTRAKT: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Metoda slouží k odebrání kontraktu z databáze (PKG_KONTRAKTY.SP_ODEBER_KONTRAKT)
        /// </summary>
        public static void OdeberKontrakt(OracleConnection conn, Kontrakt kontrakt)
        {
            using (var cmd = new OracleCommand("PKG_KONTRAKTY.SP_ODEBER_KONTRAKT", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("v_id_clen", OracleDbType.Int32).Value = kontrakt.IdHrac;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Chyba při volání procedury SP_ODEBER_KONTRAKT: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Metoda slouží k editaci kontraktu v databázi (PKG_KONTRAKTY.SP_UPDATE_KONTRAKT)
        /// </summary>
        public static void EditujKontrakt(OracleConnection conn, Kontrakt kontrakt)
        {
            using (var cmd = new OracleCommand("PKG_KONTRAKTY.SP_UPDATE_KONTRAKT", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("v_id_clen", OracleDbType.Int32).Value = kontrakt.IdHrac;

                cmd.Parameters.Add("v_datum_zacatku", OracleDbType.Date).Value = kontrakt.DatumZacatku.ToDateTime(TimeOnly.MinValue);

                cmd.Parameters.Add("v_datum_konce", OracleDbType.Date).Value = kontrakt.DatumKonce.ToDateTime(TimeOnly.MinValue);

                cmd.Parameters.Add("v_plat", OracleDbType.Int32).Value = kontrakt.Plat;

                OracleParameter parametrCisloAgenta = cmd.Parameters.Add("v_cislo_na_agenta", OracleDbType.Varchar2);

                if (kontrakt.TelCisloNaAgenta == null)
                {
                    parametrCisloAgenta.Value = "";
                }
                else
                {
                    parametrCisloAgenta.Value = kontrakt.TelCisloNaAgenta;
                }

                OracleParameter parametrVystupniKlauzule = cmd.Parameters.Add("v_vystupni_klauzule", OracleDbType.Int32);

                if (kontrakt.VystupniKlauzule == 0)
                {
                    parametrVystupniKlauzule.Value = DBNull.Value;
                }
                else
                {
                    parametrVystupniKlauzule.Value = kontrakt.VystupniKlauzule;
                }

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Chyba při volání procedury SP_UPDATE_KONTRAKT: {ex.Message}", ex);
                }
            }
        }
    }
}