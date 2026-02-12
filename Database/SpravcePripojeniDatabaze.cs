using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace InformacniSystemKlub.Database
{
    internal static class SpravcePripojeniDatabaze
    {
        private const string connectionString =
            "User Id=ADMIN;Password=Lenovotluchor01_;Data Source=schooldb_medium;";

        private const string walletPath = @"C:\Users\Aleš Tlučhoř\Desktop\vWallet_schooldb";

        private static OracleConnection? sdilenePripojeni = null;
        private static readonly object lockObj = new object();
        private static bool oracleKonfiguraceNastavena = false;

        private static void NastavOracleWallet()
        {
            if (oracleKonfiguraceNastavena)
            {
                return;
            }

            OracleConfiguration.TnsAdmin = walletPath;
            OracleConfiguration.WalletLocation = walletPath;

            oracleKonfiguraceNastavena = true;
        }

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
