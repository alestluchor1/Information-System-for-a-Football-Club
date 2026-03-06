using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace InformacniSystemKlub.Database
{
    public class DatabazeNominace
    {
        /// <summary>
        /// Přidá nominaci do databáze pomocí procedury <c>PKG_NOMINACE.SP_ADD_NOMINACE</c>
        /// </summary>
        public static void PridejNominaci(OracleConnection conn, NominaceNaZapas nominace)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_NOMINACE.SP_ADD_NOMINACE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idhrace", OracleDbType.Int32).Value = nominace.IdHrace;
                    cmd.Parameters.Add("v_idzapas", OracleDbType.Int32).Value = nominace.IdZapas;
                    cmd.Parameters.Add("v_pocetodehranychminut", OracleDbType.Int32).Value = nominace.PocetOdehranychMinut;
                    cmd.Parameters.Add("v_pocetgolu", OracleDbType.Int32).Value = nominace.PocetGolu;
                    cmd.Parameters.Add("v_zlutekarty", OracleDbType.Int32).Value = nominace.ZluteKarty;
                    cmd.Parameters.Add("v_cervenekarty", OracleDbType.Int32).Value = nominace.CerveneKarty;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_NOMINACE.SP_ADD_NOMINACE: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Editace existující nominace pomocí procedury <c>PKG_NOMINACE.SP_UPDATE_NOMINACE</c> 
        /// </summary>
        public static void EditujNominaci(OracleConnection conn, NominaceNaZapas nominace)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_NOMINACE.SP_UPDATE_NOMINACE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idnominace", OracleDbType.Int32).Value = nominace.IdNominace;
                    cmd.Parameters.Add("v_pocetodehranychminut", OracleDbType.Int32).Value = nominace.PocetOdehranychMinut;
                    cmd.Parameters.Add("v_pocetgolu", OracleDbType.Int32).Value = nominace.PocetGolu;
                    cmd.Parameters.Add("v_zlutekarty", OracleDbType.Int32).Value = nominace.ZluteKarty;
                    cmd.Parameters.Add("v_cervenekarty", OracleDbType.Int32).Value = nominace.CerveneKarty;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_NOMINACE.SP_UPDATE_NOMINACE: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Odebere nominaci z databáze pomocí procedury <c>PKG_NOMINACE.SP_DELETE_NOMINACE</c>
        /// </summary>
        public static void OdeberNominaci(OracleConnection conn, NominaceNaZapas nominace)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_NOMINACE.SP_DELETE_NOMINACE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idnominace", OracleDbType.Int32).Value = nominace.IdNominace;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_NOMINACE.SP_DELETE_NOMINACE: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Vynuluje statistiky nominací pro daný zápas pomocí procedury PKG_NOMINACE.SP_VYNUJUJ_NOMINACE_PRO_ZAPAS
        /// </summary>
        public static void VynulujNominaceProZapas(OracleConnection conn, int idZapas)
        {
            try
            {
                using (var cmd = new OracleCommand("PKG_NOMINACE.SP_VYNULUJ_NOMINACE_PRO_ZAPAS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("v_idzapas", OracleDbType.Int32).Value = idZapas;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Chyba při volání PKG_NOMINACE.SP_VYNULUJ_NOMINACE_PRO_ZAPAS: " + ex.Message, ex);
            }
        }
    }
}