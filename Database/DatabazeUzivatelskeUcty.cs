using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public class DatabazeUzivatelskeUcty
    {
        // IDROLE v databazi pro 1 je HOST
        private const int ROLE_HOST = 1;

        /// <summary>
        /// Vrátí true, pokud už existuje uživatel se stejným jménem nebo e-mailem
        /// Volá proceduru <c>PKG_REGISTRACE.SP_EXISTUJE_UZIVATEL</c>
        /// </summary>
        public static bool ExistujeUzivatel(OracleConnection conn, string uzivJmeno, string email)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_REGISTRACE.SP_EXISTUJE_UZIVATEL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;
                    cmd.Parameters.Add("v_email", OracleDbType.Varchar2).Value = email;

                    var pOut = cmd.Parameters.Add("v_existuje", OracleDbType.Int32);
                    pOut.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    int existuje = Convert.ToInt32(pOut.Value.ToString());
                    return existuje == 1;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_REGISTRACE.SP_EXISTUJE_UZIVATEL: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Vytvoří uživatele s rolí HOST
        /// Volá proceduru <c>PKG_REGISTRACE.SP_ADD_UZIVATEL</c>
        /// </summary>
        public static void PridejUcetHost(OracleConnection conn, string uzivJmeno, string email, string hesloHash, string salt)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_REGISTRACE.SP_ADD_UZIVATEL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;
                    cmd.Parameters.Add("v_email", OracleDbType.Varchar2).Value = email;
                    cmd.Parameters.Add("v_heslo", OracleDbType.Varchar2).Value = hesloHash;
                    cmd.Parameters.Add("v_salt", OracleDbType.Varchar2).Value = salt;
                    cmd.Parameters.Add("v_idrole", OracleDbType.Int32).Value = ROLE_HOST;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_REGISTRACE.SP_ADD_UZIVATEL: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Upraví účet podle uživatelského jména/e-mail
        /// Volá proceduru <c>PKG_REGISTRACE.SP_UPDATE_UZIVATEL</c>
        /// </summary>
        public static void EditujUcet(OracleConnection conn, string uzivJmeno, string email, string hesloHash, string salt, int idRole)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_REGISTRACE.SP_UPDATE_UZIVATEL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;
                    cmd.Parameters.Add("v_email", OracleDbType.Varchar2).Value = email;
                    cmd.Parameters.Add("v_heslo", OracleDbType.Varchar2).Value = hesloHash;
                    cmd.Parameters.Add("v_salt", OracleDbType.Varchar2).Value = salt;
                    cmd.Parameters.Add("v_idrole", OracleDbType.Int32).Value = idRole;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_REGISTRACE.SP_UPDATE_UZIVATEL: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Smaže účet podle uživatelského jména
        /// Volá proceduru <c>PKG_REGISTRACE.SP_DELETE_UZIVATEL</c>
        /// </summary>
        public static void OdeberUcet(OracleConnection conn, string uzivJmeno)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_REGISTRACE.SP_DELETE_UZIVATEL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_REGISTRACE.SP_DELETE_UZIVATEL: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Aktualizuje POSLEDNIPRIHLASENI na SYSDATE pro daného uživatele
        /// Volá proceduru <c>PKG_REGISTRACE.SP_UPDATE_POSLEDNI_PRIHLASENI</c>
        /// </summary>
        public static void UpdatePosledniPrihlaseni(OracleConnection conn, string uzivJmeno)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_REGISTRACE.SP_UPDATE_POSLEDNI_PRIHLASENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelske_jmeno", OracleDbType.Varchar2).Value = uzivJmeno;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_REGISTRACE.SP_UPDATE_POSLEDNI_PRIHLASENI: " + ex.Message, ex);
            }
        }
    }
}