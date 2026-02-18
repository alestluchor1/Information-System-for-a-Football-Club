using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;

namespace InformacniSystemKlub.Database
{
    /// <summary>
    /// Správce připojení k Oracle databázi
    /// Drží jedno sdílené připojení pro celou aplikaci
    /// </summary>
    internal static class SpravcePripojeniDatabaze
    {
        /// <summary>
        /// Připojovací řetězec 
        /// </summary>
        private const string connectionString ="User Id=ADMIN;Password=Lenovotluchor01_;Data Source=schooldb_medium;";

        /// <summary>
        /// Sdílené připojení
        /// </summary>
        private static OracleConnection? sdilenePripojeni = null;

        private static readonly object lockObj = new object();

        private static bool oracleKonfiguraceNastavena = false;

        /// <summary>
        /// Najde složku s wallet soubory
        /// </summary>
        private static string ZiskejWalletPath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string wallet = Path.Combine(baseDir, "Wallet_schooldb");

            if (!Directory.Exists(wallet))
            {
                string parent = Directory.GetParent(baseDir)?.FullName;
                if (parent != null)
                {
                    string wallet2 = Path.Combine(parent, "Wallet_schooldb");
                    if (Directory.Exists(wallet2))
                    {
                        wallet = wallet2;
                    }
                }
            }

            if (!Directory.Exists(wallet))
            {
                throw new Exception("Nenalezen Oracle Wallet. Vložte složku 'Wallet_schooldb' vedle aplikace (bin\\Debug...).");
            }

            return wallet;
        }

        /// <summary>
        /// Nastaví Oracle wallet konfiguraci 
        /// </summary>
        private static void NastavOracleWallet()
        {
            if (oracleKonfiguraceNastavena)
            {
                return;
            }

            string walletPath = ZiskejWalletPath();

            OracleConfiguration.TnsAdmin = walletPath;
            OracleConfiguration.WalletLocation = walletPath;

            oracleKonfiguraceNastavena = true;
        }

        /// <summary>
        /// Vrátí otevřené sdílené připojení
        /// Pokud ještě neexistuje, vytvoří ho. Pokud je zavřené, otevře ho
        /// </summary>
        public static OracleConnection ZiskejSdilenePripojeni()
        {
            lock (lockObj)
            {
                NastavOracleWallet();

                if (sdilenePripojeni == null)
                {
                    sdilenePripojeni = new OracleConnection(connectionString);
                }

                if (sdilenePripojeni.State != ConnectionState.Open)
                {
                    sdilenePripojeni.Open();
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
