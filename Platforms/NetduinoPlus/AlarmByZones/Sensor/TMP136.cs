
using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Sensor
{
    public class TMP136
    {
        static int SAMPLES = 10;
        public TMP136()
        {

        }

        /// <summary>
        /// Get temperature
        /// </summary>
        /// <param name="Celsius">Is degree Celcius</param>
        /// <param name="tempSensor">Temperature sensor signal pin.</param>
        /// <returns></returns>
        public static string GetTemperature(bool Celsius, Microsoft.SPOT.Hardware.AnalogInput tempSensor)
        {
            //Microsoft.SPOT.Hardware.AnalogInput tempSensor = new Microsoft.SPOT.Hardware.AnalogInput(SecretLabs.NETMF.Hardware.NetduinoPlus.AnalogChannels.ANALOG_PIN_A5);
            string temperature = string.Empty;
            
            float volts = 0;
            int i = 1;

#if MF_FRAMEWORK_VERSION_V4_1
            int vInput = tempSensor.Read();
            //float volts = ((float)vInput / 1024.0f) * 3.3f;
            while(i<=SAMPLES)
            {
                volts += ((float)vInput / 1024.0f) * 3.3f;
                i++;
            )
#endif

#if MF_FRAMEWORK_VERSION_V4_2

            double vInput = tempSensor.Read();
            //volts = ((float)vInput / 1024.0f) * 3.3f *1000;
            while (i <= SAMPLES)
            {
                //its actually millivolts
                volts += ((float)vInput / 1024.0f) * 3300.0f;
                i++;
            }
#endif
            volts = volts / SAMPLES;

            float temp_C = (volts - 0.5f);
            temp_C = temp_C * 100;

            //float F = (1.8f) * (C + 32);
            //F =  (9/5*C) +32;
            float temp_F = (1.8f * temp_C) + 32;

            temperature = Celsius ? System.Math.Round(temp_C).ToString() + " &#8451;" : System.Math.Round(temp_F).ToString() + " &#8457;";

            return temperature;
        }
    }
}
