using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;

namespace InformacniSystemKlub.Database
{
    /// <summary>
    /// Správce připojení k Oracle databázi
    /// Jedno sdílené připojení pro celou aplikaci
    /// </summary>
    internal static class SpravcePripojeniDatabaze
    {
        /// <summary>
        /// Sdílené připojení používané v celé aplikaci
        /// </summary>
        private static OracleConnection? sdilenePripojeni = null;

        /// <summary>
        /// Objekt pro synchronizaci práce se sdíleným připojením
        /// </summary>
        private static readonly object lockObj = new object();

        /// <summary>
        /// Vrátí připojovací řetězec z hodnot uložených v App.config
        /// </summary>
        private static string ZiskejConnectionString()
        {
            string userId = ConfigurationManager.AppSettings["OracleUserId"];
            string password = ConfigurationManager.AppSettings["OraclePassword"];
            string dataSource = ConfigurationManager.AppSettings["OracleDataSource"];

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Chybí hodnota OracleUserId v App.config");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Chybí hodnota OraclePassword v App.config");
            }

            if (string.IsNullOrWhiteSpace(dataSource))
            {
                throw new Exception("Chybí hodnota OracleDataSource v App.config");
            }

            return $"User Id={userId};Password={password};Data Source={dataSource};";
        }

        /// <summary>
        /// Vytvoří nové připojení k databázi a otevře ho
        /// </summary>
        private static OracleConnection VytvorANavazPripojeni()
        {
            OracleConnection pripojeni = new OracleConnection(ZiskejConnectionString());
            pripojeni.Open();
            return pripojeni;
        }

        /// <summary>
        /// Vrátí otevřené sdílené připojení
        /// Pokud ještě neexistuje, vytvoří ho. Pokud je zavřené, otevře ho znovu
        /// </summary>
        public static OracleConnection ZiskejSdilenePripojeni()
        {
            lock (lockObj)
            {
                if (sdilenePripojeni == null)
                {
                    sdilenePripojeni = VytvorANavazPripojeni();
                    return sdilenePripojeni;
                }

                if (sdilenePripojeni.State != ConnectionState.Open)
                {
                    sdilenePripojeni.Dispose();
                    sdilenePripojeni = VytvorANavazPripojeni();
                }

                return sdilenePripojeni;
            }
        }

        /// <summary>
        /// Zavře a zruší sdílené připojení
        /// </summary>
        public static void ZavriSdilenePripojeni()
        {
            lock (lockObj)
            {
                if (sdilenePripojeni == null)
                {
                    return;
                }

                try
                {
                    if (sdilenePripojeni.State != ConnectionState.Closed)
                    {
                        sdilenePripojeni.Close();
                    }

                    sdilenePripojeni.Dispose();
                }
                finally
                {
                    sdilenePripojeni = null;
                }
            }
        }
    }
}