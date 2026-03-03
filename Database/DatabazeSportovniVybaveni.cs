using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public class DatabazeSportovniVybaveni
    {
        /// <summary>
        /// Přidá sportovní vybavení do databáze pomocí procedury <c>PKG_SPORTOVNI_VYBAVENI.SP_ADD_SPORTOVNI_VYBAVENI</c>
        /// </summary>
        public static void PridejSportovniVybaveni(OracleConnection conn, SportovniVybaveni vybaveni)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_SPORTOVNI_VYBAVENI.SP_ADD_SPORTOVNI_VYBAVENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_nazev", OracleDbType.Varchar2).Value = vybaveni.Nazev;
                    cmd.Parameters.Add("v_velikost", OracleDbType.Varchar2).Value = vybaveni.Velikost;
                    cmd.Parameters.Add("v_datum_nakupu", OracleDbType.Date).Value = vybaveni.DatumNakupu;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_SPORTOVNI_VYBAVENI.SP_ADD_SPORTOVNI_VYBAVENI: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existujícího sportovního vybavení pomocí procedury <c>PKG_SPORTOVNI_VYBAVENI.SP_UPDATE_SPORTOVNI_VYBAVENI</c>
        /// </summary>
        public static void EditujSportovniVybaveni(OracleConnection conn, SportovniVybaveni vybaveni)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_SPORTOVNI_VYBAVENI.SP_UPDATE_SPORTOVNI_VYBAVENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_id_sportovni_vybaveni", OracleDbType.Int32).Value = vybaveni.IdSportovniVybaveni;
                    cmd.Parameters.Add("v_nazev", OracleDbType.Varchar2).Value = vybaveni.Nazev;
                    cmd.Parameters.Add("v_velikost", OracleDbType.Varchar2).Value = vybaveni.Velikost;
                    cmd.Parameters.Add("v_datum_nakupu", OracleDbType.Date).Value = vybaveni.DatumNakupu;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_SPORTOVNI_VYBAVENI.SP_UPDATE_SPORTOVNI_VYBAVENI: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Odebere sportovní vybavení z databáze pomocí procedury <c>PKG_SPORTOVNI_VYBAVENI.SP_DELETE_SPORTOVNI_VYBAVENI</c>
        /// </summary>
        public static void OdeberSportovniVybaveni(OracleConnection conn, SportovniVybaveni vybaveni)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_SPORTOVNI_VYBAVENI.SP_DELETE_SPORTOVNI_VYBAVENI", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_id_sportovni_vybaveni", OracleDbType.Int32).Value = vybaveni.IdSportovniVybaveni;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_SPORTOVNI_VYBAVENI.SP_DELETE_SPORTOVNI_VYBAVENI: " + ex.Message, ex);
            }
        }
    }
}