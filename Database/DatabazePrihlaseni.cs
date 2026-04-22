using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    /// <summary>
    /// Třída zajišťující práci s databází při přihlášení uživatele
    /// Slouží pro načtení přihlašovacích údajů z databáze
    /// a také pro aktualizaci času posledního přihlášení
    /// </summary>
    public class DatabazePrihlaseni
    {
        /// <summary>
        /// Načte z databáze přihlašovací údaje uživatele podle zadaného uživatelského jména
        /// Pomocí uložené procedury vrací informace o tom, zda uživatel existuje,
        /// jeho hash hesla, salt, roli, email a ID člena klubu
        /// </summary>

        public static Uzivatel NactiLoginData(OracleConnection conn, string uzivJmeno, out string ulozenyHash, out string ulozenySalt)
        {
            ulozenyHash = "";
            ulozenySalt = "";

            try
            {
                using (var cmd = new OracleCommand("PKG_PRIHLASENI.SP_GET_LOGIN_DATA", conn))
                {
                    // Nastavení typu příkazu na stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    // Vstupní parametr - uživatelské jméno
                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;

                    // Výstupní parametr - informace, zda uživatel existuje
                    var pExistuje = cmd.Parameters.Add("v_existuje", OracleDbType.Int32);
                    pExistuje.Direction = ParameterDirection.Output;

                    var pJmeno = cmd.Parameters.Add("v_out_uziv_jmeno", OracleDbType.Varchar2, 30);
                    pJmeno.Direction = ParameterDirection.Output;

                    var pHash = cmd.Parameters.Add("v_out_hash", OracleDbType.Varchar2, 200);
                    pHash.Direction = ParameterDirection.Output;

                    var pSalt = cmd.Parameters.Add("v_out_salt", OracleDbType.Varchar2, 200);
                    pSalt.Direction = ParameterDirection.Output;

                    var pRole = cmd.Parameters.Add("v_out_role", OracleDbType.Varchar2, 50);
                    pRole.Direction = ParameterDirection.Output;

                    var pEmail = cmd.Parameters.Add("v_out_email", OracleDbType.Varchar2, 100);
                    pEmail.Direction = ParameterDirection.Output;

                    var pIdClenKlubu = cmd.Parameters.Add("v_out_idclenklubu", OracleDbType.Varchar2, 50);
                    pIdClenKlubu.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    int existuje = 0;
                    if (pExistuje.Value != null && pExistuje.Value != DBNull.Value)
                    {
                        existuje = Convert.ToInt32(pExistuje.Value.ToString());
                    }

                    if (existuje != 1)
                    {
                        return null;
                    }

                    string uzivJmenoDb = "";
                    if (pJmeno.Value != null && pJmeno.Value != DBNull.Value)
                    {
                        uzivJmenoDb = pJmeno.Value.ToString();
                    }

                    if (pHash.Value != null && pHash.Value != DBNull.Value)
                    {
                        ulozenyHash = pHash.Value.ToString();
                    }

                    if (pSalt.Value != null && pSalt.Value != DBNull.Value)
                    {
                        ulozenySalt = pSalt.Value.ToString();
                    }

                    string role = "";
                    if (pRole.Value != null && pRole.Value != DBNull.Value)
                    {
                        role = pRole.Value.ToString();
                    }

                    string email = "";
                    if (pEmail.Value != null && pEmail.Value != DBNull.Value)
                    {
                        email = pEmail.Value.ToString();
                    }

                    int idClenKlubu = 0;
                    string rawIdClenKlubu = "";

                    if (pIdClenKlubu.Value != null && pIdClenKlubu.Value != DBNull.Value)
                    {
                        rawIdClenKlubu = pIdClenKlubu.Value.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(rawIdClenKlubu) &&
                        rawIdClenKlubu.Trim().ToLower() != "null")
                    {
                        idClenKlubu = Convert.ToInt32(rawIdClenKlubu);
                    }

                    Uzivatel uzivatel = new Uzivatel();
                    uzivatel.UzivatelskeJmeno = uzivJmenoDb;
                    uzivatel.Role = role;
                    uzivatel.Email = email;
                    uzivatel.IdClenKlubu = idClenKlubu;

                    return uzivatel;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_PRIHLASENI.SP_GET_LOGIN_DATA: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Aktualizuje poslední přihlášení pomocí <c>PKG_PRIHLASENI.SP_UPDATE_POSLEDNI_PRIHLASENI</c>
        /// </summary>
        public static void UpdatePosledniPrihlaseni(OracleConnection conn, string uzivJmeno)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_PRIHLASENI.SP_UPDATE_POSLEDNI_PRIHLASENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_PRIHLASENI.SP_UPDATE_POSLEDNI_PRIHLASENI: " + ex.Message, ex);
            }
        }
    }
}