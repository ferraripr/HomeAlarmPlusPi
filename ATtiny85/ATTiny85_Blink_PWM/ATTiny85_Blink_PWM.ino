/*  
  Code for ATtiny PWM
  fades LEDs on all five pins on and off using software PWM
  
  Source from: http://www.kobakant.at/DIY/?p=3393
 */
#include <Arduino.h>
#include <wiring.h>
 
//#define fadeSpeed 20
#define fadeSpeed 6

int led = 0;

void setup()
{
  /* initialize the digital pin as an output.*/
  pinMode(led, OUTPUT); 
}

void loop()
{
  for(int fade=1;fade<254;fade++) 
  { 
   /*fade on*/
   softPWM(led, fade, fadeSpeed);
  }
  
  for(int fade=254;fade>1;fade--)
  {
    /*fade off*/
    softPWM(led, fade, fadeSpeed);
  }
}

void softPWM(int pin, int freq, int sp) 
{ 
  /* software PWM function that fakes analog output */
  digitalWrite(pin,HIGH); /*on*/
  delayMicroseconds(sp*freq);
  digitalWrite(pin,LOW); /*off*/
  delayMicroseconds(sp*(255-freq));
}
