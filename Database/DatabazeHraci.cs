using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Text;

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
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.Id;
                    cmd.Parameters.Add("v_idtreninkovaskupina", OracleDbType.Int32).Value = hrac.IdTreninkovaSkupina;
                    cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = hrac.RodneCislo;
                    cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = hrac.Jmeno;
                    cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = hrac.Prijmeni;
                    cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = hrac.TelefonniCislo;
                    cmd.Parameters.Add("v_pozice", OracleDbType.Varchar2).Value = hrac.Pozice;
                    cmd.Parameters.Add("v_pocet_golu", OracleDbType.Int32).Value = hrac.PocetVstrelenychGolu;
                    cmd.Parameters.Add("v_pocet_zlute", OracleDbType.Int32).Value = hrac.PocetZlutychKaret;
                    cmd.Parameters.Add("v_pocet_cervene", OracleDbType.Int32).Value = hrac.PocetCervenychKaret;
                    int idZdravotniStav;

                    if (hrac.ZdravotniStav == 'A')
                    {
                        idZdravotniStav = 1;
                    }
                    else if (hrac.ZdravotniStav == 'N')
                    {
                        idZdravotniStav = 2;
                    }
                    else
                    {
                        throw new Exception("Neplatná hodnota zdravotního stavu. Povolené hodnoty jsou pouze A nebo N");
                    }

                    cmd.Parameters.Add("v_idzdravotnistav", OracleDbType.Int32).Value = idZdravotniStav;

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
                cmd.BindByName = true;

                cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = hrac.Id;
                cmd.Parameters.Add("v_idtreninkovaskupina", OracleDbType.Int32).Value = hrac.IdTreninkovaSkupina;
                cmd.Parameters.Add("v_rodne_cislo", OracleDbType.Varchar2).Value = hrac.RodneCislo;
                cmd.Parameters.Add("v_jmeno", OracleDbType.Varchar2).Value = hrac.Jmeno;
                cmd.Parameters.Add("v_prijmeni", OracleDbType.Varchar2).Value = hrac.Prijmeni;
                cmd.Parameters.Add("v_telefonni_cislo", OracleDbType.Varchar2).Value = hrac.TelefonniCislo;
                cmd.Parameters.Add("v_pozice", OracleDbType.Varchar2).Value = hrac.Pozice;
                cmd.Parameters.Add("v_pocet_golu", OracleDbType.Int32).Value = hrac.PocetVstrelenychGolu;
                cmd.Parameters.Add("v_pocet_zlute", OracleDbType.Int32).Value = hrac.PocetZlutychKaret;
                cmd.Parameters.Add("v_pocet_cervene", OracleDbType.Int32).Value = hrac.PocetCervenychKaret;
                int idZdravotniStav;

                if (hrac.ZdravotniStav == 'A')
                {
                    idZdravotniStav = 1;
                }
                else if (hrac.ZdravotniStav == 'N')
                {
                    idZdravotniStav = 2;
                }
                else
                {
                    throw new Exception("Neplatná hodnota zdravotního stavu. Povolené hodnoty jsou pouze A nebo N");
                }

                cmd.Parameters.Add("v_idzdravotnistav", OracleDbType.Int32).Value = idZdravotniStav;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Vrátí textový přehled kontraktů vybraného hráče
        /// Pokud hráč nemá žádný kontrakt, vrací prázdný řetězec
        /// </summary>
        public static string ZiskejKontraktyHrace(OracleConnection conn, int idHrace)
        {
            try
            {
                StringBuilder vystup = new StringBuilder();

                using (var cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "SELECT DATUMZACATKU, DATUMKONCE, PLAT, CISLONAAGENTA, VYSTUPNIKLAUZULE " +
                        "FROM KONTRAKTY " +
                        "WHERE HRAC_IDCLENKLUBU = :v_idhrace";

                    cmd.Parameters.Add("v_idhrace", OracleDbType.Int32).Value = idHrace;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string datumZacatku = "-";
                            string datumKonce = "-";
                            string plat = "-";
                            string cisloNaAgenta = "-";
                            string vystupniKlauzule = "-";

                            if (reader["DATUMZACATKU"] != DBNull.Value)
                            {
                                datumZacatku = Convert.ToDateTime(reader["DATUMZACATKU"]).ToString("dd.MM.yyyy");
                            }

                            if (reader["DATUMKONCE"] != DBNull.Value)
                            {
                                datumKonce = Convert.ToDateTime(reader["DATUMKONCE"]).ToString("dd.MM.yyyy");
                            }

                            if (reader["PLAT"] != DBNull.Value)
                            {
                                plat = reader["PLAT"].ToString();
                            }

                            if (reader["CISLONAAGENTA"] != DBNull.Value)
                            {
                                cisloNaAgenta = reader["CISLONAAGENTA"].ToString();
                            }

                            if (reader["VYSTUPNIKLAUZULE"] != DBNull.Value)
                            {
                                vystupniKlauzule = reader["VYSTUPNIKLAUZULE"].ToString();
                            }

                            vystup.AppendLine("Datum začátku: " + datumZacatku);
                            vystup.AppendLine("Datum konce: " + datumKonce);
                            vystup.AppendLine("Plat: " + plat);
                            vystup.AppendLine("Číslo na agenta: " + cisloNaAgenta);
                            vystup.AppendLine("Výstupní klauzule: " + vystupniKlauzule);
                            vystup.AppendLine("----------------------------------------");
                        }
                    }
                }

                return vystup.ToString().Trim();
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při načítání kontraktů hráče: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Vrátí textový přehled disciplinárních opatření vybraného hráče
        /// Pokud hráč nemá žádné opatření, vrací prázdný řetězec
        /// </summary>
        public static string ZiskejOpatreniHrace(OracleConnection conn, int idHrace)
        {
            try
            {
                StringBuilder vystup = new StringBuilder();

                using (var cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "SELECT o.IDOPATRENI, o.UKONCENIDATUMOPATRENI, o.DUVOD " +
                        "FROM DISCIPLINARNI_OPATRENI o " +
                        "JOIN HRACI h ON h.IDOPATRENI = o.IDOPATRENI " +
                        "WHERE h.IDCLENKLUBU = :v_idhrace";

                    cmd.Parameters.Add("v_idhrace", OracleDbType.Int32).Value = idHrace;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string idOpatreni = "-";
                            string ukonceniDatum = "-";
                            string duvod = "-";

                            if (reader["IDOPATRENI"] != DBNull.Value)
                            {
                                idOpatreni = reader["IDOPATRENI"].ToString();
                            }

                            if (reader["UKONCENIDATUMOPATRENI"] != DBNull.Value)
                            {
                                ukonceniDatum = Convert.ToDateTime(reader["UKONCENIDATUMOPATRENI"]).ToString("dd.MM.yyyy");
                            }

                            if (reader["DUVOD"] != DBNull.Value)
                            {
                                duvod = reader["DUVOD"].ToString();
                            }

                            vystup.AppendLine("ID opatření: " + idOpatreni);
                            vystup.AppendLine("Ukončení opatření: " + ukonceniDatum);
                            vystup.AppendLine("Důvod: " + duvod);
                            vystup.AppendLine("----------------------------------------");
                        }
                    }
                }

                return vystup.ToString().Trim();
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při načítání disciplinárních opatření hráče: " + ex.Message, ex);
            }
        }

    }
}
