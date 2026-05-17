using System;
using System.ServiceModel;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            EegService serviceInstance = new EegService();
            try
            {

                using (ServiceHost host = new ServiceHost(serviceInstance))
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
            finally
            {
                serviceInstance.Dispose();
                Console.WriteLine("[Service] Resursi obrisani.");
            }
        }
    }
}
