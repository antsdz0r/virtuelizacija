using System;
using System.ServiceModel;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {

                using (ServiceHost host = new ServiceHost(typeof(EegService)))
                {
                    host.Open();
                    Console.WriteLine("[Service] EEG WCF Service started. Press ENTER to stop.");
                    Console.ReadLine();
                    host.Close();
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"[Greska] {ex.Message}");
            }
            
        }
    }
}
