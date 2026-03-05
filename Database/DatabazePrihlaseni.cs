using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    /// <summary>
    /// Databázová třída pro přihlášení (načtení login dat + update posledního přihlášení)
    /// </summary>
    public class DatabazePrihlaseni
    {
        /// <summary>
        /// Načte login data uživatele pomocí procedury <c>PKG_PRIHLASENI.SP_GET_LOGIN_DATA</c>
        /// Když uživatel neexistuje, vrátí null
        /// </summary>
        public static Uzivatel NactiLoginData(OracleConnection conn, string uzivJmeno, out string ulozenyHash, out string ulozenySalt)
        {
            ulozenyHash = "";
            ulozenySalt = "";

            try
            {
                using (var cmd = new OracleCommand("PKG_PRIHLASENI.SP_GET_LOGIN_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    // IN parametr (posíláme do DB)
                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;

                    // OUT parametry (DB nám je vrátí zpět)
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

                    // zavolá proceduru
                    cmd.ExecuteNonQuery();

                    // přečteme, jestli existuje uživatel
                    int existuje = 0;
                    if (pExistuje.Value != null)
                    {
                        existuje = Convert.ToInt32(pExistuje.Value.ToString());
                    }

                    if (existuje != 1)
                    {
                        return null; // uživatel neexistuje
                    }

                    // načtení hodnot z OUT parametrů 
                    string uzivJmenoDb = "";
                    if (pJmeno.Value != null)
                    {
                        uzivJmenoDb = pJmeno.Value.ToString();
                    }

                    if (pHash.Value != null)
                    {
                        ulozenyHash = pHash.Value.ToString();
                    }

                    if (pSalt.Value != null)
                    {
                        ulozenySalt = pSalt.Value.ToString();
                    }

                    string role = "";
                    if (pRole.Value != null)
                    {
                        role = pRole.Value.ToString();
                    }

                    string email = "";
                    if (pEmail.Value != null)
                    {
                        email = pEmail.Value.ToString();
                    }

                    // Uložení do objektu
                    Uzivatel uzivatel = new Uzivatel();
                    uzivatel.UzivatelskeJmeno = uzivJmenoDb;
                    uzivatel.Role = role;
                    uzivatel.Email = email;

                    return uzivatel;
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