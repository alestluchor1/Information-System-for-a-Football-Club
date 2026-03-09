using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    /// <summary>
    /// Databázová vrstva pro práci s uživatelskými účty
    /// Obsahuje metody pro přidání, úpravu, smazání a kontroly uživatelských účtů
    /// </summary>
    public class DatabazeUzivatelskeUcty
    {
        // IDROLE v databázi pro 1 je HOST
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
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
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
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
            }
        }

        /// <summary>
        /// Přidá nový uživatelský účet
        /// Volá proceduru <c>PKG_UZIVATELE.SP_ADD_UZIVATEL</c>
        /// </summary>
        public static void PridejUcet(OracleConnection conn, string uzivJmeno, string email, string hesloHash, int idClenKlubu, string salt, int idRole)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_ADD_UZIVATEL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelskejmeno", OracleDbType.Varchar2).Value = uzivJmeno;
                    cmd.Parameters.Add("v_email", OracleDbType.Varchar2).Value = email;
                    cmd.Parameters.Add("v_heslo", OracleDbType.Varchar2).Value = hesloHash;
                    cmd.Parameters.Add("v_clenove_klubu_idclenklubu", OracleDbType.Int32).Value = idClenKlubu == 0 ? (object)DBNull.Value : idClenKlubu;
                    cmd.Parameters.Add("v_salt", OracleDbType.Varchar2).Value = salt;
                    cmd.Parameters.Add("v_idrole", OracleDbType.Int32).Value = idRole;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
            }
        }

        /// <summary>
        /// Upraví účet podle uživatelského jména/e-mailu
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
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
            }
        }

        /// <summary>
        /// Smaže účet podle ID uživatelského účtu
        /// Volá proceduru <c>PKG_UZIVATELE.SP_ODEBER_UZIVATELE</c>
        /// </summary>
        public static void OdeberUcet(OracleConnection conn, int idUzivatelskyUcet)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_ODEBER_UZIVATELE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_iduzivatelskyucet", OracleDbType.Int32).Value = idUzivatelskyUcet;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                if (ex.Message.Contains("ORA-20022"))
                {
                    throw new Exception("Uživatelský účet nebyl nalezen!", ex);
                }

                throw new Exception("Chyba při mazání uživatele: " + ex.Message, ex);
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
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
            }
        }

        /// <summary>
        /// Vrátí typ člena klubu podle ID člena
        /// Volá proceduru <c>PKG_UZIVATELE.SP_GET_TYP_CLENA</c>
        /// </summary>
        public static string GetTypClena(OracleConnection conn, int idClenKlubu)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_GET_TYP_CLENA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = idClenKlubu;

                    var pOut = cmd.Parameters.Add("v_typclena", OracleDbType.Varchar2, 50);
                    pOut.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    if (pOut.Value == null || pOut.Value == DBNull.Value)
                    {
                        return null;
                    }

                    return pOut.Value.ToString();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
            }
        }

        /// <summary>
        /// Vrátí true, pokud už pro daného člena klubu existuje uživatelský účet
        /// Volá proceduru <c>PKG_UZIVATELE.SP_EXISTUJE_UCET_PRO_CLENA</c>
        /// </summary>
        public static bool ExistujeUcetProClena(OracleConnection conn, int idClenKlubu)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_EXISTUJE_UCET_PRO_CLENA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idclenklubu", OracleDbType.Int32).Value = idClenKlubu;

                    var pOut = cmd.Parameters.Add("v_existuje", OracleDbType.Int32);
                    pOut.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    int existuje = Convert.ToInt32(pOut.Value.ToString());

                    if (existuje == 1)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (OracleException ex)
            {
                throw new Exception(ZiskejUzivatelskouHlaskuOracleChyby(ex), ex);
            }
        }

        /// <summary>
        /// Převede Oracle chybu na srozumitelnou hlášku pro uživatele
        /// </summary>
        private static string ZiskejUzivatelskouHlaskuOracleChyby(OracleException ex)
        {
            if (ex.Number == 1)
            {
                return "Takový uživatelský účet už existuje!";
            }

            if (ex.Message.Contains("ORA-20020"))
            {
                return "Uživatelské jméno již existuje!";
            }

            if (ex.Message.Contains("ORA-20021"))
            {
                if (ex.Message.Contains("ORA-00001"))
                {
                    return "Takový uživatelský účet už existuje!";
                }

                return "Nepodařilo se přidat uživatele.";
            }

            if (ex.Message.Contains("ORA-20025"))
            {
                return "Nepodařilo se upravit uživatele.";
            }

            if (ex.Message.Contains("ORA-20023"))
            {
                return "Nepodařilo se smazat uživatele.";
            }

            if (ex.Message.Contains("ORA-20026"))
            {
                return "Nepodařilo se načíst typ člena klubu.";
            }

            if (ex.Message.Contains("ORA-20027"))
            {
                return "Nepodařilo se ověřit existenci účtu pro člena klubu.";
            }

            return "Došlo k chybě databáze: " + ex.Message;
        }
    }
}