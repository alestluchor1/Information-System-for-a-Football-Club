using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public static class DatabazeNastaveni
    {
        /// <summary>
        /// Odebere uživatele z databáze pomocí procedury <c>PKG_UZIVATELE.SP_ODEBER_UZIVATELE</c>
        /// </summary>
        public static void OdeberUzivatele(OracleConnection conn, Uzivatel uzivatel)
        {
            using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_ODEBER_UZIVATELE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("v_iduzivatelskyucet", OracleDbType.Int32).Value = uzivatel.IdUzivatelskyUcet;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Přidá uživatele do databáze pomocí procedury <c>PKG_UZIVATELE.SP_ADD_UZIVATEL</c>
        /// </summary>
        public static void PridejUzivatele(OracleConnection conn, Uzivatel uzivatel)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_ADD_UZIVATEL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_uzivatelskejmeno", OracleDbType.Varchar2).Value = uzivatel.UzivatelskeJmeno;
                    cmd.Parameters.Add("v_email", OracleDbType.Varchar2).Value = uzivatel.Email;
                    cmd.Parameters.Add("v_heslo", OracleDbType.Varchar2).Value = uzivatel.Heslo;
                    cmd.Parameters.Add("v_clenove_klubu_idclenklubu", OracleDbType.Int32).Value = uzivatel.IdClenKlubu;
                    cmd.Parameters.Add("v_salt", OracleDbType.Varchar2).Value = uzivatel.Salt;
                    cmd.Parameters.Add("v_idrole", OracleDbType.Int32).Value = uzivatel.IdRole;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání procedury SP_ADD_UZIVATEL: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existujícího uživatele pomocí procedury <c>PKG_UZIVATELE.SP_UPDATE_UZIVATEL</c>
        /// </summary>
        public static void EditaceUzivatele(OracleConnection conn, Uzivatel uzivatel)
        {
            using (var cmd = new OracleCommand("PKG_UZIVATELE.SP_UPDATE_UZIVATEL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("v_iduzivatelskyucet", OracleDbType.Int32).Value = uzivatel.IdUzivatelskyUcet;
                cmd.Parameters.Add("v_uzivatelskejmeno", OracleDbType.Varchar2).Value = uzivatel.UzivatelskeJmeno;
                cmd.Parameters.Add("v_email", OracleDbType.Varchar2).Value = uzivatel.Email;
                cmd.Parameters.Add("v_heslo", OracleDbType.Varchar2).Value = uzivatel.Heslo;
                cmd.Parameters.Add("v_clenove_klubu_idclenklubu", OracleDbType.Int32).Value = uzivatel.IdClenKlubu;
                cmd.Parameters.Add("v_salt", OracleDbType.Varchar2).Value = uzivatel.Salt;
                cmd.Parameters.Add("v_idrole", OracleDbType.Int32).Value = uzivatel.IdRole;

                cmd.ExecuteNonQuery();
            }
        }
    }
}