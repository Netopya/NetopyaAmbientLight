/*
  Netopya Ambient Light Arduino Program
  
  This program reads LED information from the serial stream and sets TLC5940 channels to those values
  Used in a PC ambient light setup to appropriately colour 10 RGB LEDs
  
  TLC5940 library credit:
  Alex Leone <acleone ~AT~ gmail.com>, 2009-02-03 
  
*/

#include "Tlc5940.h"
char buffer[30]; //30 channels for 10 RGB LEDs

void setup()
{
  // Initialize the TLC5940
  Tlc.init();

  // Initialize Serial communication
  Serial.begin(9600);
}

void loop()
{
 
  if(Serial.available()) {
    // Read 30 bytes of information
    Serial.readBytes(buffer, 30);         
    
    // Reset the TLC5940
    Tlc.clear(); 
    
    // Set the PWM values of the channels 
    for(int i = 0; i < sizeof(buffer); i++) {
        Tlc.set(i,buffer[i]);
    }
     
    // Send the buffer to the TLC5940
    Tlc.update();
      
    // Clear any remaining information on the Serial bus
    // We will wait for new information on the next iteration of the loop
    while(Serial.available()){
      Serial.read();
    }
  }
}

